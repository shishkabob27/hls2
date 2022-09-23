namespace XeNPC;
using XeNPC.Debug;

[Library( "monster_test" ), HammerEntity] // THIS WILL NOT BE AN NPC BUT A BASE THAT EVERY NPC SHOULD DERIVE FROM!!! THIS IS HERE FOR TESTING PURPOSES ONLY!
public partial class NPC : AnimatedEntity, IUse, ICombat
{
	public bool InScriptedSequence = false;
	public bool InPriorityScriptedSequence = false;
	public bool DontSleep = false;
	public bool NoNav = false;
	public float GroundBounce = 0;
	public bool HasFriction = true;

	[Flags]
	public enum Flags
	{
		WaitTillSeen = 1,
		Gag = 2,
		MonsterClip = 4,
		NoWreckage = 8,
		Prisoner = 16,
		StartInactive = 64,
		WaitForScript = 128,
		PreDisaster = 256,
		FadeCorpse = 512,
		NotInDeathmatch = 2048
	}


	[Property( "spawnsetting", Title = "Spawn Settings" )]
	public Flags SpawnSettings { get; set; }

	[ConVar.Replicated]
	public static bool nav_drawpath { get; set; }

	[ConCmd.Server( "npc_clear" )]
	public static void NpcClear()
	{
		foreach ( var npc in Entity.All.OfType<NPC>().ToArray() )
			npc.Delete();
	}

	public float Speed;
	public float WalkSpeed = 80;
	public float RunSpeed = 200;
	public float entFOV = 0.5f;

	public string NPCAnimGraph = "";
	public string NPCSurface = "surface/hlflesh.surface";
	XeNPC.NavPath Path;
	public NavSteer Steer;

