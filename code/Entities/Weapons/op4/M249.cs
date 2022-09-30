[Library( "weapon_m249" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/m249.vmdl" )]
[Title( "weapon_m249" ), Category( "Weapons" )]
partial class M249 : HLWeapon
{
    public override string ViewModelPath => "models/op4/weapons/view/v_saw.vmdl";
    public override float PrimaryRate => 0.067f;
	public override int ClipSize => 50;
    public override AmmoType AmmoType => AmmoType.M249;
	public override float ReloadTime => 3.78f;
    public override int Bucket => 5;
    public override int BucketWeight => 1;
    public override string InventoryIcon => "/ui/op4/weapons/weapon_m249.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_m249_selected.png";
    public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op4/weapons/world/w_saw.vmdl" );
		AmmoClip = 50;
	}

	public override void AttackPrimary()
	{
		if ( Owner is not HLPlayer player ) return;
		TimeSincePrimaryAttack = 0;

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

		PlaySound( "saw_fire" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.15f, 1.5f, 5.0f, 3.0f );
		ViewPunch( 0, Rand.Float( -2, 2 ) );

	}

	[ClientRpc]
	protected override void ShootEffectsRPC()
	{
		Host.AssertClient();

		if ( Client.IsUsingVr )
		{
			Particles.Create( "particles/muzflash1.vpcf", VRWeaponModel, "muzzle" );
		}
		else
		{
			Particles.Create( "particles/muzflash1.vpcf", EffectEntity, "muzzle" );
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

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Rifle ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}

}