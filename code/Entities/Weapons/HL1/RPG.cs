[Library( "weapon_rpg" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/rpg.vmdl" )]
[Title( "RPG" ), Category( "Weapons" )]
partial class RPG : Weapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/rpg.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_rpg.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/rpg.vmdl";
	public override string Category => "Half-Life";
	public override bool HasHDModel => true;
	public override float PrimaryRate => 1.5f;
	public override int Bucket => 3;
	public override int BucketWeight => 1;
	public override AmmoType AmmoType => AmmoType.RPG;
	public override int ClipSize => 1;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair9.png";
	public override string AmmoIcon => "ui/ammo6.png";
	public override string InventoryIcon => "/ui/weapons/weapon_rpg.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_rpg_selected.png";
	public override void Spawn()
	{
		base.Spawn();

		AmmoClip = 1;
		Model = WorldModel;
	}

	public override void AttackPrimary()
	{
		if ( Owner is not HLPlayer player ) return;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();

			if ( AvailableAmmo() > 0 )
			{
				Reload();
			}
			return;
		}

		ShootEffects();
		PlaySound( "rocketfire1" );

		// TODO - if zoomed in then instant hit, no travel, 120 damage


		if ( IsServer )
		{
			var bolt = new RPGRocket();
			bolt.Position = GetFiringPos();
			bolt.Rotation = GetFiringRotation();
			bolt.Owner = Owner;
			bolt.Velocity = GetFiringRotation().Forward * 100;
		}
		if ( IsServer && player.AmmoCount( AmmoType.RPG ) == 0 )
		{

			player.SwitchToBestWeapon();
		}
		ViewPunch( 0, -5 );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
	}



	[ClientRpc]
	protected override void ShootEffectsRPC()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "fire", true );
	}
	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.RPG ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
