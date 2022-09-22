[Library( "weapon_hornetgun" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/hgun.vmdl" )]
[Title( "Hornet Gun" ), Category( "Weapons" )]
partial class HornetGun : HLWeapon
{
    //stub
    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/hgun.vmdl" );
    public override string ViewModelPath => "models/hl1/weapons/view/v_hgun.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 4;
    public override AmmoType AmmoType => AmmoType.Hornet; // we do this so we can always select the weapon.
    public override string AmmoIcon => "ui/ammo8.png";
    public override string InventoryIcon => "/ui/weapons/weapon_hornetgun.png";
    public override int ClipSize => 1;
    public override float PrimaryRate => 4;
    public override float SecondaryRate => 9.5f;

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
            var bolt = new Hornet();
            bolt.Position = vecSrc;
            bolt.Rotation = GetFiringRotation();
            bolt.Owner = Owner;
            bolt.Velocity = GetFiringRotation().Forward * 300;
        }
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
            var bolt = new Hornet();
            bolt.Position = vecSrc;
            bolt.Rotation = GetFiringRotation();
            bolt.Owner = Owner;
            bolt.Velocity = GetFiringRotation().Forward * 1200;
            bolt.Dart = true;
        }
    }
    public override void SimulateAnimator( PawnAnimator anim )
    {
        anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Hive ); // TODO this is shit
        anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }
}