public partial class HLGib : ModelEntity
{
	//[Net] public float MaxVelocity { get; set; } = 800;
	public float gGravity { get; set; } = 800;
	public float gGroundFriction { get; set; } = 4.0f;
	public float gSurfaceFriction { get; set; } = 1.0f;
	public float gStopSpeed { get; set; } = 50.0f;
	public float gGroundAngle { get; set; } = 46.0f;
	public bool toDelete = false;
	public bool toDeletefromMax = false;
	float alpha = 1.0f;
	bool isInit = false;

	bool hasCountedFading = false;

	public Angles RotAngles = Angles.Zero;
	public Angles SleepAngles = new Angles( 270, Rand.Float( 0, 360 ), 90 );
	public Angles AltSleepAngles = new Angles( 0, Rand.Float( 0, 360 ), 0 );
	Entity SleepGroundEntity;
	Vector3 prevTickPos;
	PhysicsGroup phys;
	int LifeTime = 0;
	float bGirth = 1 * 0.5f;
	float bHeight = 1 * 0.5f;

	int BloodColour = 0;

	TraceResult lastTrace;

	/// <summary>
	/// This prop won't be able to be damaged for this amount of time
	/// </summary>
	public RealTimeUntil Invulnerable { get; set; }
	public string BreakpieceName { get; set; }
	public bool AlternateLandingRotation { get; set; } = false;
	public bool BounceSound { get; set; } = false;

	public Surface SurfaceType { get; set; }

	List<string> HGibList = new List<string>{
		//"models/hl1/gib/hgib/hgib_skull1.vmdl", spawn manually please!
		"models/hl1/gib/hgib/hgib_lung1.vmdl",
		"models/hl1/gib/hgib/hgib_legbone1.vmdl",
		"models/hl1/gib/hgib/hgib_hmeat1.vmdl",
		"models/hl1/gib/hgib/hgib_guts1.vmdl",
		"models/hl1/gib/hgib/hgib_b_gib1.vmdl",
		"models/hl1/gib/hgib/hgib_b_bone1.vmdl"
	};
	List<string> AGibList = new List<string>{
		"models/hl1/gib/agib/aliengeneric1.vmdl",
		"models/hl1/gib/agib/aliengeneric2.vmdl",
		"models/hl1/gib/agib/aliengeneric3.vmdl",
		"models/hl1/gib/agib/aliengeneric4.vmdl"
	};
	Vector3 mins;
	Vector3 maxs;

	public HLGib()
	{

		alpha = Rand.Float( 1.1f, 2.0f );
	}

	public void Spawn( string ModelName )
	{
		SetModel( ModelName );
		Initialise();
		_ = FadeOut( 30 );
	}
	public void Spawn( int Colour )
	{
		if ( Colour == 0 )
		{
			SetModel( Rand.FromList<string>( HGibList ) );
		}
		else
		{
			SetModel( Rand.FromList<string>( AGibList ) );
		}
		Initialise();
		_ = FadeOut( 30 );

	}
	public override void Spawn()
	{

		SetModel( Rand.FromList<string>( HGibList ) );
		Initialise();
		_ = FadeOut( 30 );

	}

	public async Task FadeOut( float fTime )
	{
		await Task.DelaySeconds( fTime );
		if ( !toDelete )
		{

			this.toDelete = true;
			HLCombat.GibFadingCount += 1;
		}
	}
	public new Vector3 Velocity;

	public new Angles AngularVelocity;

	public void Initialise()
	{
		if ( !HLGame.hl_classic_gibs )
		{
			var a = Velocity;
			var b = AngularVelocity;
			phys = SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			EnableTouch = true;
			Velocity = a;
			if ( PhysicsBody != null )
			{
				PhysicsBody.Velocity = a * 2;
				PhysicsBody.AngularVelocity = new Vector3( b.yaw, b.pitch, b.roll ) / 32;
			}

			this.Tags.Add( "debris" );
			if ( isInit )
				return;

			isInit = true;
			HLCombat.GibCount += 1;
			return;
		}
		if ( isInit )
			return;
		phys = SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
		isInit = true;
		HLCombat.GibCount += 1;
		Velocity += new Vector3( Rand.Int( -1, 1 ), Rand.Int( -1, 1 ), Rand.Int( -1, 1 ) );
		EnableHitboxes = true;
		Transmit = TransmitType.Always;
		this.Tags.Add( "debris" );
	}

