using static Sandbox.Package;

[Library( "weapon_mp5" ), HammerEntity]
[Alias( "weapon_9mmAR" )]
[EditorModel( "models/hl1/weapons/world/mp5.vmdl" )]
[Title( "SMG" ), Category( "Weapons" )]
partial class SMG : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/mp5.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_mp5.vmdl";

	public override float PrimaryRate => 10.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 50;
	public override float ReloadTime => 1.5f;
	public override int Bucket => 2;
	public override int BucketWeight => 1;
	public override bool HasAltAmmo => true;
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
		PlaySound( "hks" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );

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
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Rifle ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}

}
