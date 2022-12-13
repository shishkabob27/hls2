partial class HLViewModel : BaseViewModel
{

	public override void PlaceViewmodel()
	{
		// nothing
	}

	public void UpdateCamera()
	{
		var rotationDistance = Rotation.Distance( Camera.Rotation );

		Position = Camera.Position;
		Rotation = Rotation.Lerp( Rotation, Camera.Rotation, RealTime.Delta * rotationDistance * 1.1f );

		Camera.ZNear = 4;

		if ( Game.LocalPawn.LifeState == LifeState.Dead )
			return;

		if ( HLGame.CurrentState == HLGame.GameStates.GameEnd )
			return;
	}
}