	public Entity TargetEntity;
	public int TargetEntityRel = 0;
	public virtual int Classify()
	{
		return (int)HLCombat.Class.CLASS_NONE;
	}
	public override void Spawn()
	{
		if ( SpawnSettings.HasFlag( Flags.NotInDeathmatch ) && HLGame.hl_gamemode == "deathmatch" && IsServer )
		{
			Delete();
		}

		Tags.Add( "npc", "playerclip" );
		base.Spawn();
		animHelper = new HLAnimationHelper( this );
		if ( !NoNav )
		{

			Path = new XeNPC.NavPath();

			Steer = new NavSteer();

		}
		SetModel( "models/citizen/citizen.vmdl" );
		EyePosition = Position + Vector3.Up * 64;
		if ( PhysicsBody == null ) SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) );

		EnableHitboxes = true;
		if ( NPCSurface != null )
		{
			PhysicsBody.SetSurface( NPCSurface );
		}
		Speed = 50;
	}

	public XeNPC.Debug.Draw Draw => XeNPC.Debug.Draw.Once;

	Vector3 InputVelocity;

	Vector3 LookDir;
	public Rotation targetRotation;

	public Nullable<Rotation> targetRotationOVERRIDE;
	public Sound CurrentSound;
	public HLAnimationHelper animHelper;
	float neck = 0.0f;
	float neck2 = 0.0f;
	[Event.Tick.Server]
	public void Tick()
	{
		if ( NoNav )
		{
			See();
			Think();
			SoundProcess();
			return;
		}

		if ( HLUtils.PlayerInRangeOf( Position, 1024 ) == false && DontSleep == false )
			return;
		using var _a = Profile.Scope( "NpcTest::Tick" );




		InputVelocity = 0;

		if ( Steer != null && !NoNav )
		{
			using var _b = Profile.Scope( "Steer" );

			Steer.Tick( Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, Speed );
			}

			if ( nav_drawpath )
			{
				Steer.DebugDrawPath();
			}
		}

		using ( Profile.Scope( "Move" ) )
		{
			Move( Time.Delta );
		}

		var walkVelocity = Velocity.WithZ( 0 );
		var turnSpeed = 0.32f;
		if ( walkVelocity.Length > 0.5f )
		{
			turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
			targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
		}
		if ( targetRotationOVERRIDE != null )
		{
			targetRotation = (Rotation)targetRotationOVERRIDE;
			Rotation = Rotation.Lerp( Rotation, (Rotation)targetRotationOVERRIDE, turnSpeed * Time.Delta * 20.0f );
			if ( Angles.AngleVector( Rotation.Angles() ).AlmostEqual( Angles.AngleVector( targetRotation.Angles() ) ) )
			{
				targetRotationOVERRIDE = null;
			}
		}
		else
		{
			if ( LifeState == LifeState.Alive )
				Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}

		//var animHelper = new HLAnimationHelper(this);

		LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		animHelper.WithLookAt( EyePosition + LookDir );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( InputVelocity );
		animHelper.HealthLevel = Health;

		//animHelper.VoiceLevel = CurrentSound.Index / 100;


		//Log.Info();//SoundFile.Load("sounds/hl1/scientist/alright.wav"));
		if ( CurrentSound.Finished != true )
		{
			animHelper.VoiceLevel = Rand.Float();
			// It's not possible to get the sound volume for animating the mouths so i'll just randomise a float for now,.
		}
		else
		{
			animHelper.VoiceLevel = 0;
		}
		if ( LifeState == LifeState.Dead )
			return;
		if ( animHelper.VoiceLevel != 0 )
		{
			var closestENTs = Entity.All.OfType<ICombat>().OfType<Entity>().ToList();
			closestENTs.Remove( this );
			var ck = closestENTs.OrderBy( o => ( o.Position.Distance( Position ) ) );
			var closestENT = ck.First();

			var a = Rotation.LookAt( closestENT.Position.WithZ( 0 ) - Position.WithZ( 0 ), Vector3.Up ).Yaw();//HLUtils.VecToYaw(closestENT.Position.WithZ(0) - Position.WithZ(0));

			//Log.Info(closestENT.Position - Position);
			var c = Rotation.Yaw();
			var d = a - c;
			//if (a > 0) d = a - c;
			//if (a < 0) d = a + c;
			var b = ( d / 90 ) * -1;

			if ( b > 1 ) b -= 2;
			if ( b < -1 ) b += 2;

			if ( b > 1 ) b -= 2;
			if ( b < -1 ) b += 2;

			if ( b > 1 ) b -= 2;
			if ( b < -1 ) b += 2;

			if ( b > 1 ) b -= 2;
			if ( b < -1 ) b += 2;

			neck2 = b;

		}
		else
		{
			neck2 = 0;
		}
		neck = neck.LerpTo( neck2, 12 * Time.Delta );
		animHelper.Neck = neck;
		//See();
		Think();
		SoundProcess();
		//SoundStream test;
		//test.

		See();

	}

	public virtual void FindCover( Vector3 fromPos )
	{
		var MyNode = NavMesh.GetClosestPoint( Position );
		var ThreatNode = NavMesh.GetClosestPoint( fromPos );
		Steer.Target = NavMesh.GetPointWithinRadius( fromPos, 500, 600 ) ?? NavMesh.GetPointWithinRadius( fromPos, 300, 600 ) ?? NavMesh.GetPointWithinRadius( fromPos, 100, 600 ) ?? NavMesh.GetPointWithinRadius( fromPos, 10, 600 ) ?? fromPos;
		/*
        foreach (var area in NavMesh.GetNavAreas())
        {
            foreach (var node in area.)
            {

            }
        }
		*/
	}

	public virtual void See()
	{
		TargetEntity = null;
		TargetEntityRel = 0;
		// todo, trace a cone maybe...

		/*
        var a = Trace.Ray(EyePosition, EyePosition + Rotation.Forward * 2000)
            .Radius(70f)
            .EntitiesOnly()
            .Ignore(this)
            .Run();
        if (a.Entity == null)
        {
            TargetEntity = null;
            return;
        }
		var b = Trace.Ray(EyePosition, (a.Entity as ModelEntity).WorldSpaceBounds.Center)
            .Ignore(this)
            .Run();
		if (b.Entity != a.Entity)
		{
			TargetEntity = null;
			return;
		}
		TargetEntity = a.Entity;
		*/

		var allents = Entity.All.OfType<ICombat>().ToList().OrderBy( o => ( ( o as Entity ).Position.Distance( Position ) ) );

		int i = 0;
		foreach ( Entity ent in allents )
		{
			i++;
			if ( i > 16 ) continue;
			if ( !InViewCone( ent ) )
			{
				continue;
			}
			var b = Trace.Ray( EyePosition, ent.Position )
				.Ignore( this )
				.Run();
			if ( b.Entity != ent )
			{
				continue;
			}


			ProcessEntity( ent, GetRelationship( ent ) );
		}
	}
	public virtual void ProcessEntity( Entity ent, int rel )
	{
	}
	public int GetRelationship( Entity ent )
	{
		if ( ent is ICombat )
		{
			var trgt = ( ent as ICombat );
			return HLCombat.ClassMatrix[Classify(), trgt.Classify()];
		}
		else
		{
			return 0;
		}
	}
	public bool InViewCone( Entity ent )
	{
		Vector2 vec2LOS;
		float flDot;


		var e = ( ent.WorldSpaceBounds.Center - WorldSpaceBounds.Center );
		vec2LOS = new Vector2( e.x, e.y );
		vec2LOS = vec2LOS.Normal;

		flDot = (float)Vector2.Dot( vec2LOS, new Vector2( Rotation.Forward.x, Rotation.Forward.y ) );

		if ( flDot > entFOV )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public virtual void Think()
	{

	}

	public virtual void SoundProcess()
	{

	}

	public virtual bool OnUse( Entity user )
	{
		if ( LifeState == LifeState.Dead )
			return false;
		return true;
	}

	protected virtual void Move( float timeDelta )
	{
		if ( LifeState == LifeState.Dead )
			return;
		var bbox = CollisionBounds;
		//DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Green );

		MoveHelper move = new( Position, Velocity );
		move.GroundBounce = GroundBounce;
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox );

		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			//	Sandbox.Debug.Draw.Once
			//						.WithColor( Color.Red )
			//						.IgnoreDepth()
			//						.Arrow( Position, Position + Velocity * 2, Vector3.Up, 2.0f );

			using ( Profile.Scope( "TryUnstuck" ) )
				move.TryUnstuck();

			using ( Profile.Scope( "TryMoveWithStep" ) )
				move.TryMoveWithStep( timeDelta, 30 );
		}

		using ( Profile.Scope( "Ground Checks" ) )
		{
			var tr = move.TraceDirection( Vector3.Down * 10.0f );

			if ( move.IsFloor( tr ) )
			{
				GroundEntity = tr.Entity;

				if ( !tr.StartedSolid )
				{
					//move.Position = tr.EndPosition;
				}

				if ( InputVelocity.Length > 0 )
				{
					var movement = move.Velocity.Dot( InputVelocity.Normal );
					move.Velocity = move.Velocity - movement * InputVelocity.Normal;
					if ( HasFriction ) move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
					move.Velocity += movement * InputVelocity.Normal;

				}
				else
				{
					if ( HasFriction ) move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
				}
			}
			else
			{
				GroundEntity = null;
				move.Velocity += Vector3.Down * 800 * timeDelta;
				//XeNPC.Debug.Draw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
			}
		}

		Position = move.Position;
		Velocity = move.Velocity;
	}


	DamageInfo LastDamage;

	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Alive )
			targetRotation = Rotation.From( ( ( Position - info.Position ) * -360 ).EulerAngles.WithRoll( 0 ).WithPitch( 0 ) );
		var trace = Trace.Ray( EyePosition, EyePosition + ( ( Position - info.Position ) * 70 ) * 2 )
			.WorldOnly()
			.Ignore( this )
			.Size( 1.0f )
			.Run();
		if ( ResourceLibrary.TryGet<DecalDefinition>( "decals/red_blood.decal", out var decal ) )
		{
			//Log.Info( "Splat!" );
			Decal.Place( decal, trace );
		}

		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;
		if ( IsServer )
		{
			Health -= info.Damage;
			if ( Health <= 0f )
			{
				if ( LifeState == LifeState.Alive )
				{
					OnKilled();
					LifeState = LifeState.Dead;
					//Delete();
				}
			}
		}
		LastDamage = info;

		if ( Health <= 0f && LifeState == LifeState.Alive )
		{
			LifeState = LifeState.Dead;
			//Delete();
		}
		if ( Health < -20 )
		{
			HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, info.Position, Health, this.CollisionBounds );
			Delete();
		}
		this.ProceduralHitReaction( info );
		//
		// Add a score to the killer
		//
		if ( LifeState == LifeState.Dead && info.Attacker != null )
		{
			if ( info.Attacker.Client != null && info.Attacker != this )
			{
				info.Attacker.Client.AddInt( "kills" );
			}
		}

	}

	public void SpeakSound( string sound, float pitch = 100 )
	{
		if ( IsServer )
		{
			using ( Prediction.Off() )
			{
				CurrentSound.Stop();
				CurrentSound = PlaySound( sound ).SetPitch( HLUtils.CorrectPitch( pitch ) );
			}
			//SpeakSoundcl(sound, pitch);
		}

	}

	[ClientRpc]
	public void SpeakSoundcl( string sound, float pitch = 100 )
	{
		//CurrentSound.Stop();
		CurrentSound = PlaySound( sound ).SetPitch( HLUtils.CorrectPitch( pitch ) );
		Log.Info( $"IsClient: {IsClient} IsServer: {IsServer}" );
	}

	public override void OnKilled()
	{
		Tags.Add( "debris" );

		SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 1, 8 ) );
		if ( LifeState == LifeState.Alive )
		{
			LifeState = LifeState.Dead;
			//Delete();
		}

		if ( LastDamage.Flags.HasFlag( DamageFlags.Blast ) )
		{

		}
		else
		{
			//BecomeRagdollOnClient(Velocity, LastDamage.Flags, LastDamage.Position, LastDamage.Force, GetHitboxBone(LastDamage.HitboxIndex));
		}
	}

	bool IUse.OnUse( Entity user )
	{
		return OnUse( user );
	}

	bool IUse.IsUsable( Entity user )
	{
		return true;
	}

	public IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		bool underWater = Trace.TestPoint( start, "water" );

		var trace = Trace.Ray( start, end )
				.UseHitboxes()
				.WithAnyTags( "solid", "player", "npc", "glass" )
				.Ignore( this )
				.Size( radius );

		//
		// If we're not underwater then we can hit water
		//
		if ( !underWater )
			trace = trace.WithAnyTags( "water" );

		var tr = trace.Run();

		if ( tr.Hit )
			yield return tr;

		//
		// Another trace, bullet going through thin material, penetrating water surface?
		//
	}

	public new void SetAnimGraph( string name )
	{
		if ( IsServer )
		{
			base.SetAnimGraph( name );
		}
	}
	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		if ( stringData == "ragdoll" && IsServer && HLGame.hl_ragdoll )
		{
			Ragdoll();
		}
		base.OnAnimEventGeneric( name, intData, floatData, vectorData, stringData );
	}

	[Input]
	public void Ragdoll()
	{

		var ent = new ModelEntity();
		ent.SetModel( this.Model.Name );
		ent.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.UsePhysicsCollision = true;
		if ( NPCSurface != null && ent.PhysicsBody != null )
		{
			ent.PhysicsBody.SetSurface( NPCSurface );
		}

		ent.CopyFrom( this );
		ent.CopyBonesFrom( this );
		ent.SetRagdollVelocityFrom( this );
		this.Delete();
	}

	[Input]
	public void Gib()
	{
		HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, Position, Health, this.CollisionBounds );
		this.Delete();
	}

	[Input]
	public void Break()
	{
		Gib();
	}

	[Input]
	public void Kill()
	{
		Health = -1;
		if ( LifeState == LifeState.Alive )
		{
			OnKilled();
			LifeState = LifeState.Dead;
			//Delete();
		}
	}
}
