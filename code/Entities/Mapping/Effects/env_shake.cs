[Library( "env_shake" )]
[HammerEntity]
[EditorSprite( "editor/env_shake.vmat" )]
[Title( "env_shake" ), Category( "Effects" ), Icon( "volume_up" )]
public partial class env_shake : Entity
{
	[Net]
	public bool Active { get; set; } = false;


	[Property( "amplitude" )]
	public float Amplitude { get; set; } = 4;


	[Property( "duration" )]
	public float Duration { get; set; } = 1;


	[Property( "frequency" )]
	public float Frequency { get; set; } = 2.5f;


	[Property( "radius" )]
	public float EffectRadius { get; set; } = 500f;

	[Input]
	void StartShake()
	{
		var a = Game.Clients.Where( ply => ply.Pawn.Position.Distance( Position ) < EffectRadius * 2 );
		ShakeRPC( To.Multiple( a ) );

	}

	[Input]
	void StopShake()
	{
		StopShakeRPC();
	}

	[ClientRpc]
	void ShakeRPC()
	{

		if ( Game.LocalPawn is not HLPlayer ply ) return;
		if ( ply.CameraMode is not FirstPersonCamera plycam ) return;

		plycam.Shake_AMPLITUDE = Math.Max( Amplitude, plycam.Shake_AMPLITUDE ); // avoid setting this lower if there is a stronger shake already active
		plycam.Shake_DURATION = Math.Max( Duration, plycam.Shake_DURATION ); // avoid setting this lower if there is a stronger shake already active
		plycam.Shake_FREQUENCY = Math.Max( Frequency, plycam.Shake_FREQUENCY ); // avoid setting this lower if there is a stronger shake already active
		plycam.Shake_ENDTIME = Time.Now + Math.Max( plycam.Shake_DURATION, 0.01f );
	}
	[ClientRpc]
	void StopShakeRPC()
	{

		if ( Game.LocalPawn is not HLPlayer ply ) return;
		if ( ply.CameraMode is not FirstPersonCamera plycam ) return;

		plycam.Shake_NEXTSHAKE = 0;
		plycam.Shake_AMPLITUDE = 0;
		plycam.Shake_DURATION = 0;
		plycam.Shake_FREQUENCY = 0;
		plycam.Shake_ENDTIME = 0;
	}
}
