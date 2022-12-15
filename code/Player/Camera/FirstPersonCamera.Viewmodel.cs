partial class FirstPersonCamera
{
	void DoViewmodelSetup()
	{
		if ( Game.LocalPawn is not HLPlayer pawn ) return;
		// Handle Viewmodel Setup, this was in Viewmodel.cs but I moved it to match HL1 and because we want access to the cameras view bobbing
		if ( pawn.ActiveChild is Weapon )
		{

			var wep = pawn.ActiveChild as Weapon;
			// Weapon position
			if ( wep.ViewModelEntity is Entity )
			{

				wep.ViewModelEntity.Position = Position;
				wep.ViewModelEntity.Rotation = Rotation;
				Camera.Main.SetViewModelCamera( Camera.FieldOfView, 4 );
			}
			AddViewmodelBob( wep.ViewModelEntity );
		}
	}
}
