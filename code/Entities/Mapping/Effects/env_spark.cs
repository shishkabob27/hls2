﻿[Library( "env_spark" )]
[HammerEntity]
[Title( "env_spark" ), Category( "Legacy" ), Icon( "volume_up" )]
public partial class env_spark : Entity
{


	[Property( "MaxDelay" )]
	public float MaxDelay { get; set; } = 1;

	bool Active = false;
	TimeSince LastSpark;
	float NextSpark;
	// stub
	[Input]
	void SparkOnce()
	{
		Particles.Create( "particles/spark.vpcf", Position );
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
