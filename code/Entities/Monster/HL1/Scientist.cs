[Library( "monster_scientist" ), HammerEntity]
[EditorModel( "models/hl1/monster/scientist/scientist_01.vmdl" )]
[Title( "Scientist" ), Category( "Monsters" ), Icon( "person" ), MenuCategory( "Black Mesa" )]
public partial class Scientist : NPC
{
	Entity FollowTarget;

	List<string> ScientistMDLList = new List<string>{
		"models/hl1/monster/scientist/scientist_01.vmdl",
		"models/hl1/monster/scientist/scientist_02.vmdl",
		"models/hl1/monster/scientist/scientist_03.vmdl",
		"models/hl1/monster/scientist/scientist_04.vmdl",
	};

	[Property]
	public float Body { get; set; } = -1;
	[Net]
	public float VoicePitch { get; set; } = 100;


	public Scientist()
	{
		NPCAnimGraph = "animgraphs/hl1/monster/scientist.vanmgrph";
		Health = 20;
		Speed = 60;
		WalkSpeed = 60;
		RunSpeed = 200;
		VoicePitch = SetPitch();
	}
	public override int Classify()
	{
		return (int)HLCombat.Class.CLASS_HUMAN_PASSIVE;
	}
	public override void Spawn()
	{
		NPCAnimGraph = "animgraphs/hl1/monster/scientist.vanmgrph";
		base.Spawn();
		EnableTouch = true;
		if ( Body > 3 )
		{
			Body = Game.Random.Int( 0, 3 );
		}
		SetModel( SetScientistModel() );

		SetAnimGraph( NPCAnimGraph );
		Health = 20;
		Speed = 70;
		VoicePitch = SetPitch();

		SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
		EnableHitboxes = true;

		Tags.Add( "npc", "playerclip" );

	}

	public string SetScientistModel()
	{
		switch ( Body )
		{
			case 0: return "models/hl1/monster/scientist/scientist_01.vmdl";
			case 1: return "models/hl1/monster/scientist/scientist_02.vmdl";
			case 2: return "models/hl1/monster/scientist/scientist_03.vmdl";
			case 3: return "models/hl1/monster/scientist/scientist_04.vmdl";
			default: return Game.Random.FromList<string>( ScientistMDLList );
		}
	}

	public int SetPitch()
	{
		switch ( Body )
		{
			case 0: return 105;  // glasses
			case 1: return 100;  // einstein
			case 2: return 95;   // luther
			case 3: return 100;  // slick
			default: return 100;
		}
	}

	string MODE = "MODE_IDLE";
	bool wasInBound = false;
	public override void Think()
	{
		if ( InScriptedSequence )
			return;
		//VoicePitch = SetPitch();
		var ply = HLUtils.ClosestPlayerTo( Position );

		if ( MODE == "MODE_FOLLOW" )
		{
			if ( FollowTarget != null && FollowTarget.IsValid && FollowTarget.Position.Distance( Position ) > 80 )
			{
				Steer.Target = FollowTarget?.Position ?? Vector3.Zero;
				wasInBound = true;
			}
			else if ( FollowTarget != null && FollowTarget.IsValid && wasInBound == true )
			{
				Steer.Target = Position;
				wasInBound = false;
			}

			if ( FollowTarget != null && FollowTarget.IsValid && FollowTarget.Position.Distance( Position ) < 230 )
			{
				Speed = WalkSpeed;
			}
			else if ( FollowTarget != null && FollowTarget.IsValid && FollowTarget.Position.Distance( Position ) > 256 )
			{
				Speed = RunSpeed;
			}
			else
			{
				Speed = WalkSpeed;
			}
		}

		// we've been pushed!
		if ( ply != null && ply.IsValid && ply.Position.Distance( Position ) < 40 && !InScriptedSequence  && Steer.Output.Finished)
		{
			Steer.Target = Position + (ply.Position - Position).Normal * -78;
			Speed = WalkSpeed;
		}

	}
	public override void Touch( Entity other )
	{
		base.Touch( other );
		if ( other is DoorEntity && Game.IsServer )
		{
			(other as DoorEntity).Open();
		}
	}

