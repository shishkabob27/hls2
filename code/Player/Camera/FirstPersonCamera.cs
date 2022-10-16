public partial class FirstPersonCamera : CameraMode
{

	public override void Activated()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Position = pawn.EyePosition;
		Rotation = pawn.EyeRotation;
	}

	public override void Update()
	{
		var pawn = Local.Pawn as HLPlayer;
		if ( pawn == null ) return;
		Viewer = pawn;
		if ( pawn.Client.IsUsingVr ) return;
		Vector3 simorg = pawn.EyePosition;
		var eyePos = pawn.EyePosition;

		Position = eyePos;
		Rotation = pawn.EyeRotation;

		// View Bob
		AddViewBob();

		// View Roll 
		AddRoll();

		// Punch Angles
		AddPunch();

		// Viewmodels and weapons
		DoViewmodelSetup();
		 
		// Smooth out stairs
		StairSmooth( simorg );

		// cl_vsmoothing, was originally trying this to smooth out elevators but I think I did something wrong, it's kinda shit.
		VSmoothing( simorg );
		
		// env_shake and other shaky things
		V_CalcShake();

	}
	
	Vector3 VectorMA( Vector3 va, float scale, Vector3 vb )
	{
		Vector3 vc = Vector3.Zero;
		vc[0] = va[0] + scale * vb[0];
		vc[1] = va[1] + scale * vb[1];
		vc[2] = va[2] + scale * vb[2];
		return vc;
	}

}
