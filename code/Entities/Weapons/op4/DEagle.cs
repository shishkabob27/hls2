[Library( "weapon_eagle" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/w_desert_eagle.vmdl" )]
[Title( "Desert Eagle" ), Category( "Weapons" )]
partial class DEagle : HLWeapon
{
	public override string ViewModelPath => "models/op4/weapons/view/v_desert_eagle.vmdl";
	public override float PrimaryRate => 0.22f;
	public override float SecondaryRate => 0.2f;
	public override float ReloadTime => 1.4f;
	public override int ClipSize => 7;
	public override int Bucket => 1;
	public override int BucketWeight => 3;
	public override AmmoType AmmoType => AmmoType.Python;
	public override string AmmoIcon => "ui/ammo2.png";
	public override string InventoryIcon => "/ui/op4/weapons/weapon_eagle.png";
	public override string InventoryIconSelected => "/ui/op4/weapons/weapon_eagle_selected.png";
	LaserDot Dot;

	[Net]
	public bool isLaserOn { get; set; } = false;
	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op4/weapons/world/w_desert_eagle.vmdl" );
		AmmoClip = 7;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack();
	}

	public override void AttackPrimary()
	{

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;
		if ( isLaserOn )
		{

			TimeSincePrimaryAttack = -0.6f;
			TimeSinceSecondaryAttack = 0;
		}

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
		PlaySound( "desert_eagle_fire" );

		//
		// Shoot the bullets
		//
		if ( isLaserOn )
		{
			ShootBullet( 0, 1, 34.0f, 2.0f );
		}
		else
		{
			ShootBullet( 0.25f, 1, 34.0f, 2.0f );
		}

		(Owner as AnimatedEntity).SetAnimParameter( "b_attack", true );
	}
	public override void AttackSecondary()
	{
		Log.Info( "Gaming" );
		base.AttackSecondary();
		if ( !isLaserOn && IsServer )
		{
			Dot = new LaserDot();
			isLaserOn = true;
		}
		else if ( IsServer )
		{
			Dot.Delete();
			Dot = null;
			isLaserOn = false;
		}
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );
		if ( Owner is not HLPlayer ply ) return;
		if ( Dot != null )
		{
			Dot.Position = Trace.Ray( ply.EyePosition, ply.EyePosition + ply.EyeRotation.Forward * 10000 )
				.WithoutTags( "player" )
				.Ignore( this )
				.Run()
				.EndPosition - ply.EyeRotation.Forward * 1;
		}
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		if ( isLaserOn && IsServer )
		{
			Dot.Delete();
			Dot = null;
			isLaserOn = false;
		}
	}
}
