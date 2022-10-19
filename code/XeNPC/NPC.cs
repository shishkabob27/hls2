namespace XeNPC;

using Sandbox;
using System;
using XeNPC.Debug;

[Library( "monster_test" ), HammerEntity] // THIS WILL NOT BE AN NPC BUT A BASE THAT EVERY NPC SHOULD DERIVE FROM!!! THIS IS HERE FOR TESTING PURPOSES ONLY!
public partial class NPC : AnimatedEntity, IUse, ICombat
{
	public bool InScriptedSequence = false;
	public bool InPriorityScriptedSequence = false;
	public bool ScriptedSequenceOverrideAi = false;
	public bool DontSleep = false;
	public bool DontSee = false;
	public bool CannotBeSeen = false;
	public bool NoNav = false;
	public float GroundBounce = 0;
	public float WallBounce = 0;
	public Entity WallEntity;
	public bool HasFriction = true;
	public bool Unstick = true;
	public bool SticktoFloor = true;

	public const int BLOOD_COLOUR_RED = 0;
	public const int BLOOD_COLOUR_YELLOW = 1;
	public const int BLOOD_COLOUR_GREEN = BLOOD_COLOUR_YELLOW;
	public int BloodColour = BLOOD_COLOUR_RED;
	Color MyDebugColour = Color.Random;

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
	public Flags spawnflags { get; set; }

	[ConVar.Replicated]
	public static bool nav_drawpath { get; set; }

	[ConCmd.Server( "npc_clear" )]
	public static void NpcClear()
	{
		foreach ( var npc in Entity.All.OfType<NPC>().ToArray() )
			npc.Delete();
	}

	/// <summary>
	///  +-180 degrees
	/// </summary>
	const float VIEW_FIELD_FULL = -1.0f;
	/// <summary>
	/// +-135 degrees 0.1 // +-85 degrees, used for full FOV checks 
	/// </summary>
	const float VIEW_FIELD_WIDE = -0.7f;
	/// <summary>
	/// +-45 degrees, more narrow check used to set up ranged attacks
	/// </summary>
	const float VIEW_FIELD_NARROW = 0.7f;
	/// <summary>
	/// +-25 degrees, more narrow check used to set up ranged attacks
	/// </summary>
	const float VIEW_FIELD_ULTRA_NARROW = 0.9f;

	/// <summary>
	/// NPC's current movement speed.
	/// </summary>
	public float Speed;
	/// <summary>
	/// The speed of the NPC in the walking state.
	/// </summary>
	public float WalkSpeed = 80;
	/// <summary>
	/// The speed of the NPC in the running state.
	/// </summary>
	public float RunSpeed = 200;
	/// <summary>
	/// The NPCs Field of View as a dot product.
	/// </summary>
	public float entFOV = VIEW_FIELD_WIDE;
	/// <summary>
	/// The distance (in hammer units) the NPC can see.
	/// </summary>
	public float entDist = 512;
	/// <summary>
	/// The distance (in hammer units) the NPC will require a player to be within to be active.
	/// </summary>
	public float SleepDist = 1024;
	/// <summary>
	/// The height of the NPCs eyes, decides where they should see from.
	/// </summary>
	public float EyeHeight = 64;

	public string NPCAnimGraph = "";
	public string NPCSurface = "flesh";
	XeNPC.NavPath Path;
	public NavSteer Steer;

	/// <summary>
	/// Specifies an Entity to follow or target.
	/// </summary>
	public Entity TargetEntity;
	public int TargetEntityRel = 0;

