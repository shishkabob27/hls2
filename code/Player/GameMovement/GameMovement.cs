

using Sandbox.Utility;

public partial class HL1GameMovement : BasePlayerController
{
	HLPlayer Player { get; set; }
	public Unstick Unstuck;
	protected float MaxSpeed { get; set; }
	protected float FallVelocity { get; set; }
	bool IsTouchingLadder = false;
	/// <summary>
	/// Forward direction of the player's movement.
	/// </summary>
	protected Vector3 Forward { get; set; }
	/// <summary>
	/// Right direction of the player's movement.
	/// </summary>
	protected Vector3 Right { get; set; }
	/// <summary>
	/// Up direction of the player's movement.
	/// </summary>
	protected Vector3 Up { get; set; }

	/// <summary>
	/// How much should we move forward?
	/// </summary>
	protected float ForwardMove { get; set; }
	/// <summary>
	/// How much should we move to the side?
	/// </summary>
	protected float RightMove { get; set; }
	/// <summary>
	/// How much should we move up?
	/// </summary>
	protected float UpMove { get; set; }
	/// <summary>
	/// Local eye position that is not modified by any of view punches.
	/// </summary>
	protected Vector3 PureLocalEyePosition { get; set; }
	public HL1GameMovement()
	{
		Unstuck = new Unstick( this );
	}

	public override void FrameSimulate()
	{
		base.FrameSimulate();

		if ( Player == null )
			return;

		EyeRotation = Player.ViewAngles.ToRotation();
		UpdateViewOffset();
	}

	public virtual void PawnChanged( HLPlayer player, HLPlayer prev ) { }

	public override void Simulate()
	{
		if ( Player != Pawn )
		{
			var newPlayer = Pawn as HLPlayer;
			PawnChanged( newPlayer, Player );
			Player = newPlayer;
		}
		if ( Unstuck.TestAndFix() )
			return;
		ProcessMovement();
		ShowDebugOverlay();
	}

	public virtual void ProcessMovement()
	{
		if ( Player == null )
			return;

		MaxSpeed = sv_maxspeed;
		RestoreGroundPos();
		PlayerMove();
		SaveGroundPos();
	}

	public virtual void PlayerMove()
	{
		EyeRotation = (Pawn as HLPlayer).ViewAngles.ToRotation();
		Forward = (Pawn as HLPlayer).ViewAngles.ToRotation().Forward;
		Right = (Pawn as HLPlayer).ViewAngles.ToRotation().Right;
		Up = (Pawn as HLPlayer).ViewAngles.ToRotation().Up;

		var speed = MaxSpeed;

		speed = sv_forwardspeed;



		if ( GroundEntity != null && Input.Down( "Run" ) ) speed *= sv_movespeedkey;

		ForwardMove = Player.InputDirection.x * speed;
		RightMove = -Player.InputDirection.y * speed;
		UpMove = Player.InputDirection.z * speed;

		if ( Client.IsUsingVr )
		{
			ForwardMove = Input.VR.LeftHand.Joystick.Value.y * speed;
			RightMove = Input.VR.LeftHand.Joystick.Value.x * speed;
			EyeRotation = Input.VR.Head.Rotation;
			Forward = Input.VR.Head.Rotation.Forward;
			Right = Input.VR.Head.Rotation.Right;
			Up = Input.VR.Head.Rotation.Up;
		}

		ReduceTimers();
		CheckParameters();

		if ( GroundEntity != null && Input.Down( "Use" ) ) Velocity *= 0.3f;

		// Decrease velocity if we move vertically too quickly.
		if ( Velocity.z > 250 )
		{
			ClearGroundEntity();
		}

		// remember last level type
		LastWaterLevelType = Player.WaterLevelType;

		// If we are not on ground, store how fast we are moving down
		if ( IsInAir )
		{
			FallVelocity = -Velocity.z;
		}

		SimulateDucking();
		UpdateViewOffset();
		//Player.SimulateFootsteps( Position, Velocity );
		if ( IsAlive )
		{
			if ( !LadderMove() && Player.IsOnLadder )
			{
				// Clear ladder stuff unless player is dead or riding a train
				// It will be reset immediately again next frame if necessary
				Player.IsOnLadder = false;
			}
		}
		if ( Player.IsNoclipping )
		{
			FullNoClipMove( sv_noclip_speed, sv_noclip_accelerate );

		}
		else if ( Player.IsOnLadder )
		{
			FullLadderMove();
		}
		else
		{
			FullWalkMove();
		}
	}

