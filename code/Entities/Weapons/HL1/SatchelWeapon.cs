[Library( "weapon_satchel" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/satchel.vmdl" )]
[Title( "Satchel" ), Category( "Weapons" )]
partial class SatchelWeapon : HLWeapon
{
	//stub
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/satchel.vmdl" );

	public override string ViewModelPath => "models/hl1/weapons/view/v_satchel.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/satchel.vmdl";
	public override bool HasHDModel => true;
	public override int Bucket => 4;
	public override int BucketWeight => 2;
	public override float PrimaryRate => 0.5f;
	public override float SecondaryRate => 0.5f;
	public override AmmoType AmmoType => AmmoType.Satchel;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair0.png";
	public override string AmmoIcon => "ui/ammo10.png";
	public override string InventoryIcon => "/ui/weapons/weapon_satchel.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_satchel_selected.png";
	public override int ClipSize => -1;

	[Net]
	List<Satchel> CurrentSatchels { get; set; } = new();
	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 0;
		WeaponIsAmmo = true;
	}

	public override bool IsUsable()
	{
		if ( CurrentSatchels.Count > 0 ) return true;
		return (AvailableAmmo() > 0);
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;

		if ( Owner is not HLPlayer player ) return;

		if ( CurrentSatchels.Count <= 0 )
		{
			AttackSecondary();
		}
		else
		{

			foreach ( var satchel in CurrentSatchels )
			{
				try
				{
					if ( IsServer )
						using ( Prediction.Off() )
						{
							satchel.Explode();
						}
				}
				catch { }

			}
			CurrentSatchels.Clear();
		}


	}
	public override void AttackSecondary()
	{
		TimeSinceSecondaryAttack = 0;
		if ( Owner is not HLPlayer player ) return;

		var owner = Owner as HLPlayer;

		if ( owner.TakeAmmo( AmmoType, 1 ) == 0 )
		{
			return;
		}

		// woosh sound
		// screen shake

		PlaySound( "dm.grenade_throw" );

		Rand.SetSeed( Time.Tick );


		if ( IsServer )
			using ( Prediction.Off() )
			{
				var satchel = new Satchel
				{
					Position = GetFiringPos() + GetFiringRotation().Forward * 3.0f,
					Owner = Owner
				};

				satchel.c.SetVelocity(GetFiringRotation().Forward * 274 + Owner.Velocity);
				satchel.c.SetAngularVelocity( new Angles( 0, 400, 0 ));
				satchel.Rotation = (new Angles( 90, 0, 0 )).ToRotation();

				//grenade.CollisionGroup = CollisionGroup.Debris;
				//grenade.SetInteractsExclude( CollisionLayer.Player );
				//grenade.SetInteractsAs( CollisionLayer.Debris );

				CurrentSatchels.Add( satchel );
			}

		player.SetAnimParameter( "b_attack", true );

		player.SetAnimParameter( "attack", true );

		if ( IsServer && player.AmmoCount( AmmoType.Satchel ) == 0 )
		{

			//player.SwitchToBestWeapon();
		}
	}
	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Trip ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
