[Library( "weapon_crossbow" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/crossbow.vmdl" )]
[Title( "Crossbow" ), Category( "Weapons" ), MenuCategory("Half-Life")]
partial class Crossbow : Weapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/crossbow.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_crossbow.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/crossbow.vmdl";
	public override float PrimaryRate => 0.75f;
	public override int Bucket => 2;
	public override int BucketWeight => 3;
	public override AmmoType AmmoType => AmmoType.Crossbow;
	public override int ClipSize => 5;
	public override bool HasHDModel => true;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair4.png";
	public override string AmmoIcon => "ui/ammo5.png";
	public override string InventoryIcon => "/ui/weapons/weapon_crossbow.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_crossbow_selected.png";

	[Net, Predicted]
	public bool Zoomed { get; set; }

	private float? LastFov;
	private float? LastViewmodelFov;

	public override void Spawn()
	{
		base.Spawn();

		AmmoClip = 5;
		Model = WorldModel;
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
		PlaySound( "crossbow_shot" );

		// TODO - if zoomed in then instant hit, no travel, 120 damage
		if ( Zoomed )
		{

			ShootBullet( 0.0f, 1, 50.0f, 1.0f, 1, false );
			return;
		}

		ViewPunch( 0, -2 );

		if ( IsServer )
		{
			var bolt = new CrossbowBolt();
			bolt.Position = GetFiringPos();
			bolt.Rotation = GetFiringRotation();
			bolt.Owner = Owner;
			bolt.Velocity = GetFiringRotation().Forward * 100;
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
		{
			Zoomed = !Zoomed;
		}
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
			targetFov = 20.0f;
			targetViewmodelFov = 20.0f;
		}

		float lerpedFov = LastFov.Value.LerpTo( targetFov, Time.Delta * 24.0f );
		float lerpedViewmodelFov = LastViewmodelFov.Value.LerpTo( targetViewmodelFov, Time.Delta * 24.0f );

		camSetup.FieldOfView = targetFov;
		camSetup.ViewModel.FieldOfView = targetViewmodelFov;

		LastFov = lerpedFov;
		LastViewmodelFov = lerpedViewmodelFov;
	}

	public override void BuildInput()
	{
		var owner = Owner as HLPlayer;
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
		SetHoldType( HLCombat.HoldTypes.Crossbow, anim );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
