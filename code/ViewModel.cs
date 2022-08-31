partial class HLViewModel : BaseViewModel
{
	float walkBob = 0;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		// camSetup.ViewModelFieldOfView = camSetup.FieldOfView + (FieldOfView - 80);

		AddCameraEffects( ref camSetup );
	}

	private void AddCameraEffects( ref CameraSetup camSetup )
	{
		if ( Local.Pawn.LifeState == LifeState.Dead )
			return;

		//
		// Bob up and down based on our walk movement
		//
		var speed = Owner.Velocity.Length.LerpInverse( 0, 800 );
		var forward = camSetup.Rotation.Forward;

		if ( Owner.GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		Position += forward * MathF.Sin( walkBob ) * speed * -1;
	}
}