	public virtual void UpdateViewOffset()
	{
		// reset x,y
		EyeLocalPosition = GetPlayerViewOffset( false );

		// this updates z offset.
		SetDuckedEyeOffset( Easing.QuadraticInOut( DuckProgress ) );
	}

	public virtual void SetDuckedEyeOffset( float duckFraction )
	{
		Vector3 vDuckHullMin = GetPlayerMins( true );
		Vector3 vStandHullMin = GetPlayerMins( false );

		float fMore = vDuckHullMin.z - vStandHullMin.z;

		Vector3 vecDuckViewOffset = GetPlayerViewOffset( true );
		Vector3 vecStandViewOffset = GetPlayerViewOffset( false );
		Vector3 temp = EyeLocalPosition;

		temp.z = (vecDuckViewOffset.z - fMore) * duckFraction + vecStandViewOffset.z * (1 - duckFraction);

		EyeLocalPosition = temp;
	}

	public virtual void ReduceTimers()
	{
		if ( JumpTime > 0 )
			JumpTime = Math.Max( JumpTime - Time.Delta, 0 );
	}

	public virtual void CheckParameters()
	{
		if ( !Player.IsNoclipping )
		{
			var speed = ForwardMove * ForwardMove + RightMove * RightMove + UpMove * UpMove;

			if ( speed != 0 && speed > MaxSpeed * MaxSpeed )
			{
				var ratio = MaxSpeed / MathF.Sqrt( speed );

				ForwardMove *= ratio;
				RightMove *= ratio;
				UpMove *= ratio;
			}
		}
	}

	public virtual void StepMove( Vector3 dest )
	{
		if ( !HL1GameMovement.sv_use_sbox_movehelper )
		{
			StepMove2();
			return;
		}
		var mover = new MoveHelper( Position, Velocity );
		mover.Trace = SetupBBoxTrace( 0, 0 )
			.Ignore( Pawn );



		mover.MaxStandableAngle = sv_maxstandableangle;

		mover.TryMoveWithStep( Time.Delta, sv_stepsize );

		Position = mover.Position;
		Velocity = mover.Velocity;
	}

	public virtual void TryPlayerMove()
	{
		if ( !HL1GameMovement.sv_use_sbox_movehelper )
		{
			Move2();
			return;
		}
		MoveHelper mover = new MoveHelper( Position, Velocity );
		mover.Trace = SetupBBoxTrace( 0, 0 )
			.Ignore( Pawn );

		mover.MaxStandableAngle = sv_maxstandableangle;

		mover.TryMove( Time.Delta );

		Position = mover.Position;
		Velocity = mover.Velocity;
	}

	public virtual bool CanAccelerate()
	{
		if ( IsJumpingFromWater )
			return false;

		return true;
	}

	/// <summary>
	/// Add our wish direction and speed onto our velocity
	/// </summary>
	public virtual void Accelerate( Vector3 wishdir, float wishspeed, float acceleration )
	{
		if ( !CanAccelerate() )
			return;

		// See if we are changing direction a bit
		var speed = Velocity.Dot( wishdir );

		var addspeed = wishspeed - speed;
		if ( addspeed <= 0 )
			return;

		// Determine amount of acceleration.
		var accelspeed = acceleration * Time.Delta * wishspeed * Player.SurfaceFriction;

		// Cap at addspeed
		if ( accelspeed > addspeed )
			accelspeed = addspeed;

		Velocity += accelspeed * wishdir;
	}

	/// <summary>
	/// Add our wish direction and speed onto our velocity
	/// </summary>
	public virtual void AirAccelerate( Vector3 wishdir, float wishSpeed, float acceleration )
	{
		if ( !CanAccelerate() )
			return;

		var speedCap = sv_airspeedcap;

		var wishSpeedCapped = wishSpeed;
		if ( wishSpeedCapped > speedCap )
			wishSpeedCapped = speedCap;

		// See if we are changing direction a bit
		var currentspeed = Velocity.Dot( wishdir );

		// Reduce wishspeed by the amount of veer.
		var addspeed = wishSpeedCapped - currentspeed;

		// If not going to add any speed, done.
		if ( addspeed <= 0 )
			return;

		// Determine amount of acceleration.
		var accelspeed = acceleration * wishSpeed * Time.Delta * Player.SurfaceFriction;

		// Cap at addspeed
		if ( accelspeed > addspeed )
			accelspeed = addspeed;

		Velocity += accelspeed * wishdir;
	}

