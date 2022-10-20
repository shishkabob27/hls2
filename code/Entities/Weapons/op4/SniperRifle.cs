[Library( "weapon_sniperrifle" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/w_m40a1.vmdl" )]
[Title( "Sniper Rifle" ), Category( "Weapons" )]
partial class SniperRifle : HLWeapon
{
	public override string ViewModelPath => "models/op4/weapons/view/v_m40a1.vmdl";
	public override string WorldModelPath => "models/op4/weapons/world/w_m40a1.vmdl";
	public override float PrimaryRate => 0.75f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 2.5f;
	public override AmmoType AmmoType => AmmoType.Sniper;
	public override int ClipSize => 5;
	public override int Bucket => 5;
	public override int BucketWeight => 3;
	public override string InventoryIcon => "/ui/op4/weapons/weapon_sniperrifle.png";
	public override string InventoryIconSelected => "/ui/op4/weapons/weapon_sniperrifle_selected.png";

	[Net, Predicted]
	public bool Zoomed { get; set; } = false;

	private float? LastFov;
	private float? LastViewmodelFov;

	public override void Spawn()
	{
		base.Spawn();

		AmmoClip = 5;
		Model = Model.Load( "models/op4/weapons/world/w_m40a1.vmdl" );
	}

	public override void AttackPrimary()
	{
		if ( Owner is not HLPlayer player ) return;
		if ( !TakeAmmo( 1 ) )
		{
			DryFire();

			if ( AvailableAmmo() > 0 )
			{
				Reload();
			}
			return;
		}

		ShootEffects();
		PlaySound( "sniper_fire" );


		ShootBullet( 0.0f, 1f, 100.0f, 2.0f );


		player.punchanglecl.x = -2;

	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
		{
			Zoomed = !Zoomed;
		}
		//Zoomed = Input.Down( InputButton.SecondaryAttack );
	}

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		float targetFov = camSetup.FieldOfView;
		float targetViewmodelFov = camSetup.ViewModel.FieldOfView;
		LastFov = LastFov ?? camSetup.FieldOfView;
		LastViewmodelFov = LastViewmodelFov ?? camSetup.ViewModel.FieldOfView;

		if ( Zoomed )
		{
			targetFov = 18.0f;
			targetViewmodelFov = 0.0f;
		}

		float lerpedFov = LastFov.Value.LerpTo( targetFov, Time.Delta * 24.0f );
		float lerpedViewmodelFov = LastViewmodelFov.Value.LerpTo( targetViewmodelFov, Time.Delta * 24.0f );

		camSetup.FieldOfView = targetFov;
		camSetup.ViewModel.FieldOfView = targetViewmodelFov;

		LastFov = lerpedFov;
		LastViewmodelFov = lerpedViewmodelFov;
	}

	public override void BuildInput( InputBuilder owner )
	{
		if ( Zoomed )
		{
			owner.ViewAngles = Angles.Lerp( owner.OriginalViewAngles, owner.ViewAngles, 0.2f );
		}
	}

	[ClientRpc]
	protected override void ShootEffectsRPC()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "fire", true );
	}
	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Crossbow ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
