﻿partial class HLViewModel : BaseViewModel
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
		//Rotation = Local.Pawn.EyeRotation;

		if ( Local.Pawn.LifeState == LifeState.Dead )
			return;

		if (HLGame.CurrentState == HLGame.GameStates.GameEnd )
			return;


		//
		// Bob up and down based on our walk movement
		//
		var speed = Owner.Velocity.Length.LerpInverse( 0, 800 );
		var left = camSetup.Rotation.Left;
		var forward = camSetup.Rotation.Forward;

		if ( Owner.GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		Position += forward * MathF.Sin( walkBob ) * speed * -1;
		//Position += left * MathF.Sin( walkBob * 0.5f ) * speed * -0.5f;

		//var uitx = new Sandbox.UI.PanelTransform();
		//uitx.AddTranslateY( MathF.Sin( walkBob * 1.0f ) * speed * -4.0f );
		//uitx.AddTranslateX( MathF.Sin( walkBob * 0.5f ) * speed * -3.0f );

		//HudRootPanel.Current.Style.Transform = uitx;
	}
}
