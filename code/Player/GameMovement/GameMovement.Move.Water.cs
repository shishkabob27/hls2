
public enum WaterLevelType
{
	NotInWater,
	Feet,
	Waist,
	Eyes
}

public partial class Source1GameMovement
{
	protected float WaterJumpTime { get; set; }
	protected Vector3 WaterJumpVelocity { get; set; }
	protected bool IsJumpingFromWater => WaterJumpTime > 0;
	protected TimeSince TimeSinceSwimSound { get; set; }
	protected WaterLevelType LastWaterLevelType { get; set; }

	public virtual float WaterJumpHeight => 8;

	protected void CheckWaterJump()
	{
		// Already water jumping.
		if ( IsJumpingFromWater )
			return;

		// Don't hop out if we just jumped in
		// only hop out if we are moving up
		if ( Velocity.z < -180 )
			return;

		// See if we are backing up
		var flatvelocity = Velocity.WithZ( 0 );

		// Must be moving
		var curspeed = flatvelocity.Length;
		flatvelocity = flatvelocity.Normal;

		// see if near an edge
		var flatforward = Forward.WithZ( 0 ).Normal;

		// Are we backing into water from steps or something?  If so, don't pop forward
		if ( curspeed != 0 && Vector3.Dot( flatvelocity, flatforward ) < 0 )
			return;

		var vecStart = Position + (GetPlayerMins() + GetPlayerMaxs()) * .5f;
		var vecEnd = vecStart + flatforward * 24;

		var tr = TraceBBox( vecStart, vecEnd );
		if ( tr.Fraction == 1 )
			return;

		vecStart.z = Position.z + GetPlayerViewOffset().z + WaterJumpHeight;
		vecEnd = vecStart + flatforward * 24;
		WaterJumpVelocity = tr.Normal * -50;

		tr = TraceBBox( vecStart, vecEnd );
		if ( tr.Fraction < 1.0 )
			return;

		// Now trace down to see if we would actually land on a standable surface.
		vecStart = vecEnd;
		vecEnd.z -= 1024;

		tr = TraceBBox( vecStart, vecEnd );
		if ( tr.Fraction < 1 && tr.Normal.z >= 0.7f )
		{
			Velocity = Velocity.WithZ( 256 );
			Player.Tags.Add( PlayerTags.WaterJump );
			WaterJumpTime = 2000;
		}
	}

	public virtual void WaterJump()
	{
		if ( WaterJumpTime == 0 )
			return;

		WaterJumpTime -= Time.Delta * 1000;

		if ( WaterJumpTime <= 0 || Player.WaterLevelType == WaterLevelType.NotInWater )
		{
			WaterJumpTime = 0;
			Player.Tags.Remove( PlayerTags.WaterJump );
		}

		Velocity = WaterJumpVelocity.WithZ( Velocity.z ); ;
	}

	public virtual bool CheckWater()
	{
		var vPlayerExtents = GetPlayerExtents();
		var playerHeight = vPlayerExtents.z;

		var eyeHeight = GetPlayerViewOffset().z;

		var waterFraction = Player.WaterLevel;
		var waterHeight = waterFraction * playerHeight;

		// last water type
		var lastWaterType = Player.WaterLevelType;
		Player.WaterLevelType = WaterLevelType.NotInWater;

		if ( waterFraction > 0 )
		{
			Player.WaterLevelType = WaterLevelType.Feet;

			if ( waterHeight > eyeHeight )
			{
				Player.WaterLevelType = WaterLevelType.Eyes;
			}
			else if ( waterFraction > 0.5f )
			{
				Player.WaterLevelType = WaterLevelType.Waist;
			}
		}

		// check water events
		var newWaterType = Player.WaterLevelType;
		if ( newWaterType != lastWaterType )
		{
			//if ( lastWaterType == WaterLevelType.NotInWater ) Player.OnEnterWater();
			//if ( newWaterType == WaterLevelType.NotInWater ) Player.OnLeaveWater();

			//if ( lastWaterType == WaterLevelType.Eyes ) Player.OnLeaveUnderwater();
			//if ( newWaterType == WaterLevelType.Eyes ) Player.OnEnterUnderwater();
		}

		return waterFraction >= 0.5f;
	}

