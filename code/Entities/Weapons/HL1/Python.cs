[Library( "weapon_357" ), HammerEntity]
[Alias( "weapon_python" )]
[EditorModel( "models/hl1/weapons/world/python.vmdl" )]
[Title( ".357 Magnum Revolver" ), Category( "Weapons" )]
partial class Python : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/python.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_python.vmdl";

	public override float PrimaryRate => 1.5f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 2.4f;
	public override int ClipSize => 6;
	public override AmmoType AmmoType => AmmoType.Python;

	public override int Bucket => 1;
	public override int BucketWeight => 200;
	public override string AmmoIcon => "ui/ammo2.png";
	public override string InventoryIcon => "/ui/weapons/weapon_357.png";

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
	[ClientRpc]
	protected override void ShootEffectsRPC()
	{

		Host.AssertClient();

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
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
	}
	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Python ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
