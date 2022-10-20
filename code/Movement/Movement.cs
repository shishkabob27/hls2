public partial class Movement : EntityComponent
{

	private Vector3 mins = Vector3.Zero;
	private Vector3 maxs = Vector3.Zero;

	public Vector3 minsOverride;
	public Vector3 maxsOverride;

	protected float SurfaceFriction;

	// Config
	public float bGirth = 1 * 0.8f;
	public float bHeight = 1;

	public float Friction { get; set; } = 1.0f;
	public float GroundBounce { get; set; } = 0.1f;
	public float WallBounce { get; set; } = 0.1f;
	public float GroundAngle { get; set; } = 46.0f;

	public bool SnapAngleToFloor { get; set; } = false;
	public float Gravity { get; set; } = 1.0f;
	public bool DontSleep = false;

	Entity lastTouch;
	Entity lastHit;
	Vector3 lastHitNormal;

	bool ShouldSimulate = true;
	protected override void OnActivate()
	{

		base.OnActivate();
		try
		{
			if ( HLGame.sv_force_physics && Entity is ModelEntity mdl )
			{
				ShouldSimulate = false;

				var a = Entity.Velocity;
				var b = Entity.AngularVelocity;
				mdl.PhysicsEnabled = true;
				mdl.UsePhysicsCollision = true;
				var phys = mdl.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				mdl.EnableTouch = true;
				mdl.Velocity = a;

				if ( mdl.PhysicsGroup != null )
				{
					mdl.PhysicsGroup.Velocity = a * 2;
					mdl.PhysicsGroup.AngularVelocity = new Vector3( b.pitch, b.yaw, b.roll ) / 32;
				}
			}
			if (!HLGame.sv_force_physics && Entity is ModelEntity mdl2)
			{

				mdl2.PhysicsEnabled = false;
				mdl2.UsePhysicsCollision = false;
			}
		}
		catch { }
	}
	public void SetVelocity( Vector3 vel )
	{
		Entity.Velocity = vel;
		if ( HLGame.sv_force_physics && Entity is ModelEntity mdl )
		{
			if ( mdl.PhysicsGroup != null )
			{
				mdl.PhysicsGroup.Velocity = vel * 1.5f;
			}
		} 
	}
	public void SetAngularVelocity( Angles vel ) 
	{
		Entity.AngularVelocity = vel;
		if ( HLGame.sv_force_physics && Entity is ModelEntity mdl )
		{
			if ( mdl.PhysicsGroup != null )
			{
				mdl.PhysicsGroup.AngularVelocity = new Vector3( vel.yaw, vel.pitch, vel.roll ) / 50;
			}
		}
	}

	[Event.Tick]
	void Tick()
	{
		if ( ShouldSimulate ) Simulate();
	}

	public void Simulate()
	{

		if ( Entity == null ) return;
		if ( Entity.Owner is HLPlayer && Entity is HLWeapon ) return;
		if ( HLUtils.PlayerInRangeOf( Entity.Position, 2048 ) == false && !DontSleep )
			return;
		try
		{
			Entity.Velocity += Entity.BaseVelocity;
			CalcGroundEnt();
			StartGravity();
			ApplyFriction( HL1GameMovement.sv_friction * SurfaceFriction );
			ApplyAngularFriction( HL1GameMovement.sv_friction * SurfaceFriction );
			Move();
			AngularMove();
			Entity.Velocity -= Entity.BaseVelocity;
		}
		catch
		{
		}
	}

	public void AngularMove()
	{
		//Entity.Rotation = (Entity.Rotation.Angles() + (Entity.AngularVelocity * Time.Delta)).ToRotation();
		var scaledANGVEL = (Entity.AngularVelocity * Time.Delta);
		Entity.Rotation = scaledANGVEL.ToRotation() * Entity.Rotation;
		//Entity.Rotation = Entity.Rotation.RotateAroundAxis( new Vector3( 0, 0, 1 ), scaledANGVEL.yaw );
		//Entity.Rotation = Entity.Rotation.RotateAroundAxis( new Vector3( 0, 1, 0 ), scaledANGVEL.pitch );
		//Entity.Rotation = Entity.Rotation.RotateAroundAxis( new Vector3( 1, 0, 0 ), scaledANGVEL.roll );

	}
	public void Move()
	{
		if ( !HL1GameMovement.sv_use_sbox_movehelper )
		{
			Move2();
			return;
		}
		mins = new Vector3( -bGirth, -bGirth, 0 );
		maxs = new Vector3( +bGirth, +bGirth, bHeight );
		if ( minsOverride != Vector3.Zero )
		{
			mins = minsOverride;
		}
		if ( maxsOverride != Vector3.Zero )
		{
			maxs = maxsOverride;
		}
		NewMoveHelper mover = new NewMoveHelper( Entity.Position, Entity.Velocity );
		
		mover.Trace = mover.Trace
			.Size( mins, maxs )
			.Ignore( Entity )
			.WithoutTags( "player" );
		/*
		if (Entity is ModelEntity mdl)
		{
			mover.Trace = Trace.Sweep( mdl.PhysicsBody, mdl.Transform )
				.Ignore( Entity )
				.WithoutTags( "player" );
			mover.TryUnstuck();
				
		}
		*/
		mover.GroundBounce = GroundBounce;
		mover.WallBounce = WallBounce;
		mover.TryMoveWithStep( Time.Delta, HL1GameMovement.sv_stepsize );
		if ( mover.HitWall || mover.HitFloor )
		{
			if ( mover.NormalResult != lastHitNormal )
			{
				Entity.Touch( Entity );
			}
			if ( SnapAngleToFloor )
			{
				Entity.Rotation = Rotation.LookAt( mover.NormalResult ) * Rotation.From( new Angles( 90, 0, 0 ) );
			}
			lastHitNormal = mover.NormalResult;
		}

		lastTouch = mover.TraceResult.Entity;
		Entity.Position = mover.Position;
		Entity.Velocity = mover.Velocity;
	}
	public void CalcGroundEnt()
	{


		mins = new Vector3( -bGirth, -bGirth, 0 );
		maxs = new Vector3( +bGirth, +bGirth, bHeight );
		if ( minsOverride != Vector3.Zero )
		{
			mins = minsOverride;
		}
		if ( maxsOverride != Vector3.Zero )
		{
			maxs = maxsOverride;
		}
		SurfaceFriction = 1.0f;
		var point = Entity.Position - Vector3.Up * 2;
		var vBumpOrigin = Entity.Position;
		//if ( GroundEntity != null ) // and not underwater
		//{
		//bMoveToEndPos = true;
		//point.z -= 18;
		//}


		var pm = TraceBBox( vBumpOrigin, point, mins, maxs, 4.0f );

		if ( pm.Entity == null || Vector3.GetAngle( Vector3.Up, pm.Normal ) > GroundAngle )
		{
			ClearGroundEntity();
			if ( Entity.Velocity.z > 0 )
			{
				SurfaceFriction = 0.25f;
			}
		}
		else
		{
			UpdateGroundEntity( pm );
		}

	}
	public Vector3 TraceOffset = 0;

	/// <summary>
	/// Traces the bbox and returns the trace result.
	/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var tr = Trace.Ray( start + TraceOffset, end + TraceOffset )
					.Size( mins, maxs )
					.WithAnyTags( "solid" )
					.Ignore( Entity )
					.Run();

		tr.EndPosition -= TraceOffset;
		return tr;
	}
	/// <summary>
	/// We have a new ground entity
	/// </summary>
	public void UpdateGroundEntity( TraceResult tr )
	{
		var GroundNormal = tr.Normal;

		// VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
		// A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
		// This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
		SurfaceFriction = tr.Surface.Friction * 1.25f;
		if ( SurfaceFriction > 1 ) SurfaceFriction = 1;

		//if ( tr.Entity == GroundEntity ) return;

		Vector3 oldGroundVelocity = default;
		if ( Entity.GroundEntity != null ) oldGroundVelocity = Entity.GroundEntity.Velocity;

		bool wasOffGround = Entity.GroundEntity == null;

		Entity.GroundEntity = tr.Entity;

		if ( Entity.GroundEntity != null )
		{
			Entity.BaseVelocity = Entity.GroundEntity.Velocity;
		}
		if ( wasOffGround )
		{

			//this.StartTouch(this);
		}

	}

	/// <summary>
	/// We're no longer on the ground, remove it
	/// </summary>
	public void ClearGroundEntity()
	{

		if ( Entity.GroundEntity == null ) return;
		Entity.EndTouch( Entity );
		Entity.GroundEntity = null;
		var GroundNormal = Vector3.Up;
		SurfaceFriction = 1.0f;
	}

	public void ApplyFriction( float frictionAmount = 1.0f )
	{
		// If we are in water jump cycle, don't apply friction
		//if ( player->m_flWaterJumpTime )
		//   return;

		// Not on ground - no friction
		if ( Entity.GroundEntity == null )
			return;
		frictionAmount = frictionAmount + (Friction - 1);

		// Calculate speed
		var speed = Entity.Velocity.Length;
		if ( speed < 0.1f ) return;

		// Bleed off some speed, but if we have less than the bleed
		//  threshold, bleed the threshold amount.
		float control = (speed < HL1GameMovement.sv_stopspeed) ? HL1GameMovement.sv_stopspeed : speed;

		// Add the amount to the drop amount.
		var drop = control * Time.Delta * frictionAmount;

		// scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			Entity.Velocity *= newspeed;
		}

		// mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
	}
	public void ApplyAngularFriction( float frictionAmount = 1.0f )
	{
		// If we are in water jump cycle, don't apply friction
		//if ( player->m_flWaterJumpTime )
		//   return;

		// Not on ground - no friction
		if ( Entity.GroundEntity == null )
			return;
		frictionAmount = frictionAmount + (Friction - 1);

		// Calculate speed
		var speed = Entity.AngularVelocity.Length;
		if ( speed < 0.1f ) return;

		// Bleed off some speed, but if we have less than the bleed
		//  threshold, bleed the threshold amount.
		float control = (speed < HL1GameMovement.sv_stopspeed) ? HL1GameMovement.sv_stopspeed : speed;

		// Add the amount to the drop amount.
		var drop = control * Time.Delta * frictionAmount;

		// scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			Entity.AngularVelocity *= newspeed;
		}

		// mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
	}
}