	int ticker = 1;

	static SoundEvent soundevent = new SoundEvent( "sounds/hl1/scientist/alright.vsnd" );
	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		if ( LifeState == LifeState.Alive )
		{
			SpeakSound( "sounds/hl1/scientist/sci_pain.sound", VoicePitch );
		}
		animHelper.IsScared = true;
		//CurrentSound.Stop();
		//CurrentSound.Stop();
		//CurrentSound = PlaySound("sounds/hl1/scientist/sci_pain.sound").SetPitch(VoicePitch / 100);

	}

	public override bool OnUse( Entity user )
	{
		if ( !(base.OnUse( user )) )
		{
			return false;
		}
		if ( ticker == 1 )
		{
			ticker = 0;

			//Steer.Target = Position + (user.Position - Position).Normal * 10; // Turn to face the user.
			//targetRotation.w = Rotation.w;
			//this.LookDir = 
			//lookAt = user.Position.WithZ(Position.z);




			//if ( ResourceLibrary.TryGet<SoundEvent>("sounds/hl1/scientist/sci_follow.sound", out var soundevent) ) //PlaySound("sci_follow");
			//{


			//}

			if ( spawnflags.HasFlag( Flags.PreDisaster ) )
			{
				SpeakSound( "sounds/hl1/scientist/sci_poke.sound", VoicePitch );
			}
			else
			{
				targetRotation = Rotation.LookAt( user.Position.WithZ( 0 ) - Position.WithZ( 0 ), Vector3.Up );
				if ( MODE == "MODE_IDLE" )
				{
					//CurrentSound.Stop();
					//CurrentSound = PlaySound("sounds/hl1/scientist/sci_follow.sound").SetPitch(VoicePitch / 100);
					SpeakSound( "sounds/hl1/scientist/sci_follow.sound", VoicePitch );
					MODE = "MODE_FOLLOW";
					DontSleep = true;
					FollowTarget = HLUtils.ClosestPlayerTo( Position );
				}
				else if ( MODE == "MODE_FOLLOW" )
				{
					//CurrentSound.Stop();
					//CurrentSound = PlaySound("sounds/hl1/scientist/sci_stopfollow.sound").SetPitch(VoicePitch / 100);
					SpeakSound( "sounds/hl1/scientist/sci_stopfollow.sound", VoicePitch );
					MODE = "MODE_IDLE";
					DontSleep = false;
					FollowTarget = null;
				}
			}

			return true;
		}
		else
		{
			ticker = 1;
			return false;
		}
	}
	TimeSince TimeSinceSeen = new TimeSince();
	public override void ProcessEntity( Entity ent, int rel )
	{
		if ( rel > 0 )
		{
			targetRotation = Rotation.LookAt( (ent.Position - Position).Normal.WithZ( 0 ), Vector3.Up );
			if ( TimeSinceSeen > 5 )
			{

				TimeSinceSeen = 0;
				Speed = RunSpeed;
				FindCover( ent.Position );
				SpeakSound( "sounds/hl1/scientist/sci_fear.sound", VoicePitch );
				animHelper.IsScared = true;
			}
		}
	}

}

[Library( "monster_sitting_scientist" ), HammerEntity]
[EditorModel( "models/dev/scientist_sitting.vmdl" )]
[Title( "Sitting Scientist" ), Category( "Monsters" ), Icon( "person" )]
public class ScientistSitting : NPC
{
	// Stub NPC, this does nothing yet

	List<string> ScientistMDLList = new List<string>{
		"models/hl1/monster/scientist/scientist_01.vmdl",
		"models/hl1/monster/scientist/scientist_02.vmdl",
		"models/hl1/monster/scientist/scientist_03.vmdl",
		"models/hl1/monster/scientist/scientist_04.vmdl",
	};

	List<string> SittingAnims = new List<string>{
		"sitting2",
		"sitting3",
	};

	[Property]
	public float Body { get; set; } = 5;

