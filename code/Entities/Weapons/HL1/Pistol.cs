[Library( "weapon_9mmhandgun" ), HammerEntity]
[Alias( "weapon_glock" )]
[EditorModel( "models/hl1/weapons/world/glock.vmdl" )]
[Title( "Pistol" ), Category( "Weapons" )]
partial class Pistol : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/glock.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_glock.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/glock.vmdl";
	public override int ClipSize => 17;
	public override float PrimaryRate => 0.3f;
	public override float SecondaryRate => 0.2f;
	public override float ReloadTime => 1.4f;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair2.png";
	public override string InventoryIcon => "/ui/weapons/weapon_pistol.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_pistol_selected.png";
	public override bool HasHDModel => true;
	public override int Bucket => 1;
	public override int BucketWeight => 1;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 17;
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
		PlaySound( "pistol_shot" );
		//
		// Shoot the bullets
		//
		ShootBullet( 0.05f, 1, 8.0f, 2.0f );

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );
		ViewPunch( 0, -2 );
	}

	public override void AttackSecondary()
	{
		if ( Owner is not HLPlayer player ) return;
		base.AttackSecondary();

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "pistol_shot" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.4f, 1.5f, 8.0f, 3.0f );

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );
		ViewPunch( 0, -2 );
	}

	[ClientRpc]
	protected override void ShootEffectsRPC()
	{
		Host.AssertClient();

		if ( Client.IsUsingVr )
		{
			Particles.Create( "particles/muzflash.vpcf", VRWeaponModel, "muzzle" );
		}
		else
		{
			Particles.Create( "particles/muzflash.vpcf", EffectEntity, "muzzle" );
		}
		if ( Client.IsUsingVr )
		{
			Particles.Create( "particles/pistol_ejectbrass.vpcf", VRWeaponModel, "ejection_point" );
		}
		else
		{
			Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
	}

}
