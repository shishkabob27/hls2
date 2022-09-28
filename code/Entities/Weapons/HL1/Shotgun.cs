[Library( "weapon_shotgun" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/shotgun.vmdl" )]
[Title( "Shotgun" ), Category( "Weapons" )]
partial class Shotgun : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/shotgun.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_shotgun.vmdl";
	public override float PrimaryRate => 0.75f;
	public override float SecondaryRate => 1.5f;
	public override AmmoType AmmoType => AmmoType.Buckshot;
	public override int ClipSize => 8;
	public override float ReloadTime => 0.5f;
	public override int Bucket => 2;
	public override int BucketWeight => 2;

	public override string AmmoIcon => "ui/ammo4.png";
	public override string InventoryIcon => "/ui/weapons/weapon_shotgun.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_shotgun_selected.png";

	[Net, Predicted]
	public bool StopReloading { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 6;
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		if ( IsReloading && ( Input.Pressed( InputButton.PrimaryAttack ) || Input.Pressed( InputButton.SecondaryAttack ) ) )
		{
			StopReloading = true;
		}
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

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "shotgun_shot" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.2f, 0.3f, 20.0f, 2.0f, 4 );
		ViewPunch( 0, -5 );
	}

	public override void AttackSecondary()
	{
		if ( Owner is not HLPlayer player ) return;
		TimeSincePrimaryAttack = -1.5f + PrimaryRate;
		TimeSinceSecondaryAttack = 0f;

		if ( !TakeAmmo( 2 ) )
		{
			DryFire();
			return;
		}

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		DoubleShootEffects();
		PlaySound( "shotgun_shot_double" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.4f, 0.3f, 20.0f, 2.0f, 8 );
		ViewPunch( 0, -10 );
	}

	[ClientRpc]
	protected override void ShootEffectsRPC()
	{
		Host.AssertClient();

		Particles.Create( "particles/muzflash2.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimParameter( "fire", true );
	}

	[ClientRpc]
	protected virtual void DoubleShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/muzflash2.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimParameter( "fire_double", true );
	}

	public override void OnReloadFinish()
	{
		var stop = StopReloading;

		StopReloading = false;
		IsReloading = false;

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( AmmoClip >= ClipSize )
			return;

		if ( Owner is HLPlayer player )
		{
			var ammo = player.TakeAmmo( AmmoType, 1 );
			if ( ammo == 0 )
				return;

			AmmoClip += ammo;

			if ( AmmoClip < ClipSize && !stop )
			{
				Reload();
			}
			else
			{
				FinishReload();
			}
		}
	}

	[ClientRpc]
	protected virtual void FinishReload()
	{
		ViewModelEntity?.SetAnimParameter( "reload_finished", true );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Shotgun ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}

}
