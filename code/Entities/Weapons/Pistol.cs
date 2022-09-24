﻿[Library( "weapon_9mmhandgun" ), HammerEntity]
[Alias( "weapon_glock" )]
[EditorModel( "models/hl1/weapons/world/glock.vmdl" )]
[Title( "Pistol" ), Category( "Weapons" )]
partial class Pistol : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/glock.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_glock.vmdl";

	public override int ClipSize => 17;
	public override float PrimaryRate => 3.3f;
	public override float SecondaryRate => 5f;
	public override float ReloadTime => 1.4f;

	public override string InventoryIcon => "/ui/weapons/weapon_pistol.png";
	public override int Bucket => 1;

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
	}

	public override void AttackSecondary()
	{
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
	}

}

[Library("weapon_glock"), HammerEntity]
[EditorModel( "models/hl1/weapons/world/glock.vmdl" )]
[Title( "Pistol" ), Category( "Weapons" )]
partial class Glock : Pistol
{
	
}