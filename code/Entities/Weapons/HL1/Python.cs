[Library( "weapon_357" ), HammerEntity]
[Alias( "weapon_python" )]
[EditorModel( "models/hl1/weapons/world/python.vmdl" )]
[Title( ".357 Magnum Revolver" ), Category( "Weapons" ), MenuCategory( "Half-Life" )]
partial class Python : Weapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/python.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_python.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/python.vmdl";
	public override bool HasHDModel => true;
	public override float PrimaryRate => 0.75f;
	public override float SecondaryRate => 0.5f;
	public override float ReloadTime => 2.4f;
	public override int ClipSize => 6;
	public override AmmoType AmmoType => AmmoType.Python;

	public override int Bucket => 1;
	public override int BucketWeight => 2;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair3.png";
	public override string AmmoIcon => "ui/ammo2.png";
	public override string InventoryIcon => "/ui/weapons/weapon_357.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_357_selected.png";

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 6;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack();
	}

	public override void AttackPrimary()
	{
		if ( Owner is not HLPlayer player ) return;
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
		ShootBullet( 0.01f, 4.0f, 40.0f, 2.0f );
		ViewPunch( 0, -10 );
	}
	[ClientRpc]
	protected override void ShootEffectsRPC()
	{

		Game.AssertClient();

		if ( Client.IsUsingVr )
		{
			Particles.Create( "particles/muzflash2.vpcf", VRWeaponModel, "muzzle" );
		}
		else
		{
			Particles.Create( "particles/muzflash2.vpcf", EffectEntity, "muzzle" );
		}



		ViewModelEntity?.SetAnimParameter( "fire", true );

		if ( Owner is HLPlayer player )
		{
			player.SetAnimParameter( "b_attack", true );
		}
	}
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
	}
	public override void SimulateAnimator( CitizenAnimationHelper anim )
	{
		SetHoldType( HLCombat.HoldTypes.Python, anim );
		//anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
