public partial class FirstPersonCamera : CameraMode
{
	public override void Update()
	{
		var pawn = Game.LocalPawn as HLPlayer;
		if ( pawn == null ) return;
		if ( pawn.Client.IsUsingVr ) return;

		Vector3 simorg = pawn.EyePosition;

		Position = pawn.EyePosition;
		Rotation = pawn.ViewAngles.ToRotation();

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
		// Is there already function for this? I can't remember what this is called for the life of me.
		return va + scale * vb;
	}

}