	/// <summary>
	/// Classify an NPC in the relationship matrix.
	/// </summary>
	/// <returns></returns>
	public virtual int Classify()
	{
		return (int)HLCombat.Class.CLASS_NONE;
	}
	public override void Spawn()
	{
		if ( spawnflags.HasFlag( Flags.NotInDeathmatch ) && HLGame.GameIsMultiplayer() && IsServer )
		{
			Delete();
		}
		if ( spawnflags.HasFlag( Flags.WaitForScript ) )
		{
			InScriptedSequence = true;
			ScriptedSequenceOverrideAi = true;
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
		EyePosition = Position + Vector3.Up * EyeHeight;
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
	/// <summary>
	/// The rotation in which our NPC should look.
	/// </summary>
	public Rotation targetRotation;

	public Nullable<Rotation> targetRotationOVERRIDE;
	/// <summary>
	/// The NPCs current sound, we have one so talking NPCs can't say multiple things at once.
	/// </summary>
	public Sound CurrentSound;
	public HLAnimationHelper animHelper;
	float neck = 0.0f;
	float neck2 = 0.0f;
	[Event.Tick.Server]
	public void Tick()
	{
		if ( HLUtils.PlayerInRangeOf( Position, SleepDist ) == false && DontSleep == false )
			return;

		if ( NoNav )
		{
			See();
			Think();
			Hear();
			return;
		}

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
			var ck = closestENTs.OrderBy( o => (o.Position.Distance( Position )) );
			var closestENT = ck.First();

			var a = Rotation.LookAt( closestENT.Position.WithZ( 0 ) - Position.WithZ( 0 ), Vector3.Up ).Yaw();//HLUtils.VecToYaw(closestENT.Position.WithZ(0) - Position.WithZ(0));

			//Log.Info(closestENT.Position - Position);
			var c = Rotation.Yaw();
			var d = a - c;
			//if (a > 0) d = a - c;
			//if (a < 0) d = a + c;
			var b = (d / 90) * -1;

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
		Hear();
		//SoundStream test;
		//test.

		See();

	}

	/// <summary>
	/// Find some cover from an area and walk to it (unfinished but works)
	/// </summary>
	/// <param name="fromPos"></param>
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

	int posinarray = 0;
	Vector3 lastTraceStart = Vector3.Zero;
	Vector3 lastTraceEnd = Vector3.Zero;
	TraceResult lastTraceResult;
	Trace[] LastTraces = new Trace[32];
	TraceResult[] LastTracesResults = new TraceResult[32];

	TimeSince nextSee;
	float SeeDelay = 0.1f;

	/// <summary>
	/// green = visible
	/// red = not visible but still in viewcone
	/// yellow = cached trace, neither entities have moved so assume it's the same
	/// </summary>
	[ConVar.Replicated] public static bool npc_debug_los { get; set; } = false;

	/// <summary>
	/// Process what other NPCs/Players we can see and call ProcessEntity() for each
	/// </summary>
	public virtual void See()
	{
		if ( IsClient ) return;
		if ( DontSee ) return;
		if ( InScriptedSequence && ScriptedSequenceOverrideAi ) return;
		TargetEntity = null;
		TargetEntityRel = 0;
		if ( nextSee <= SeeDelay ) return;
		nextSee = 0;
		//return;
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
		Vector3 delta = new Vector3( entDist, entDist, entDist );
		var allents = Entity.FindInBox( new BBox( Position - delta, Position + delta ) ).OfType<ICombat>().OrderBy( o => ((o as Entity).Position.Distance( Position )) );
		/*
		allents.RemoveAll( o =>
			((o as Entity).Position.Distance( Position )) > entDist || // Remove everything further away than entDist
			o == this || // Remove ourselves
			!InViewCone( (o as Entity) ) || // Remove anything not in our view code.
			!EntityShouldBeSeen( (o as Entity) ) // Remove anything that doesn't want to be seen.
		); */
		//allents = allents; // sort by closest
		foreach ( Entity ent in allents.Take( 16 ) ) // iterate through the first 16 at MOST.
		{
			if ( ent == this ) continue;
			//if ( ent.Position.Distance( Position ) > entDist ) continue; // Ignore everything further away than entDist
			if ( !InViewCone( ent ) ) continue; // Ignore anything not in our view cone.
			if ( !EntityShouldBeSeen( ent ) ) continue; // Ignore anything that doesn't want to be seen.
			var REL = GetRelationship( ent );
			if ( REL == 0 ) continue; // Ignore anything that we don't care about.
			bool hasdrawn = false;
			var a = Trace.Ray( EyePosition, ent.EyePosition )
				.WithoutTags( "monster", "npc", "player" );

			TraceResult b;
			if ( LastTraces.Contains( a ) )
			{
				b = LastTracesResults[Array.IndexOf( LastTraces, a )];
				if ( npc_debug_los && !hasdrawn ) DebugOverlay.Line( b.StartPosition, ent.EyePosition, Color.Yellow, SeeDelay + Time.Delta, false );
			}
			else
			{
				if ( posinarray >= LastTraces.Length ) posinarray = 0;
				b = a.Run();
				//Array.Copy( LastTracesResults, 1, LastTracesResults, 0, LastTracesResults.Length - 1 );
				LastTracesResults[posinarray] = b;
				//Array.Copy( LastTraces, 1, LastTraces, 0, LastTraces.Length - 1 );
				LastTraces[posinarray] = a;
				posinarray++;
			}


			if ( b.Fraction != 1 )
			{
				if ( npc_debug_los && !hasdrawn ) DebugOverlay.Line( b.StartPosition, ent.EyePosition, Color.Red, SeeDelay + Time.Delta, false );
				continue;
			}
			if ( npc_debug_los && !hasdrawn ) DebugOverlay.Line( b.StartPosition, ent.EyePosition, Color.Green, SeeDelay + Time.Delta, false );
			ProcessEntity( ent, REL );
		}
	}

	/// <summary>
	/// Called for every NPC/Player we can see
	/// </summary>
	/// <param name="ent">The Entity we have seen</param>
	/// <param name="rel">Our relationship with said entity, refer to HLCombat.cs (TODO MOVE OUT OF HLS2 SPECIFIC CODE)</param>
	public virtual void ProcessEntity( Entity ent, int rel )
	{
	}
	/// <summary>
	/// Get our relationship with another NPC/Player
	/// </summary>
	/// <param name="ent"></param>
	/// <returns>The relationship, refer to HLCombat.cs (TODO MOVE OUT OF HLS2 SPECIFIC CODE)</returns>
	public int GetRelationship( Entity ent )
	{
		if ( ent is ICombat )
		{
			var trgt = (ent as ICombat);
			return HLCombat.ClassMatrix[Classify(), trgt.Classify()];
		}
		else
		{
			return 0;
		}
	}

	public bool EntityShouldBeSeen( Entity ent )
	{
		if ( ent is NPC enpc )
		{
			if ( enpc.CannotBeSeen ) return false;
		}
		return true;
	}

	[ConVar.Replicated] public static bool npc_draw_cone { get; set; } = false;
	/// <summary>
	/// Check if an Entity is in our view cone, This does NOT check if they're behind walls.
	/// </summary>
	/// <param name="ent">The Entity to check</param>
	/// <returns></returns>
	public bool InViewCone( Entity ent )
	{
		Vector2 vec2LOS;
		float flDot;


		var e = (ent.WorldSpaceBounds.Center - (Position + Vector3.Up * EyeHeight));
		vec2LOS = new Vector2( e.x, e.y );
		vec2LOS = vec2LOS.Normal;

		flDot = (float)Vector2.Dot( vec2LOS, new Vector2( Rotation.Forward.x, Rotation.Forward.y ) );

		if ( npc_draw_cone ) DrawViewCone();

		if ( flDot > entFOV )
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void DrawViewCone()
	{
		Vector3[] rots = {
			new Vector3( 0, 0, 1 ),
			new Vector3( 0, 1, 0 ),
		};
		Vector3 LastConePos1 = (((Position + Vector3.Up * EyeHeight) + Rotation.RotateAroundAxis( rots.Last(), (MathF.Acos( entFOV ) * (180 / MathF.PI)) ).Forward * 1000));
		Vector3 LastConePos2 = (((Position + Vector3.Up * EyeHeight) + Rotation.RotateAroundAxis( rots.Last(), (MathF.Acos( -entFOV ) * (180 / MathF.PI)) ).Forward * -1000));
		foreach ( Vector3 rot in rots )
		{
			Vector3 Pos1 = (((Position + Vector3.Up * EyeHeight) + Rotation.RotateAroundAxis( rot, (MathF.Acos( entFOV ) * (180 / MathF.PI)) ).Forward * -1000));
			Vector3 Pos2 = (((Position + Vector3.Up * EyeHeight) + Rotation.RotateAroundAxis( rot * -1, (MathF.Acos( entFOV ) * (180 / MathF.PI)) ).Forward * -1000));
			DebugOverlay.Line( (Position + Vector3.Up * EyeHeight), Pos1, MyDebugColour, SeeDelay + Time.Delta );
			DebugOverlay.Line( (Position + Vector3.Up * EyeHeight), Pos2, MyDebugColour, SeeDelay + Time.Delta );

			DebugOverlay.Line( Pos1, LastConePos1, MyDebugColour, SeeDelay + Time.Delta );
			DebugOverlay.Line( Pos2, LastConePos2, MyDebugColour, SeeDelay + Time.Delta );
			DebugOverlay.Line( Pos1, LastConePos2, MyDebugColour, SeeDelay + Time.Delta );
			DebugOverlay.Line( Pos2, LastConePos1, MyDebugColour, SeeDelay + Time.Delta );
			LastConePos1 = Pos1;
			LastConePos2 = Pos2;
		}
	}

	/// <summary>
	/// Put all NPC thinking logic in here
	/// </summary>
	public virtual void Think()
	{

	}
	/// <summary>
	/// Process all hearable sounds, TODO
	/// </summary>
	public virtual void Hear()
	{
		ProcessSound(); // TODO: all of this.
	}

	/// <summary>
	/// Called for every sound we can hear.
	/// </summary>
	public virtual void ProcessSound()
	{

	}

	/// <summary>
	/// Called when a player presses their use key on the NPC
	/// </summary>
	/// <param name="user">The player who pressed their USE key on us</param>
	/// <returns></returns>
	public virtual bool OnUse( Entity user )
	{
		if ( LifeState == LifeState.Dead )
			return false;
		return true;
	}

	/// <summary>
	/// All movement, todo use HLMovement maybe?
	/// </summary>
	/// <param name="timeDelta"></param>
	protected virtual void Move( float timeDelta )
	{
		if ( LifeState == LifeState.Dead )
			return;
		var bbox = CollisionBounds;
		//DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Green );

		MoveHelper move = new( Position, Velocity );
		move.GroundBounce = GroundBounce;
		move.WallBounce = WallBounce;
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox );


		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			//	Sandbox.Debug.Draw.Once
			//						.WithColor( Color.Red )
			//						.IgnoreDepth()
			//						.Arrow( Position, Position + Velocity * 2, Vector3.Up, 2.0f );

			using ( Profile.Scope( "TryUnstuck" ) )
				if ( Unstick ) move.TryUnstuck();

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
					if ( SticktoFloor ) move.Position = tr.EndPosition;
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
				if ( InScriptedSequence && ScriptedSequenceOverrideAi ) return;
				move.Velocity += Vector3.Down * 800 * timeDelta;
				//XeNPC.Debug.Draw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
			}

			var tr2 = move.TraceDirection( Vector3.Forward * 16.0f );
			if ( move.HitWall )
			{
				WallEntity = tr2.Entity;
			}
			else
			{
				WallEntity = null;
			}
		}

		Position = move.Position;
		Velocity = move.Velocity;
	}


