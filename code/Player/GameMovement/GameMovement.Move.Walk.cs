

public partial class HL1GameMovement
{
	public virtual void FullWalkMove()
	{
		if ( !InWater() )
		{
			StartGravity();
		}

		// If we are leaping out of the water, just update the counters.
		if ( IsJumpingFromWater )
		{
			// Try to jump out of the water (and check to see if we still are).
			WaterJump();
			TryPlayerMove();

			CheckWater();
			return;
		}

		// If we are swimming in the water, see if we are nudging against a place we can jump up out
		//  of, and, if so, start out jump.  Otherwise, if we are not moving up, then reset jump timer to 0.
		//  Also run the swim code if we're a ghost or have the TF_COND_SWIMMING_NO_EFFECTS condition
		if ( InWater() )
		{
			FullWalkMoveUnderwater();
			return;
		}

		if ( WishJump() ) CheckJumpButton();

		// Make sure velocity is valid.
		CheckVelocity();

		if ( IsGrounded )
		{
			Velocity = Velocity.WithZ( 0 );
			Friction();
			WalkMove();
		}
		else
		{
			AirMove();
		}

		// Set final flags.
		CategorizePosition();

		// Add any remaining gravitational component if we are not in water.
		if ( !InWater() )
		{
			FinishGravity();
		}

		// If we are on ground, no downward velocity.
		if ( IsGrounded )
		{
			Velocity = Velocity.WithZ( 0 );
		}

		// Handling falling.
		CheckFalling();

		// Make sure velocity is valid.
		CheckVelocity();
	}

	public void CheckFalling()
	{
		if ( IsInAir || FallVelocity <= 0 || IsDead )
			return;

		// let any subclasses know that the player has landed and how hard
		OnLand( FallVelocity );

		//
		// Clear the fall velocity so the impact doesn't happen again.
		//
		FallVelocity = 0;
	}

	public virtual void OnLand( float velocity )
	{
		// Take specified amount of fall damage when landed.
		//Player.OnLanded( velocity );
	}
	public virtual void WalkMove()
	{
		var oldGround = GroundEntity;

		var forward = Forward.WithZ( 0 ).Normal;
		var right = Right.WithZ( 0 ).Normal;

		WishVelocity = forward * ForwardMove + right * RightMove;
		WishVelocity = WishVelocity.WithZ( 0 );

		var wishspeed = WishVelocity.Length;
		var wishdir = WishVelocity.Normal;

		if ( wishspeed != 0 && wishspeed > MaxSpeed )
		{
			WishVelocity *= MaxSpeed / wishspeed;
			wishspeed = MaxSpeed;
		}
		var acceleration = sv_accelerate;

		// if our wish speed is too low, we must increase acceleration or we'll never overcome friction
		// Reverse the basic friction calculation to find our required acceleration
		var wishspeedThreshold = 100 * sv_friction / sv_accelerate;
		if ( wishspeed > 0 && wishspeed < wishspeedThreshold )
		{
			float speed = Velocity.Length;
			float flControl = (speed < sv_stopspeed) ? sv_stopspeed : speed;
			acceleration = (flControl * sv_friction) / wishspeed + 1;
		}

		Velocity = Velocity.WithZ( 0 );
		Accelerate( wishdir, wishspeed, acceleration );
		Velocity = Velocity.WithZ( 0 );

		Velocity += BaseVelocity;

		if ( Velocity.Length < 1 )
		{
			Velocity = 0;
			Velocity -= BaseVelocity;
			if ( Velocity.z <= 0 )
			{
				StayOnGround();
			}
			return;
		}

		var dest = (Position + Velocity * Time.Delta).WithZ( Position.z );
		var trace = TraceBBox( Position, dest );

		// didn't hit anything.
		if ( trace.Fraction == 1 )
		{
			Position = trace.EndPosition;
			Velocity -= BaseVelocity;
			StayOnGround();
			return;
		}

		if ( oldGround == null && Pawn.GetWaterLevel() == 0 )
		{
			Velocity -= BaseVelocity;
			return;
		}

		// If we are jumping out of water, don't do anything more.
		if ( IsJumpingFromWater )
		{
			Velocity -= BaseVelocity;
			return;
		}

		StepMove( dest );
		Velocity -= BaseVelocity;

	}

	public virtual void AirMove()
	{
		var forward = Forward.WithZ( 0 ).Normal;
		var right = Right.WithZ( 0 ).Normal;

		WishVelocity = forward * ForwardMove + right * RightMove;
		WishVelocity = WishVelocity.WithZ( 0 );

		var wishdir = WishVelocity.Normal;
		var wishspeed = WishVelocity.Length;

		if ( wishspeed != 0 && wishspeed > MaxSpeed )
		{
			WishVelocity *= MaxSpeed / wishspeed;
			wishspeed = MaxSpeed;
		}

		AirAccelerate( wishdir, wishspeed, sv_airaccelerate );

		Velocity += BaseVelocity;
		TryPlayerMove();
		Velocity -= BaseVelocity;
	}
}