	public virtual void StepMove()
	{
		NewMoveHelper mover = new NewMoveHelper( Position, Velocity );
		mover.Trace = mover.Trace.Size( 1, 2 ).Ignore( this );
		mover.MaxStandableAngle = 10;

		mover.TryMoveWithStep( Time.Delta, 1000 );

		Position = mover.Position;
		Velocity = mover.Velocity;
	}

	public virtual void Move( bool setpos = true )
	{
		mins = new Vector3( -bGirth, -bGirth, 0 );
		maxs = new Vector3( +bGirth, +bGirth, bHeight );
		NewMoveHelper mover = new NewMoveHelper( Position, Velocity );
		mover.Trace = mover.Trace
			.Size( mins, maxs )
			.Ignore( this )
			.WithoutTags( "player" );
		mover.GroundBounce = 0.55f;
		mover.MaxStandableAngle = 10;
		mover.WallBounce = 0.55f;
		mover.TryMove( Time.Delta );
		lastTrace = mover.TraceResult;

		if ( mover.HitWall || mover.HitFloor )
		{
			this.StartTouch( this );

		}
		mover.TryUnstuck();
		prevTickPos = Position;
		if ( setpos )
		{
			Position = mover.Position;
			Velocity = mover.Velocity;
		}

	}

	int sleepytime = 0;
	[Event.Tick.Server]
	public void Think()
	{
		if ( HLCombat.GibCount < 0 )
		{
			HLCombat.GibCount = 0;
		}
		if ( HLCombat.GibFadingCount < 0 )
		{
			HLCombat.GibFadingCount = 0;
		}
		if ( HLCombat.GibFadingCount > HLCombat.GibCount )
		{
			HLCombat.GibFadingCount = 0;
		}

		if ( ( ( HLCombat.GibCount - HLCombat.GibFadingCount ) > HLCombat.max_gibs ) )
		{
			toDeletefromMax = true;
		}
		else
		{
			if ( !hasCountedFading )
				toDeletefromMax = false;

		}

		if ( toDelete || toDeletefromMax )
		{
			alpha = alpha.LerpTo( 0, ( ( ( HLCombat.GibCount - HLCombat.max_gibs ).Clamp( 0, HLCombat.max_gibs ) * 0.1f ) ).Clamp( 1, 100 ) * Time.Delta );
			RenderColor = RenderColor.WithAlpha( alpha );
			if ( alpha.AlmostEqual( 0, 0.2f ) )
			{
				HLCombat.GibCount -= 1;
				HLCombat.GibFadingCount -= 1;
				Delete();
			}
		}

		if ( hasCountedFading == false && alpha < 1.0f )
		{
			HLCombat.GibFadingCount += 1;
			hasCountedFading = true;
		}
		else if ( hasCountedFading && alpha >= 1 )
		{
			HLCombat.GibFadingCount -= 1;
			hasCountedFading = false;
		}

		if ( !HLGame.hl_classic_gibs )
		{
			Move( false ); // hacky workaround for starttouch not working idk why
			return;
		}

		if ( sleepytime > 20 )
		{
			return;
		}
		LifeTime += 1;
		if ( RotAngles != SleepAngles )
			RotAngles += AngularVelocity * Time.Delta;
		Rotation = RotAngles.ToRotation();
		if ( ( Position == prevTickPos ) || ( Velocity.WithZ( 0 ).IsNearlyZero( 6 ) && Position.AlmostEqual( prevTickPos, 1f ) && GroundEntity != null && ( GroundEntity is not HLPlayer ) ) )
		{
			sleepytime += 1;
			if ( AlternateLandingRotation )
			{
				RotAngles = AltSleepAngles; // new Angles(0, Rand.Float(0, 360), 0);
			}
			else
			{
				RotAngles = SleepAngles;
			}
			// Clear rotation if not moving (even if on a conveyor)
			//AngularVelocity = Angles.Zero;
			if ( Velocity != Vector3.Zero )
				Velocity.LerpTo( Vector3.Zero, 0.1f * Time.Delta );

			//Move();


			return;
		}
		/*if ( true )
		{
			//DebugOverlay.Box( Position + TraceOffset, mins, maxs, Color.Red );
			//DebugOverlay.Box( Position, mins, maxs, Color.Blue );

			var lineOffset = 0;
			if ( Host.IsServer ) lineOffset = 10;
			DebugOverlay.ScreenText( $"      PrevPosition: {prevTickPos}", lineOffset + 0 );
			DebugOverlay.ScreenText( $"          Position: {Position}", lineOffset + 1 );
			DebugOverlay.ScreenText( $"          Velocity: {Velocity}", lineOffset + 2 );
			DebugOverlay.ScreenText( $"   AngularVelocity: {AngularVelocity}", lineOffset + 3 );
			DebugOverlay.ScreenText( $"      BaseVelocity: {BaseVelocity}", lineOffset + 4 );
			DebugOverlay.ScreenText( $"      GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]", lineOffset + 5 );
			DebugOverlay.ScreenText( $" SleepGroundEntity: {SleepGroundEntity} [{GroundEntity?.Velocity}]", lineOffset + 6 );
			//DebugOverlay.ScreenText( $" SurfaceFriction: {SurfaceFriction}", lineOffset + 6 );
			//DebugOverlay.ScreenText( $"    WishVelocity: {WishVelocity}", lineOffset + 7 );
		}*/
		if ( LifeTime > 300 && !toDelete )
		{
			toDelete = true;
		}
		CalcGroundEnt();
		Velocity -= new Vector3( 0, 0, gGravity * 0.5f ) * Time.Delta;
		Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;
		BaseVelocity = BaseVelocity.WithZ( 0 );




		//player->m_Local.m_flFallVelocity = 0.0f;
		bool bStartOnGround = GroundEntity != null;
		if ( bStartOnGround )
		{
			//AngularVelocity * 100f * Time.Delta;
			//if ( Velocity.z < FallSoundZ ) bDropSound = true;

			//Velocity = Velocity.WithZ( 0 );
			//player->m_Local.m_flFallVelocity = 0.0f;

			if ( GroundEntity != null )
			{
				ApplyFriction( gGroundFriction * gSurfaceFriction );
			}
		}
		Move();
	}


