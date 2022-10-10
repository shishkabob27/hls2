[Library( "weapon_shockrifle" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "Shock Roach" ), Category( "Weapons" )]
class ShockRifle : HLWeapon
{
	public override string ViewModelPath => "models/op4/weapons/view/v_shock.vmdl";
    public override int Bucket => 6;
    public override int BucketWeight => 2;
	public override AmmoType AmmoType => AmmoType.Plasma;
	public override string AmmoIcon => "/ui/op4/ammo_plasma.png";
    public override string InventoryIcon => "/ui/op4/weapons/weapon_shockrifle.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_shockrifle_selected.png";
	public override int ClipSize => -1;
    public override float PrimaryRate => 0.1f;

    int tickammoregen = 0;

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op4/weapons/world/w_shock.vmdl" );
		AmmoClip = 7;
	}

	public override void Simulate( Client owner )
    {
        base.Simulate( owner );
        if ( Owner is not HLPlayer player ) return;

        var owner2 = Owner as HLPlayer;
        //if (!Input.Down(InputButton.PrimaryAttack))
        tickammoregen += 1;
        if ( tickammoregen >= 24 )
        {
            owner2.GiveAmmo( AmmoType.Plasma, 1 );
            tickammoregen = 0;
        }
    }

    public override bool IsUsable()
    {
        return true;
    }
    public override void AttackPrimary()
    {
        if ( Owner is not HLPlayer player ) return;

        var owner = Owner as HLPlayer;
        if ( owner.TakeAmmo( AmmoType.Plasma, 1 ) == 0 )
        {
            return;
        }
        tickammoregen = 0;


        ViewModelEntity?.SetAnimParameter( "fire", true );
        if ( IsServer )
        {
            var beam = new ShockBeam();
            beam.Position = GetFiringPos();
            beam.Rotation = GetFiringRotation();
            beam.Owner = Owner;
            beam.Velocity = GetFiringRotation().Forward * 300;
        }
        ViewPunch( 0, Rand.Float( -2, 2 ) );
    }

    public override void SimulateAnimator( PawnAnimator anim )
    {
        anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Hive ); // TODO this is shit
        anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }

}
