[Library( "hl_357" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/python.vmdl" )]
[Title( ".357 Magnum Revolver" ), Category( "Weapons" )]
partial class Python : HLWeapon
{
	public static readonly Model WorldModel = Model.Load("models/hl1/weapons/world/python.vmdl");
	public override string ViewModelPath => "models/hl1/weapons/view/v_python.vmdl";

	public override float PrimaryRate => 1.5f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 4.0f;
	public override int ClipSize => 6;
	public override AmmoType AmmoType => AmmoType.Python;

	public override int Bucket => 1;
	public override int BucketWeight => 200;

	[Net, Predicted]
	public bool Zoomed { get; set; }

	//private float? LastFov;
	//private float? LastViewmodelFov;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 6;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.PrimaryAttack );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();

			if ( AvailableAmmo() > 0 )
			{
				Reload();
			}
			return;
		}

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "357_shot" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.01f, 1.5f, 40.0f, 2.0f );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		//Zoomed = Input.Down( InputButton.SecondaryAttack );
	}

	/*public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		float targetFov = camSetup.FieldOfView;
		float targetViewmodelFov = camSetup.ViewModel.FieldOfView;
		LastFov = LastFov ?? camSetup.FieldOfView;
		LastViewmodelFov = LastViewmodelFov ?? camSetup.ViewModel.FieldOfView;

		if ( Zoomed )
		{
			targetFov = 40.0f;
			targetViewmodelFov = 40.0f;
		}

		float lerpedFov = LastFov.Value.LerpTo( targetFov, Time.Delta * 24.0f );
		float lerpedViewmodelFov = LastViewmodelFov.Value.LerpTo( targetViewmodelFov, Time.Delta * 24.0f );

		camSetup.FieldOfView = lerpedFov;
		camSetup.ViewModel.FieldOfView = lerpedViewmodelFov;

		LastFov = lerpedFov;
		LastViewmodelFov = lerpedViewmodelFov;
	}*/

	public override void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var draw = Render.Draw2D;

		var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.3f, 0.0f ) );
		var color = Color.Lerp( Color.Red, Color.Yellow, lastReload.LerpInverse( 0.0f, 0.4f ) );

		draw.BlendMode = BlendMode.Lighten;
		draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

		var length = 3.0f + shootEase * 5.0f;

		draw.Ring( center, length, length - 3.0f );
	}

}
