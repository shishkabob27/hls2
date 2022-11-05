using static Sandbox.Package;

[Library( "weapon_mp5" ), HammerEntity]
[Alias( "weapon_9mmAR" )]
[EditorModel( "models/hl1/weapons/world/mp5.vmdl" )]
[Title( "SMG" ), Category( "Weapons" ), MenuCategory( "Half-Life" )]
partial class SMG : Weapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/mp5.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_mp5.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/mp5.vmdl";
	public override bool HasHDModel => true;

	public override float PrimaryRate => 0.1f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 50;
	public override float ReloadTime => 1.5f;
	public override int Bucket => 2;
	public override int BucketWeight => 1;
	public override bool HasAltAmmo => true;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair8.png";
	public override string InventoryIcon => "/ui/weapons/weapon_smg.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_smg_selected.png";

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 20;
		AltAmmoClip = 0;
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

		if (!HLGame.cl_himodels)
		{
			PlaySound( "sounds/hl1/weapons/hks.sound" );
		}
		else
		{
			PlaySound( "sounds/hl1hd/weapons/hks.sound" );
		}
		

		//
		// Shoot the bullets
		//
		ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );
		ViewPunch( 0, Rand.Float( -2, 2 ) );

	}

	public override void AttackSecondary()
	{

		TimeSinceSecondaryAttack = 0;

		if ( Owner is not HLPlayer player ) return;

		var owner = Owner as HLPlayer;
		if ( owner.TakeAmmo( AltAmmoType, 1 ) == 0 )
		{
			return;
		}

		// woosh sound
		// screen shake

		PlaySound( "glauncher" );

		Rand.SetSeed( Time.Tick );

		ViewPunch( 0, -10 );

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );
		ViewModelEntity?.SetAnimParameter( "fire_grenade", true );

		if ( IsServer )
		{
			var grenade = new SMGGrenade
			{
				Owner = Owner,
				Rotation = Rotation.LookAt( GetFiringRotation().Forward ),
				Position = GetFiringPos() + GetFiringRotation().Forward * 40
			};

			grenade.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			grenade.PhysicsGroup.Velocity = GetFiringRotation().Forward * 1000;

			grenade.ApplyLocalAngularImpulse( new Vector3( 0, 300, 0 ) );
		}

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
		SetHoldType( HLCombat.HoldTypes.Rifle, anim );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}

}
