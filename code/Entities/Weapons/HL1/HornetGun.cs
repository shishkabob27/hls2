[Library( "weapon_hornetgun" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/hgun.vmdl" )]
[Title( "Hornet Gun" ), Category( "Weapons" )]
partial class HornetGun : HLWeapon
{
    //stub
    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/hgun.vmdl" );
    public override string ViewModelPath => "models/hl1/weapons/view/v_hgun.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/hgun.vmdl";
    public override int Bucket => 3;
    public override int BucketWeight => 4;
    public override AmmoType AmmoType => AmmoType.Hornet; // we do this so we can always select the weapon.
    public override string CrosshairIcon => "/ui/crosshairs/crosshair7.png";
	public override string AmmoIcon => "ui/ammo8.png";
    public override string InventoryIcon => "/ui/weapons/weapon_hornetgun.png";
    public override string InventoryIconSelected => "/ui/weapons/weapon_hornetgun_selected.png";
	public override bool HasHDModel => true;
    public override int ClipSize => -1;
    public override float PrimaryRate => 0.25f;
    public override float SecondaryRate => 0.1f;

    int tickammoregen = 0;
    int FirePhase = 0;

    public override void Spawn()
    {
        base.Spawn();

        Model = WorldModel;
        AmmoClip = 0;
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
            owner2.GiveAmmo( AmmoType.Hornet, 1 );
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
        if ( owner.TakeAmmo( AmmoType.Hornet, 1 ) == 0 )
        {
            return;
        }
        tickammoregen = 0;


        var vecSrc = GetFiringPos() + GetFiringRotation().Forward * 16 + GetFiringRotation().Right * 8 + GetFiringRotation().Up * -12;
        ViewModelEntity?.SetAnimParameter( "fire", true );
        if ( IsServer )
        {
            var hornet = new Hornet();
            hornet.Position = vecSrc;
            hornet.Rotation = GetFiringRotation();
            hornet.Owner = Owner;
            hornet.Velocity = GetFiringRotation().Forward * 300;
        }
        ViewPunch( 0, Rand.Float( -2, 2 ) );
    }

    public override void AttackSecondary()
    {
        var owner = Owner as HLPlayer;
        if ( owner.TakeAmmo( AmmoType.Hornet, 1 ) == 0 )
        {
            return;
        }
        tickammoregen = 0;
        var vecSrc = GetFiringPos() + GetFiringRotation().Forward * 16 + GetFiringRotation().Right * 8 + GetFiringRotation().Up * -12;
        FirePhase++;
        switch ( FirePhase )
        {
            case 1:
                vecSrc = vecSrc + GetFiringRotation().Up * 8;
                break;
            case 2:
                vecSrc = vecSrc + GetFiringRotation().Up * 8;
                vecSrc = vecSrc + GetFiringRotation().Right * 8;
                break;
            case 3:
                vecSrc = vecSrc + GetFiringRotation().Right * 8;
                break;
            case 4:
                vecSrc = vecSrc + GetFiringRotation().Up * -8;
                vecSrc = vecSrc + GetFiringRotation().Right * 8;
                break;
            case 5:
                vecSrc = vecSrc + GetFiringRotation().Up * -8;
                break;
            case 6:
                vecSrc = vecSrc + GetFiringRotation().Up * -8;
                vecSrc = vecSrc + GetFiringRotation().Right * -8;
                break;
            case 7:
                vecSrc = vecSrc + GetFiringRotation().Right * -8;
                break;
            case 8:
                vecSrc = vecSrc + GetFiringRotation().Up * 8;
                vecSrc = vecSrc + GetFiringRotation().Right * -8;
                FirePhase = 0;
                break;
        }

        ViewModelEntity?.SetAnimParameter( "fire", true );
        if ( IsServer )
        {
            var hornet = new Hornet();
            hornet.Position = vecSrc;
            hornet.Rotation = GetFiringRotation();
            hornet.Owner = Owner;
            hornet.Velocity = GetFiringRotation().Forward * 1200;
            hornet.Dart = true;
        }
    }
    public override void SimulateAnimator( PawnAnimator anim )
    {
        anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Hive ); // TODO this is shit
        anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }
}
