﻿[Library( "env_laser" )]
[HammerEntity]
[Title( "env_laser" ), Category( "Effects" ), Icon( "volume_up" )]
public partial class env_laser : Entity
{
	[Flags]
	public enum Flags
	{
		StartOn = 1,
		Toggle = 2,
		RandomStrike = 4,
		Ring = 8,
		StartSparks = 16,
		EndSparks = 32,
		DecalEnd = 64,
		ShadeStart = 128,
		ShadeEnd = 256,
		TaperOut = 512,
	}

	/// <summary>
	/// Settings that are only applicable when the entity spawns
	/// </summary>
	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags spawnflags { get; set; } = Flags.StartOn;



	[Property( "LightningStart" ), FGDType( "target_destination" )]
	public EntityTarget LightningStart { get; set; }
	[Property( "LightningEnd" ), FGDType( "target_destination" )]
	public EntityTarget LightningEnd { get; set; }
	[Property( "LaserTarget" ), FGDType( "target_destination" )]
	public EntityTarget LaserTarget { get; set; }
	[Property]
	public string texture { get; set; } = "";
	[Property]
	public float NoiseAmplitude { get; set; } = 0;
	[Property]
	public float BoltWidth { get; set; } = 2;
	[Property]
	Vector3 rendercolor { get; set; } = new Vector3( 1, 0, 0 );
	[Property]
	Vector3 targetpoint { get; set; }
	Particles Beam;
	// stub
	Entity StartEnt;
	Entity TargetEnt;
	public override void Spawn()
	{
		base.Spawn();
		if ( spawnflags.HasFlag( Flags.StartOn ) )
		{
			TurnOn();
		}

	}

	[Input]
	void TurnOn()
	{
		if ( true )//Beam == null)
		{
			Beam = Particles.Create( "particles/env_beam.vpcf", Position );
		}
		UpdateBeam();
		Beam.SetPosition( 2, rendercolor );
		Beam.SetPosition( 3, new Vector3( BoltWidth, 1, 0 ) );
		Beam.SetPosition( 4, new Vector3( NoiseAmplitude, 0, 0 ) );
	}
	[Input]
	void TurnOff()
	{
		Remove();
	}
	void Remove()
	{
		rpcremove();
		if ( Beam != null )
		{
			Beam.Destroy();
			Beam = null;
		}
	}
	[ClientRpc]
	void rpcremove()
	{
		if ( Beam != null )
		{
			Beam.Destroy();
			Beam = null;
		}
	}
	[Input]
	void Toggle()
	{
		if ( Beam == null )
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}
	[GameEvent.Tick.Server]
	void UpdateBeam()
	{
		if ( Beam != null )
		{
			if (StartEnt == null)
			{
				StartEnt = LightningStart.GetTarget();
			}
			if (TargetEnt == null)
			{
				TargetEnt = LaserTarget.GetTarget();
			}
			try
			{ 
				Beam.SetEntity( 0, StartEnt );
			}
			catch
			{

			}
			var a = Position;
			var b = targetpoint;
			if ( StartEnt != null ) a = StartEnt.Position;
			if ( TargetEnt != null ) b = TargetEnt.Position;
			Beam.SetPosition( 0, a );
			var tr = Trace.Ray( a, b ).Run();
			Beam.SetPosition( 1, tr.HitPosition );
		}
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();

		Remove();
	}
	public override void OnKilled()
	{
		base.OnKilled();
		Remove();
	}
}