	public virtual void CalcGroundEnt()
	{
		mins = new Vector3( -bGirth, -bGirth, 0 );
		maxs = new Vector3( +bGirth, +bGirth, bHeight );
		gSurfaceFriction = 1.0f;
		var point = Position - Vector3.Up * 2;
		var vBumpOrigin = Position;
		//if ( GroundEntity != null ) // and not underwater
		//{
		//bMoveToEndPos = true;
		//point.z -= 18;
		//}


		var pm = TraceBBox( vBumpOrigin, point, mins, maxs, 4.0f );

		if ( pm.Entity == null || Vector3.GetAngle( Vector3.Up, pm.Normal ) > gGroundAngle )
		{
			ClearGroundEntity();
			if ( Velocity.z > 0 )
			{
				gSurfaceFriction = 0.25f;
			}
		}
		else
		{
			UpdateGroundEntity( pm );
		}

	}
	public Vector3 TraceOffset;

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
					.Ignore( this )
					.Run();

		tr.EndPosition -= TraceOffset;
		return tr;
	}
	/// <summary>
	/// We have a new ground entity
	/// </summary>
	public virtual void UpdateGroundEntity( TraceResult tr )
	{
		var GroundNormal = tr.Normal;

		// VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
		// A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
		// This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
		gSurfaceFriction = tr.Surface.Friction * 1.25f;
		if ( gSurfaceFriction > 1 ) gSurfaceFriction = 1;

		//if ( tr.Entity == GroundEntity ) return;

		Vector3 oldGroundVelocity = default;
		if ( GroundEntity != null ) oldGroundVelocity = GroundEntity.Velocity;

		bool wasOffGround = GroundEntity == null;

		GroundEntity = tr.Entity;
		if ( tr.Entity.IsValid && tr.Entity != null )
			SleepGroundEntity = tr.Entity;



		if ( GroundEntity != null )
		{
			BaseVelocity = GroundEntity.Velocity;
		}
		if ( wasOffGround )
		{

			//this.StartTouch( this );
		}

	}

	/// <summary>
	/// We're no longer on the ground, remove it
	/// </summary>
	public virtual void ClearGroundEntity()
	{

		if ( GroundEntity == null ) return;
		this.EndTouch( this );
		GroundEntity = null;
		var GroundNormal = Vector3.Up;
		gSurfaceFriction = 1.0f;
	}
	public virtual void ApplyFriction( float frictionAmount = 1.0f )
	{
		// If we are in water jump cycle, don't apply friction
		//if ( player->m_flWaterJumpTime )
		//   return;

		// Not on ground - no friction


		// Calculate speed
		var speed = Velocity.Length;
		if ( speed < 0.1f ) return;

		// Bleed off some speed, but if we have less than the bleed
		//  threshold, bleed the threshold amount.
		float control = ( speed < gStopSpeed ) ? gStopSpeed : speed;

		// Add the amount to the drop amount.
		var drop = control * Time.Delta * frictionAmount;

		// scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			Velocity *= newspeed;
		}

		// mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
	}
	public override void Touch( Entity other )
	{
		StartTouch( other ); // shitty
		base.Touch( other );
	}
	public override void StartTouch( Entity other )
	{
		//AngularVelocity = Angles.Zero;
		base.StartTouch( other );

		if ( ( this.PhysicsBody != null && ( this.PhysicsBody.GetDominantSurface() == "hl_flesh" || this.PhysicsBody.GetDominantSurface() == "flesh" || this.PhysicsBody.GetDominantSurface() == "flesh_yellow" || this.PhysicsBody.GetDominantSurface() == "hl_flesh_yellow" ) ) )
		{
			if ( BloodColour == NPC.BLOOD_COLOUR_RED )
			{
				if ( ResourceLibrary.TryGet<DecalDefinition>( "decals/red_blood.decal", out var decal ) )
				{
					var vecSpot = Position + new Vector3( 0, 0, 8 );
					Decal.Place( decal, lastTrace );
				}
			}
			else
			{
				if ( ResourceLibrary.TryGet<DecalDefinition>( "decals/yellow_blood.decal", out var decal ) )
				{

					var vecSpot = Position + new Vector3( 0, 0, 8 );
					Decal.Place( decal, lastTrace );
				}
			}
		}

		if ( BounceSound )
		{
			if ( SurfaceType != null )
			{
				SurfaceType.GetBounceSound( Position, 0.3f );

			}

		}
		//Log.Info( "boing!" );
		//if (Velocity.IsNearlyZero())
		//RotAngles = new Angles( 270, Rand.Float( 0, 360 ), 90 );
		// set angle

		// set anglular velocity

		//bounce?

	}

	public override void EndTouch( Entity other )
	{
		//Log.Info( "bye" );
		base.EndTouch( other );

		//RotAngles = (Vector3.Random * 10).EulerAngles;
		//AngularVelocity = new Vector3( Rand.Float( -100, 100 ), 0, Rand.Float( -100, 100 ) ).EulerAngles;
	}
}
/*
[Event.Tick.Server]
public void Think()
{

	this.Predictable = true;
	//Log.Info( "sim" );
	if ( this.GroundEntity != null )
		FL_ONGROUND = false;
	//Velocity -= new Vector3( 0, 0, 800 * 0.5f ) * Time.Delta;
	//Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;
	//BaseVelocity = BaseVelocity.WithZ( 0 );
	UpdateBaseVelocity();
	PhysicsToss();

}

void PhysicsAddGravityMove( Vector3 move )
{
	Vector3 vecAbsVelocity = this.Velocity;

	move.x = (vecAbsVelocity.x + this.BaseVelocity.x) * Time.Delta;
	move.y = (vecAbsVelocity.y + this.BaseVelocity.y) * Time.Delta;

	if ( this.GroundEntity != null )
	{
		move.z = BaseVelocity.z * Time.Delta;
		return;
	}

	// linear acceleration due to gravity
	float newZVelocity = vecAbsVelocity.z - gravity * Time.Delta;

	move.z = ((vecAbsVelocity.z + newZVelocity) / 2.0f + BaseVelocity.z) * Time.Delta;

	Vector3 vecBaseVelocity = BaseVelocity;
	vecBaseVelocity.z = 0.0f;
	BaseVelocity = vecBaseVelocity;

	vecAbsVelocity.z = newZVelocity;
	Velocity = vecAbsVelocity;

	// Bound velocity
	PhysicsCheckVelocity();
}

void UpdateBaseVelocity()
{
	if ( FL_ONGROUND )
	{
		if ( GroundEntity != null )
		{
			// On conveyor belt that's moving?
			//if ( groundentity->GetFlags() & FL_CONVEYOR )
			//{
				//Vector vecNewBaseVelocity;
				//groundentity->GetGroundVelocityToApply( vecNewBaseVelocity );
				//if ( GetFlags() & FL_BASEVELOCITY )
				//{
					//vecNewBaseVelocity += GetBaseVelocity();
				//}
				//AddFlag( FL_BASEVELOCITY );
				//SetBaseVelocity( vecNewBaseVelocity );
			//}
		}
	}
}

void PhysicsCheckVelocity()
{
	Vector3 origin = this.Position;
	Vector3 vecAbsVelocity = Velocity;

	bool bReset = false;
	for ( int i = 0; i < 3; i++ )
	{
		if ( ( vecAbsVelocity[i] ) == float.NaN )
		{
			Log.Warning( $"Got a NaN velocity on {this.ClassName}" );
			vecAbsVelocity[i] = 0;
			bReset = true;
		}
		if ( ( origin[i] ) == float.NaN )
		{
			Log.Warning( $"Got a NaN origin on {this.ClassName}" );
			origin[i] = 0;
			bReset = true;
		}

		if ( vecAbsVelocity[i] > maxVelocity )
		{

			vecAbsVelocity[i] = maxVelocity;
			bReset = true;
		}
		else if ( vecAbsVelocity[i] < -maxVelocity )
		{
			vecAbsVelocity[i] = -maxVelocity;
			bReset = true;
		}
	}

	if ( bReset )
	{
		this.Position = origin;
		this.Velocity = vecAbsVelocity;
	}
}

void PhysicsToss()
{

	Trace tTrace = Trace.Ray( Position, Position );
	TraceResult trace = tTrace.Run();
	Vector3 move = Vector3.Zero;

	//PhysicsCheckWater();

	// regular thinking
	//if ( !PhysicsRunThink() )
		//return;

	// Moving upward, off the ground, or  resting on a client/monster, remove FL_ONGROUND
	if ( Velocity[2] > 0 || !(GroundEntity == null) || false)//!(GroundEntity.IsWorld) )
	{
		GroundEntity = null;
	}

	// Check to see if entity is on the ground at rest
	if ( FL_ONGROUND )
	{
		if ( Velocity == Position )
		{
			// Clear rotation if not moving (even if on a conveyor)
			AngularVelocity = this.Rotation.Angles();
			if ( Velocity == Position )
				return;
		}
	}

	PhysicsCheckVelocity();

	// add gravity
	if ( FL_FLY )
	{
		PhysicsAddGravityMove( move );
	}

	else
	{
		// Base velocity is not properly accounted for since this entity will move again after the bounce without
		// taking it into account
		Vector3 vecAbsVelocity = Velocity;
		vecAbsVelocity += BaseVelocity;
		//VectorScale( vecAbsVelocity., Time.Delta, move );
		PhysicsCheckVelocity();
	}

	// move angles
	//SimulateAngles( Time.Delta );

	// move origin
	PhysicsPushEntity( move, trace );

//#if !defined( CLIENT_DLL )
//		if ( VPhysicsGetObject() )
//		{
//			VPhysicsGetObject()->UpdateShadow( GetAbsOrigin(), vec3_angle, true, gpGlobals->frametime );
//		}
//#endif

	PhysicsCheckVelocity();

	if ( trace.StartedSolid )
	{
		// entity is trapped in another solid
		// UNDONE: does this entity needs to be removed?
		Velocity = Position;
		AngularVelocity = Rotation.Angles();
		return;
	}

//#if !defined( CLIENT_DLL )
//		if ( IsEdictFree() )
//			return;
//#endif

	if ( trace.Fraction != 1.0f )
	{
		//PerformFlyCollisionResolution( trace, move );
	}

	// check for in water
	//PhysicsCheckWaterTransition();
}

void PhysicsPushEntity( Vector3 push, TraceResult pTrace )
{
	//VPROF("CBaseEntity::PhysicsPushEntity");

	if ( this.Parent != null )
	{
		Log.Warning( $"pushing entity {this.ClassName} that has parent {this.Parent.ClassName}");
		//Assert(0);
	}

	// NOTE: absorigin and origin must be equal because there is no moveparent
	Vector3 prevOrigin = this.Position;

	//PhysicsCheckSweep( this, prevOrigin, push, pTrace );

	if ( pTrace.Fraction != null )
	{
		Position = pTrace.EndPosition;

		// FIXME(ywb):  Should we try to enable this here
		// WakeRestingObjects();
	}

	// Passing in the previous abs origin here will cause the relinker
	// to test the swept ray from previous to current location for trigger intersections
	//PhysicsTouchTriggers( &prevOrigin );

	//if ( pTrace->m_pEnt )
	//{
	//	PhysicsImpact( pTrace->m_pEnt, *pTrace );
//	}
}
*/

