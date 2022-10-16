partial class FirstPersonCamera
{
	void DoViewmodelSetup()
	{
		if ( Local.Pawn is not HLPlayer pawn ) return;
		// Handle Viewmodel Setup, this was in Viewmodel.cs but I moved it to match HL1 and because we want access to the cameras view bobbing
		if ( pawn.ActiveChild is HLWeapon )
		{

			var wep = pawn.ActiveChild as HLWeapon;
			// Weapon position
			if ( wep.ViewModelEntity is Entity )
			{

				wep.ViewModelEntity.Position = Position;
				wep.ViewModelEntity.Rotation = Rotation;
			}
			AddViewmodelBob(wep.ViewModelEntity);
		}
	}
}
