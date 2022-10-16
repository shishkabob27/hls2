partial class FirstPersonCamera
{
	void AddPunch()
	{
		if ( Local.Pawn is not HLPlayer pawn ) return;
		// Apply punchangles
		Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + pawn.punchangle.z ).ToRotation();
		Rotation = Rotation.Angles().WithPitch( Rotation.Angles().pitch + pawn.punchangle.x ).ToRotation();
		Rotation = Rotation.Angles().WithYaw( Rotation.Angles().yaw + pawn.punchangle.y ).ToRotation();

		// Apply client side punchangles
		Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + pawn.punchanglecl.z ).ToRotation();
		Rotation = Rotation.Angles().WithPitch( Rotation.Angles().pitch + pawn.punchanglecl.x ).ToRotation();
		Rotation = Rotation.Angles().WithYaw( Rotation.Angles().yaw + pawn.punchanglecl.y ).ToRotation();

		// Drop client side punchangles (server side is handled in Player.cs, should move maybe?)
		pawn.punchanglecl = pawn.punchanglecl.Approach( 0, Time.Delta * 14.3f ); // was Delta * 10, 14.3 matches hl1 the most
																				 //Log.Info( pawn.punchangle );
	}
}
