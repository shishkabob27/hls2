[Library( "env_spark" )]
[HammerEntity]
[EditorSprite( "editor/env_spark.vmat" )]
[Title( "env_spark" ), Category( "Legacy" ), Icon( "volume_up" )]
public partial class env_spark : Entity
{
	[Flags]
	public enum Flags
	{
		StartON = 64,
		Glow = 128,
		Silent = 256,
		Directional = 512,
	}

	/// <summary>
	/// Settings that are only applicable when the entity spawns
	/// </summary>
	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags spawnflags { get; set; } = Flags.StartON;

	[Property( "MaxDelay" )]
	public float MaxDelay { get; set; } = 1;

	bool Active = false;
	TimeSince LastSpark;
	float NextSpark;

	public override void Spawn()
	{
		base.Spawn();
		if ( MaxDelay == 0 ) MaxDelay = 1;
		if ( spawnflags.HasFlag( Flags.StartON ) )
		{
			StartSpark();
		}
	}

	// stub
	[Input]
	void SparkOnce()
	{
		Particles.Create( "particles/spark.vpcf", Position );
		if ( spawnflags.HasFlag( Flags.Silent ) == false )
		{
			Sound.FromWorld( "spark", Position );
		}
		LastSpark = 0;
		OnSpark.Fire( this );
	}

	[Input]
	void StartSpark()
	{
		Active = true;
	}
	[Input]
	void StopSpark()
	{
		Active = false;
	}
	[Input]
	void ToggleSpark()
	{
		Active = !Active;
	}

	[Event.Tick.Server]
	void Tick()
	{
		if ( !Active ) return;
		if ( LastSpark > MaxDelay || LastSpark > NextSpark )
		{
			SparkOnce();
			NextSpark = Rand.Float( MaxDelay );
		}

	}

	protected Output OnSpark { get; set; }

}