	protected void WaterMove()
	{
		WishVelocity = Forward * ForwardMove + Right * RightMove;

		// if we have the jump key down, move us up as well
		if ( Input.Down( InputButton.Jump ) )
		{
			WishVelocity = WishVelocity.WithZ( WishVelocity.z + MaxSpeed );
		}

		// Sinking after no other movement occurs
		else if ( ForwardMove == 0 && RightMove == 0 && UpMove == 0 )
		{
			WishVelocity = WishVelocity.WithZ( WishVelocity.z - 60 );
		}
		else  // Go straight up by upmove amount.
		{
			// exaggerate upward movement along forward as well
			float upwardMovememnt = ForwardMove * Forward.z * 2;
			upwardMovememnt = Math.Clamp( upwardMovememnt, 0, MaxSpeed );
			WishVelocity = WishVelocity.WithZ( WishVelocity.z + UpMove + upwardMovememnt );
		}

		var wishdir = WishVelocity.Normal;
		var wishspeed = WishVelocity.Length;

		// Cap speed.
		if ( wishspeed > MaxSpeed )
		{
			WishVelocity *= MaxSpeed / wishspeed;
			wishspeed = MaxSpeed;
		}

		// Slow us down a bit.
		wishspeed *= 0.8f;

		// Water friction
		var temp = Velocity;
		var speed = temp.Length;

		var newspeed = 0f;
		if ( speed != 0 )
		{
			newspeed = speed - Time.Delta * speed * sv_friction * Player.SurfaceFriction;
			if ( newspeed < 0.1f ) newspeed = 0;
			Velocity *= newspeed / speed;
		}
		else
		{
			newspeed = 0;
		}

		// water acceleration
		if ( wishspeed >= .1f )
		{
			var addspeed = wishspeed - newspeed;
			if ( addspeed > 0 )
			{
				WishVelocity = WishVelocity.Normal;

				var accelspeed = sv_accelerate * wishspeed * Time.Delta * Player.SurfaceFriction;
				if ( accelspeed > addspeed ) accelspeed = addspeed;

				Velocity += accelspeed * WishVelocity;
			}
		}

		Velocity += BaseVelocity;

		// Now move
		// assume it is a stair or a slope, so press down from stepheight above
		var dest = Position + Velocity * Time.Delta;

		var pm = TraceBBox( Position, dest );
		if ( pm.Fraction == 1 )
		{
			var start = dest.WithZ( dest.z + sv_stepsize + 1 );

			pm = TraceBBox( start, dest );
			if ( !pm.StartedSolid )
			{
				// walked up the step, so just keep result and exit
				Position = pm.EndPosition;
				Velocity -= BaseVelocity;
				return;
			}

			// Try moving straight along out normal path.
			TryPlayerMove();
		}
		else
		{
			if ( IsInAir )
			{
				TryPlayerMove();
				Velocity -= BaseVelocity;
				return;
			}

			StepMove( dest );
		}

		Velocity -= BaseVelocity;
	}

	public bool InWater()
	{
		return Player.WaterLevelType > WaterLevelType.Feet;
	}

	public virtual void FullWalkMoveUnderwater()
	{
		if ( Player.WaterLevelType == WaterLevelType.Waist )
		{
			CheckWaterJump();
		}

		// If we are falling again, then we must not trying to jump out of water any more.
		if ( (Velocity.z < 0.0f) && IsJumpingFromWater )
		{
			WaterJumpTime = 0.0f;
		}

		// Was jump button pressed?
		if ( Input.Down( InputButton.Jump ) )
		{
			CheckJumpButton();
		}

		// Perform regular water movement
		WaterMove();

		// Redetermine position vars
		CategorizePosition();

		// If we are on ground, no downward velocity.
		if ( IsGrounded )
		{
			Velocity = Velocity.WithZ( 0 );
		}

		SetTag( "swimming" );
	}

	public virtual bool CheckWaterJumpButton()
	{
		// See if we are water jumping.  If so, decrement count and return.
		if ( IsJumpingFromWater )
		{
			WaterJumpTime -= Time.Delta;
			if ( WaterJumpTime < 0 )
			{
				WaterJumpTime = 0;
			}

			return false;
		}

		// In water above our waist.
		if ( Player.WaterLevelType >= WaterLevelType.Waist )
		{
			// Swimming, not jumping.
			ClearGroundEntity();

			// We move up a certain amount.
			Velocity = Velocity.WithZ( 100 );

			// Play swimming sound.
			if ( TimeSinceSwimSound > 1 )
			{
				// Don't play sound again for 1 second.
				TimeSinceSwimSound = 0;
				//Player.OnWaterWade();
			}

			return false;
		}

		return true;
	}
}
