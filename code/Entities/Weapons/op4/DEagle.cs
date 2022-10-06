[Library( "weapon_eagle" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/w_desert_eagle.vmdl" )]
[Title( "weapon_eagle" ), Category( "Weapons" )]
class DEagle : HLWeapon
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
		ShootBullet( 0.25f, 1, 34.0f, 2.0f );

		(Owner as AnimatedEntity).SetAnimParameter( "b_attack", true );
	}
	public override void AttackSecondary()
	{
		Log.Info( "Gaming" );
		base.AttackSecondary();
		if ( Dot == null && IsServer )
		{
			Dot = new LaserDot();
		}
		else if ( IsServer )
		{
			Dot.Delete();
			Dot = null;
		}
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );
		if ( Owner is not HLPlayer ply ) return;
		if ( Dot != null && IsServer )
		{
			Dot.Position = Trace.Ray( ply.EyePosition, ply.EyePosition + ply.EyeRotation.Forward * 10 ).WithoutTags( "player" ).Run().EndPosition;
		}
	}
}
