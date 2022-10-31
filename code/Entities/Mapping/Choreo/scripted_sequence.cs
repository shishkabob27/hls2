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

	public enum moveToMode
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
	public moveToMode MoveToMode { get; set; }

	public NPC TargetNPC;
	public scripted_sequence()
	{
		TargetNPC = FindByName( TargetEntity ) as NPC;
	}

	protected Output OnEndSequence { get; set; }
	public void EndSequence()
	{

	}
	protected Output OnBeginSequence { get; set; }

	[Input]
	public void BeginSequence()
	{
	
	}
	[Input]
	void CancelSequence()
	{

	}
	[Input]
	void MoveToPosition()
	{

	}
	public override void Spawn()
	{


	}

}