	public override void Spawn()
	{

		TraceResult beans = Trace.Ray( Position, Position - new Vector3( 0, 0, 500 ) ).Run();
		Position = beans.EndPosition + new Vector3( 0, 0, 19 );
		NoNav = true;

		SetupPhysicsFromOBB( PhysicsMotionType.Static, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );
		base.Spawn();
		//SetAnimGraph("animgraphs/hl1/monster/scientist.vanmgrph");
		Health = 20;
		if ( Body > 3 )
		{
			Body = Game.Random.Int( 0, 3 );
		}
		SetModel( SetScientistModel() );
		CurrentSequence.Name = Game.Random.FromList<string>( SittingAnims );
		//UseAnimGraph = false;
		//CollisionBounds.
		PhysicsEnabled = false;
		EnableHitboxes = true;
		//Position = Position - new Vector3(0, 0, 22);

		Tags.Add( "npc", "playerclip" );

	}

	public string SetScientistModel()
	{
		switch ( Body )
		{
			case 0: return "models/hl1/monster/scientist/scientist_01.vmdl";
			case 1: return "models/hl1/monster/scientist/scientist_02.vmdl";
			case 2: return "models/hl1/monster/scientist/scientist_03.vmdl";
			case 3: return "models/hl1/monster/scientist/scientist_04.vmdl";
			default: return Game.Random.FromList<string>( ScientistMDLList );
		}
	}

	float tick = 0;
	public override void Think()
	{
		tick += 0.01f;
		if ( CurrentSequence.IsFinished == true || CurrentSequence.TimeNormalized == 1.0f || tick > CurrentSequence.Duration / 2 )
		{
			tick = 0;
			CurrentSequence.Name = Game.Random.FromList<string>( SittingAnims );
		}
	}

}

[Library( "monster_scientist_dead" ), HammerEntity]
[EditorModel( "models/hl1/monster/scientist/scientist_01.vmdl" )]
[Title( "Dead Scientist" ), Category( "Monsters" ), Icon( "person" )]
public class ScientistDead : NPC
{

	List<string> ScientistMDLList = new List<string>{
		"models/hl1/monster/scientist/scientist_01.vmdl",
		"models/hl1/monster/scientist/scientist_02.vmdl",
		"models/hl1/monster/scientist/scientist_03.vmdl",
		"models/hl1/monster/scientist/scientist_04.vmdl",
	};

	List<string> SittingAnims = new List<string>{
		"sitting2",
		"sitting3",
	};

	[Property]
	public float Body { get; set; } = -1;

	[Property]
	public int pose { get; set; } = 0;

	public override void Spawn()
	{

		TraceResult beans = Trace.Ray( Position, Position - new Vector3( 0, 0, 500 ) ).Run();
		Position = beans.EndPosition + new Vector3( 0, 0, 19 );
		NoNav = true;

		SetupPhysicsFromOBB( PhysicsMotionType.Static, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );
		base.Spawn();

		Health = 20;
		if ( Body > 3 )
		{
			Body = Game.Random.Int( 0, 3 );
		}
		SetModel( SetScientistModel() );
		
		switch (pose)
		{
			case 0: CurrentSequence.Name = "lying_on_back"; break;
			case 1: CurrentSequence.Name = "lying_on_stomach"; break;
			case 2: CurrentSequence.Name = "dead_sitting"; break;
			case 3: CurrentSequence.Name = "lying_on_back"; break;
			case 4: CurrentSequence.Name = "dead_table1"; break;
			case 5: CurrentSequence.Name = "dead_table2"; break;
			case 6: CurrentSequence.Name = "dead_table3"; break;
			default: CurrentSequence.Name = "lying_on_back"; break;
		}

		UseAnimGraph = false;
		PhysicsEnabled = false;
		EnableHitboxes = true;

		Tags.Add( "npc", "playerclip" );

	}

	public string SetScientistModel()
	{
		switch ( Body )
		{
			case 0: return "models/hl1/monster/scientist/scientist_01.vmdl";
			case 1: return "models/hl1/monster/scientist/scientist_02.vmdl";
			case 2: return "models/hl1/monster/scientist/scientist_03.vmdl";
			case 3: return "models/hl1/monster/scientist/scientist_04.vmdl";
			default: return Game.Random.FromList<string>( ScientistMDLList );
		}
	}

	public override void Think()
	{
		
	}

}
