[Library("func_rotating")]
[HammerEntity]
[Title("func_rotating"), Category("Brush Entities"), Icon("volume_up")]
public partial class func_rotating : KeyframeEntity
{
	[Flags]
	public enum Flags
	{
		StartON = 1,
		ReverseDirection = 2,
		XAxis = 4,
		YAxis = 8,
		AccDcc = 16,
		FanPain = 32,
		NotSolid = 64,
		SmallSoundRadius = 128,
		MediumSoundRadius = 256,
		LargeSoundRadius = 512,
		//StartUnbreakable = 524288,
	}

	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags SpawnSettings { get; set; } = 0;

	[Property( "maxspeed" )]
	public float MaxSpeed { get; set; } = 100;
	public float Speed { get; set; } = 0;
	// stub
	[GameEvent.Tick.Server]
	void Tick()
	{
		var a = Rotation.From( 0, 1, 0 );
		if (SpawnSettings.HasFlag(Flags.XAxis)) a = Rotation.From( 0, 0, 1 );
		if (SpawnSettings.HasFlag(Flags.YAxis)) a = Rotation.From( 1, 0, 0 );
		Rotation *= (a * Time.Delta) * Speed; 
	}

	[Input]
	public void Start()
	{
		Speed = MaxSpeed;
	}
	[Input]
	public void Stop()
	{
		Speed = 0;
	}
	public override void Spawn()
	{
		base.Spawn();
		if (SpawnSettings.HasFlag(Flags.StartON))
		{
			Start();
		}
	}
}
