[Library( "env_shake" )]
[HammerEntity]
[Title( "env_shake" ), Category( "Legacy" ), Icon( "volume_up" )]
public partial class env_shake : Entity
{
	[Net]
	public bool Active { get; set; } = false;
	// stub
	[Input]
	void StartShake()
	{
		Active = true;
	}
	[Input]
	void StopShake()
	{
		Active = false;
	}

	[Event.Frame]
	void Tick()
	{
		if ( !Active ) return;
		if ( Local.Pawn is not HLPlayer ply ) return;
		if ( ply.CameraMode is not FirstPersonCamera plycam ) return;
		plycam.ShakeOffset = new Vector3( Rand.Float( 1 ), Rand.Float( 1 ), Rand.Float( 1 ) );
		Log.Info( "shakeyshakey!" );
	}
}
