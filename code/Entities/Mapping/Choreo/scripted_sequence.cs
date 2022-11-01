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

	[Property( "delay" )]
	public float DelayLegacy { get; set; } = 0;

	[Property( "killtarget" ), FGDType( "target_destination" )]
	public string KillTargetLegacy { get; set; } = "";
	public Entity KillTargetLegacyEnt;

	[Property( "m_iszEntity" ), FGDType( "target_destination" )]
	public string TargetEntity { get; set; }

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
		TargetNPC = FindByName( TargetEntity ) as NPC;
	}

	protected Output OnEndSequence { get; set; }
	protected Output OnBeginSequence { get; set; }
	public void EndSequence()
	{
		TargetNPC.InScriptedSequence = false; 
		OnEndSequence.Fire( this );
	}

	/// <summary>
	/// Summons an NPC to act out the scripted sequence.
	/// </summary>
	[Input]
	public void BeginSequence()
	{
		TargetNPC = FindByName( TargetEntity ) as NPC;
		MoveTo( MoveMode );
		TargetNPC.InScriptedSequence = true; 
		TargetNPC.NPCTaskQueue.Enqueue( new PlayAnimTask( ActionAnimation, this ) );
	}

	/// <summary>
	/// Stops the scripted sequence. If fired after a sequence starts, this input will not take effect until the NPC finishes playing the scripted action animation.
	/// </summary>
	[Input]
	void CancelSequence()
	{
		TargetNPC.InScriptedSequence = false;

	}

	/// <summary>
	/// Summons an NPC to the script location. They will play their scripted idle (or "ACT_IDLE" if none is specified) until "BeginSequence" is triggered.
	/// </summary>
	[Input]
	void MoveToPosition()
	{
		TargetNPC = FindByName( TargetEntity ) as NPC;
		MoveTo( MoveMode );
	}
	public override void Spawn()
	{ 

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
}
