[Library( "scripted_sequence" )]
[HammerEntity]
[EditorModel( "models/editor/scripted_sequence.vmdl" )]
[Title( "scripted_sequence" ), Category( "Choreo" ), Icon( "theater_comedy" )]
public partial class scripted_sequence : Entity
{
	[Flags]
	public enum Flags
	{
		Repeatable = 1,
		LeaveCorpse = 2,
		StartonSpawn = 4,
		NoInterruptions = 8,
		OverrideAI = 16,
		DontTeleportNPCatEnd = 128,
		LoopinPostIdle = 256,
		PriorityScript = 512,
		SearchCyclically = 1024,
		NoComplaints = 2048,
		AllowNPCDeath = 4096,
		//StartUnbreakable = 524288,
	}

	public enum MoveToMode
	{
		No,
		Walk,
		Run,
		Custom_movement,
		Instantaneous,
		No_turn_to_face
	}

	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags SpawnSettings { get; set; } = Flags.Repeatable;

	/// <summary>
	/// The target entity. legacy goldsrc stuff.
	/// </summary>
	[Property( "target" ), FGDType( "target_destination" )]
	public string TargetLegacy { get; set; } = "";
	public Entity TargetLegacyEnt;

	[Property( "m_iszNextScript" ), FGDType( "target_destination" )]
	public string NextScript { get; set; } = "";
	public scripted_sequence NextScriptEnt;

	[Property( "delay" )]
	public float DelayLegacy { get; set; } = 0;

	[Property( "m_flRadius" )]
	public float SearchRadius { get; set; } = 0;

	[Property( "killtarget" ), FGDType( "target_destination" )]
	public string KillTargetLegacy { get; set; } = "";
	public Entity KillTargetLegacyEnt;

	[Property( "m_iszEntity" ), FGDType( "target_destination" )]
	public string TargetEntity { get; set; }

	[Property( "m_iszEntry" )]
	public string EntryAnimation { get; set; } = "null";
	
	[Property( "m_iszPlay" )]
	public string ActionAnimation { get; set; } = "null";

	[Property( "m_iszPostIdle" )]
	public string PostActionAnimation { get; set; } = "null";

	[Property( "m_iszIdle" )]
	public string PreActionAnimation { get; set; } = "null";

	[Property( "m_bLoopActionSequence" )]
	public bool LoopActionAnimation { get; set; } = false;

	[Property( "m_fMoveTo" )]
	public MoveToMode MoveMode { get; set; }

	public NPC TargetNPC;
	public scripted_sequence()
	{
		EnsureTargetNPC();
	}

	protected Output OnEndSequence { get; set; }
	protected Output OnBeginSequence { get; set; }
	public void EndSequence()
	{
		TargetNPC.InScriptedSequence = false;
		OnEndSequence.Fire( this );
		if ( PostActionAnimation != null && PostActionAnimation != "null" )
		{
			TargetNPC.DirectPlayback.Play( PostActionAnimation );
			if ( SpawnSettings.HasFlag( Flags.LoopinPostIdle ) )
			{
				// Absolutely fucking flood the queue, TODO: Proper looping 
				for ( var i = 0; i < 50; i++ )
				{
					TargetNPC.NPCTaskQueue.Enqueue( new PlayAnimTask( ActionAnimation, this ) );
				}
			}
		}

		if ( NextScript != null && NextScript != "" )
		{ 
			NextScriptEnt = FindByName( NextScript ) as scripted_sequence;
			NextScriptEnt.TargetNPC = TargetNPC;
			NextScriptEnt.TargetEntity = TargetEntity;
			NextScriptEnt.BeginSequence();
		}
		if (!SpawnSettings.HasFlag(Flags.Repeatable))
		{
			Delete();
		}
	}

	/// <summary>
	/// Summons an NPC to act out the scripted sequence.
	/// </summary>
	[Input]
	public void BeginSequence()
	{
		if ( !EnsureTargetNPC() ) return;
		OnBeginSequence.Fire( this );
		MoveTo( MoveMode );
		TargetNPC.InScriptedSequence = true;

		if ( EntryAnimation != null && EntryAnimation != "null" && EntryAnimation != "")
		{
			TargetNPC.NPCTaskQueue.Enqueue( new PlayAnimTask( EntryAnimation ) );
		}
		TargetNPC.NPCTaskQueue.Enqueue( new PlayAnimTask( ActionAnimation, this ) );

		if ( LoopActionAnimation )
		{
			// Absolutely fucking flood the queue, TODO: Proper looping 
			for ( var i = 0; i < 50; i++ )
			{
				TargetNPC.NPCTaskQueue.Enqueue( new PlayAnimTask( ActionAnimation, this ) );
			}
		}
	}