	/// <summary>
	/// Remove ground friction from velocity
	/// </summary>
	public virtual void Friction()
	{
		// If we are in water jump cycle, don't apply friction
		if ( IsJumpingFromWater )
			return;

		// Calculate speed
		var speed = Velocity.Length;
		if ( speed < 0.1f )
			return;

		float control, drop = 0;
		if ( IsGrounded )
		{
			var friction = sv_friction * Player.SurfaceFriction;

			control = (speed < sv_stopspeed) ? sv_stopspeed : speed;

			// Add the amount to the drop amount.
			drop += control * friction * Time.Delta;
		}

		// scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 )
			newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			Velocity *= newspeed;
		}
	}

	public virtual void CategorizePosition()
	{
		Player.SurfaceFriction = 1.0f;
		CheckWater();

		if ( Player.IsObserver )
			return;

		var point = Position - Vector3.Up * 2;
		var bumpOrigin = Position;

		float zvel = Velocity.z;
		bool bMovingUp = zvel > 0;
		bool bMovingUpRapidly = zvel > sv_maxnonjumpvelocity;
		float flGroundEntityVelZ = 0;

		if ( bMovingUpRapidly )
		{
			if ( IsGrounded )
			{
				flGroundEntityVelZ = GroundEntity.Velocity.z;
				bMovingUpRapidly = (zvel - flGroundEntityVelZ) > sv_maxnonjumpvelocity;
			}
		}


		if ( bMovingUpRapidly || (bMovingUp && Player.IsOnLadder) )
		{
			ClearGroundEntity();
		}
		else
		{
			var trace = TraceBBox( bumpOrigin, point );
			if ( trace.Entity == null || Vector3.GetAngle( Vector3.Up, trace.Normal ) >= sv_maxstandableangle )
			{
				trace = TryTouchGroundInQuadrants( bumpOrigin, point, trace );
				if ( trace.Entity == null || Vector3.GetAngle( Vector3.Up, trace.Normal ) >= sv_maxstandableangle )
				{
					ClearGroundEntity();

					if ( Velocity.z > 0 && !Player.IsNoclipping )
					{
						Player.SurfaceFriction = 0.25f;
					}
				}
				else
				{
					UpdateGroundEntity( trace );
				}
			}
			else
			{
				UpdateGroundEntity( trace );
			}
		}
	}

	public TraceResult TryTouchGroundInQuadrants( Vector3 start, Vector3 end, TraceResult pm )
	{
		bool isDucked = false;

		Vector3 mins, maxs;
		Vector3 minsSrc = GetPlayerMins( isDucked );
		Vector3 maxsSrc = GetPlayerMaxs( isDucked );

		float fraction = pm.Fraction;
		Vector3 endpos = pm.EndPosition;

		// Check the -x, -y quadrant
		mins = minsSrc;
		maxs = new( MathF.Min( 0, maxsSrc.x ), MathF.Min( 0, maxsSrc.y ), maxsSrc.z );

		pm = TraceBBox( start, end, mins, maxs );
		if ( pm.Entity != null && Vector3.GetAngle( Vector3.Up, pm.Normal ) >= sv_maxstandableangle )
		{
			pm.Fraction = fraction;
			pm.EndPosition = endpos;
			return pm;
		}

		// Check the +x, +y quadrant
		maxs = maxsSrc;
		mins = new( MathF.Max( 0, minsSrc.x ), MathF.Max( 0, minsSrc.y ), minsSrc.z );

		pm = TraceBBox( start, end, mins, maxs );
		if ( pm.Entity != null && Vector3.GetAngle( Vector3.Up, pm.Normal ) >= sv_maxstandableangle )
		{
			pm.Fraction = fraction;
			pm.EndPosition = endpos;
			return pm;
		}

		// Check the -x, +y quadrant
		mins = new( minsSrc.x, MathF.Max( 0, minsSrc.y ), minsSrc.z );
		maxs = new( MathF.Min( 0, maxsSrc.x ), maxsSrc.y, maxsSrc.z );

		pm = TraceBBox( start, end, mins, maxs );
		if ( pm.Entity != null && Vector3.GetAngle( Vector3.Up, pm.Normal ) >= sv_maxstandableangle )
		{
			pm.Fraction = fraction;
			pm.EndPosition = endpos;
			return pm;
		}

		// Check the +x, -y quadrant
		mins = new( MathF.Max( 0, minsSrc.x ), minsSrc.y, minsSrc.z );
		maxs = new( maxsSrc.x, MathF.Min( 0, maxsSrc.y ), maxsSrc.z );

		pm = TraceBBox( start, end, mins, maxs );
		if ( pm.Entity != null && Vector3.GetAngle( Vector3.Up, pm.Normal ) >= sv_maxstandableangle )
		{
			pm.Fraction = fraction;
			pm.EndPosition = endpos;
			return pm;
		}

		pm.Fraction = fraction;
		pm.EndPosition = endpos;
		return pm;
	}


	/// <summary>
	/// We have a new ground entity
	/// </summary>
	public virtual void UpdateGroundEntity( TraceResult tr )
	{
		var newGround = tr.Entity;
		var oldGround = GroundEntity;

		//var vecBaseVelocity = BaseVelocity;
		var vecBaseVelocity = Vector3.Zero;

		if ( oldGround == null && newGround != null )
		{
			// Subtract ground velocity at instant we hit ground jumping
			vecBaseVelocity -= newGround.Velocity;
			vecBaseVelocity.z = newGround.Velocity.z;
		}
		else if ( oldGround != null && newGround != null )
		{
			// Add in ground velocity at instant we started jumping
			vecBaseVelocity += oldGround.Velocity;
			vecBaseVelocity.z = oldGround.Velocity.z;
		}

		BaseVelocity = vecBaseVelocity;
		GroundEntity = newGround;

		// If we are on something...
		if ( newGround != null )
		{
			CategorizeGroundSurface( tr );

			// Then we are not in water jump sequence
			WaterJumpTime = 0;

			Velocity = Velocity.WithZ( 0 );
		}
	}

	/// <summary>
	/// We're no longer on the ground, remove it
	/// </summary>
	public virtual void ClearGroundEntity()
	{
		if ( GroundEntity == null )
			return;

		GroundEntity = null;
		GroundNormal = Vector3.Up;
		Player.SurfaceFriction = 1.0f;
	}

	/// <summary>
	/// Try to keep a walking player on the ground when running down slopes etc
	/// </summary>
	public virtual void StayOnGround()
	{
		var start = Position + Vector3.Up * 2;
		var end = Position + Vector3.Down * sv_stepsize;

		// See how far up we can go without getting stuck
		var trace = TraceBBox( Position, start );
		start = trace.EndPosition;

		// Now trace down from a known safe position
		trace = TraceBBox( start, end );

		if ( trace.Fraction <= 0 ) return;
		if ( trace.Fraction >= 1 ) return;
		if ( trace.StartedSolid ) return;
		if ( Vector3.GetAngle( Vector3.Up, trace.Normal ) >= sv_maxstandableangle ) return;

		Position = trace.EndPosition;
	}

	public Entity TestPlayerPosition( Vector3 pos, ref TraceResult pm )
	{
		pm = TraceBBox( pos, pos );
		return pm.Entity;
	}

	public virtual void CategorizeGroundSurface( TraceResult pm )
	{
		//Player.SurfaceData = pm.Surface;
		Player.SurfaceFriction = pm.Surface.Friction;

		Player.SurfaceFriction *= 1.25f;
		if ( Player.SurfaceFriction > 1.0f )
			Player.SurfaceFriction = 1.0f;
	}

	public bool IsAlive => Pawn.LifeState == LifeState.Alive;
	public bool IsDead => !IsAlive;
	public bool IsGrounded => GroundEntity != null;
	public bool IsInAir => !IsGrounded;

	protected virtual void ShowDebugOverlay()
	{
		if ( sv_debug_movement && Player.Client.IsListenServerHost && Game.IsServer )
		{
			DebugOverlay.ScreenText(
				$"[PLAYER]\n" +
				$"LifeState             {Player.LifeState}\n" +
				//$"TeamNumber            {Player.TeamNumber}\n" +
				$"LastAttacker          {Player.LastAttacker}\n" +
				$"LastAttackerWeapon    {Player.LastAttackerWeapon}\n" +
				$"GroundEntity          {Player.GroundEntity}\n" +
				$"\n" +

				$"[MOVEMENT]\n" +
				$"Direction             {new Vector3( Input.AnalogMove.x, -Input.AnalogMove.y, Input.AnalogMove.z )}\n" +
				$"WishVelocity          {WishVelocity}\n" +
				$"SurfaceFriction       {Player.SurfaceFriction}\n" +
				$"MoveType              Obsoleted\n" +
				$"Speed                 {Velocity.Length}\n" +
				$"MaxSpeed              {MaxSpeed}\n" +
				$"\n" +

				$"[DUCKING]\n" +
				$"IsDucked              {IsDucked}\n" +
				$"DuckTime              {DuckProgress}\n" +
				$"\n" +

				$"[OBSERVER]\n"

				, new Vector2( 60, 250 ) );
		}
	}

	[ConVar.Replicated] public static bool sv_debug_movement { get; set; }
}
