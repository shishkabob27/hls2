partial class HLViewModel : BaseViewModel
{

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		AddCameraEffects( ref camSetup );
	}

	private void AddCameraEffects( ref CameraSetup camSetup )
	{
		if ( Game.LocalPawn.LifeState == LifeState.Dead )
			return;

		camSetup.ViewModel.ZNear = 4;

		if ( HLGame.CurrentState == HLGame.GameStates.GameEnd )
			return;
	}
}