	/// <summary>
	/// Stops the scripted sequence. If fired after a sequence starts, this input will not take effect until the NPC finishes playing the scripted action animation.
	/// </summary>
	[Input]
	void CancelSequence()
	{
		if ( !EnsureTargetNPC() ) return;
		TargetNPC.InScriptedSequence = false;
		TargetNPC.NPCTaskQueue.Clear();
	}

	/// <summary>
	/// Summons an NPC to the script location. They will play their scripted idle (or "ACT_IDLE" if none is specified) until "BeginSequence" is triggered.
	/// </summary>
	[Input]
	void MoveToPosition()
	{
		MoveTo( MoveMode );
		if ( PreActionAnimation != null && PreActionAnimation != "null" )
		{
			TargetNPC.DirectPlayback.Play( PreActionAnimation );
		}
	}
	public override void Spawn()
	{ 
		EnsureTargetNPC();   
	} 
	[Event.Tick.Server]
	void Tick()
	{ 
		EnsureTargetNPC();
	}
	bool EnsureTargetNPC()
	{
		// Do we need to check for an npc? do we have one already?
		if ( TargetNPC == null )
		{
			// Do we check in a radius?
			if ( SearchRadius != 0 )
			{
				// Check in that radius.
				var a = FindInSphere( Position, SearchRadius ).OfType<NPC>();
				if ( a.Where( x => x.Name == TargetEntity ).Count() > 0 && a.Where( x => x.Name == TargetEntity).First() is NPC newTarget )
				{
					TargetNPC = newTarget;
				}
				else
				{
					// Didn't find one, exit.
					return false;
				}
			}
			else
			{
				// Check everything
				TargetNPC = FindByName( TargetEntity ) as NPC;
			}
			if ( TargetNPC != null )
			{
				// Should we start when we find our NPC?
				if ( SpawnSettings.HasFlag( Flags.StartonSpawn ) )
				{
					MoveToPosition();
				}
				// Found one, return happily.
				return true;
			}
			else
			{
				DebugWarn( "Scripted sequence with full search cannot find its NPC?" );
				// Didn't find one, exit.
				return false;
			}
		}
		else
		{
			// We already have one, we're good.
			return true;
		}
	}

	// TODO: Script Events.
	protected Output OnScriptEvent01 { get; set; }
	protected Output OnScriptEvent02 { get; set; }
	protected Output OnScriptEvent03 { get; set; }
	protected Output OnScriptEvent04 { get; set; }
	protected Output OnScriptEvent05 { get; set; }
	protected Output OnScriptEvent06 { get; set; }
	protected Output OnScriptEvent07 { get; set; }
	protected Output OnScriptEvent08 { get; set; }

	[ConVar.Replicated]
	public static bool npc_script_debug {get; set; } = false;
	void DebugPrint(string toprint)
	{
		if (npc_script_debug)
		{
			Log.Info( $"[Scripted Sequence] {toprint}" );
		}
	}
	void DebugWarn(string toprint)
	{
		if (npc_script_debug)
		{
			Log.Warning( $"[Scripted Sequence] {toprint}" );
		}
	}

	public void MoveTo( MoveToMode moveMode )
	{
		EnsureTargetNPC();
		DebugPrint( "Move to position started." );
		switch ( moveMode )
		{
			case MoveToMode.No:
				break;
			case MoveToMode.Walk:
				WalkTo();
				break;
			case MoveToMode.Run:
				WalkTo( true );
				break;
			case MoveToMode.Custom_movement:
				WalkTo();
				break;
			case MoveToMode.Instantaneous:
				TargetNPC.Position = Position;
				TargetNPC.targetRotation = Rotation;
				TargetNPC.targetRotationOVERRIDE = Rotation;
				break;
			case MoveToMode.No_turn_to_face:
				TargetNPC.targetRotation = Rotation;
				TargetNPC.targetRotationOVERRIDE = Rotation;
				break;
		}
	}
	void WalkTo( bool running = false )
	{
		EnsureTargetNPC();
		DebugPrint( "Walking to position." ); 
		TargetNPC.NPCTaskQueue.Enqueue( new MoveToTask( Position, running ) );
		TargetNPC.NPCTaskQueue.Enqueue( new RotateToTask( Rotation ) );
		//TargetNPC.Steer.Target = Position;
		//TargetNPC.Speed = running ? TargetNPC.RunSpeed : TargetNPC.WalkSpeed;
	}
}