	DamageInfo LastDamage;

	const int HITGROUP_GENERIC = 0;
	const int HITGROUP_HEAD = 1;
	const int HITGROUP_CHEST = 2;
	const int HITGROUP_STOMACH = 3;
	const int HITGROUP_LEFTARM = 4;
	const int HITGROUP_RIGHTARM = 5;
	const int HITGROUP_LEFTLEG = 6;
	const int HITGROUP_RIGHTLEG = 7;
	const int HITGROUP_GEAR = 10;
	const int HITGROUP_SPECIAL = 11;
	/// <summary>
	/// Called when we take damage
	/// </summary>
	/// <param name="info"></param>
	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Alive )
			targetRotation = Rotation.LookAt( (info.Position - Position).Normal.WithZ(0), Vector3.Up );
		var trace = Trace.Ray( EyePosition, EyePosition + ((Position - info.Position) * 70) * 2 )
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

		if ( info.Hitbox.HasTag( "generic" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "head" ) )
			info.Damage *= 3;
		if ( info.Hitbox.HasTag( "chest" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "stomach" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "arm" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "leg" ) )
			info.Damage *= 1;

		switch ( GetHitboxGroup( info.HitboxIndex ) )
		{
			case HITGROUP_GENERIC:
				break;
			case HITGROUP_HEAD:
				info.Damage *= 3;
				break;
			case HITGROUP_CHEST:
				info.Damage *= 1;
				break;
			case HITGROUP_STOMACH:
				info.Damage *= 1;
				break;
			case HITGROUP_LEFTARM:
			case HITGROUP_RIGHTARM:
				info.Damage *= 1;
				break;
			case HITGROUP_LEFTLEG:
			case HITGROUP_RIGHTLEG:
				info.Damage *= 1;
				break;
			default:
				break;
		}

		if ( IsServer )
		{
			Health -= info.Damage;
			if ( Health <= 0f )
			{
				if ( LifeState == LifeState.Alive )
				{
					OnKilled();
					LifeState = LifeState.Dead;
					if ( info.Flags.HasFlag( DamageFlags.AlwaysGib ) )
					{
						HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, info.Position, Health, this.CollisionBounds, BloodColour );
						Delete();
					}
					else if ( HLGame.hl_ragdoll )
					{
						Ragdoll( info.Force );
					}
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
		if ( Health < -20 && !info.Flags.HasFlag( DamageFlags.DoNotGib ) )
		{
			HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, info.Position, Health, this.CollisionBounds, BloodColour );
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
	/// <summary>
	/// Speak a sound out of the NPC's mouth, if they have one.
	/// </summary>
	/// <param name="sound"></param>
	/// <param name="pitch"></param>
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

	/// <summary>
	/// Trace a bullet.
	/// </summary>
	/// <param name="start"></param>
	/// <param name="end"></param>
	/// <param name="radius"></param>
	/// <returns></returns>
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
			Ragdoll(Vector3.Zero);
		}
		base.OnAnimEventGeneric( name, intData, floatData, vectorData, stringData );
	}
	/// <summary>
	/// Turn into a ragdoll
	/// </summary>
	[Input]
	public void Ragdoll()
	{
		Ragdoll(Vector3.Zero);
	}

	/// <summary>
	/// Turn into a ragdoll
	/// </summary>
	public void Ragdoll(Vector3 force)
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
		ent.PhysicsGroup.AddVelocity( force );
		this.Delete();
	}

	/// <summary>
	/// Turn into Gibs
	/// </summary>
	[Input]
	public void Gib()
	{
		HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, Position, Health, this.CollisionBounds, BloodColour );
		this.Delete();
	}

	/// <summary>
	/// Synonymous with Gib()
	/// </summary>
	[Input]
	public void Break()
	{
		Gib();
	}
	/// <summary>
	/// Die
	/// </summary>
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
