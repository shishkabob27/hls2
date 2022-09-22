[Library( "weapon_satchel" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/satchel.vmdl" )]
[Title( "Satchel" ), Category( "Weapons" )]
partial class SatchelWeapon : HLWeapon
{
    //stub
    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/satchel.vmdl" );

    public override string ViewModelPath => "models/hl1/weapons/view/v_satchel.vmdl";

    public override int Bucket => 4;
    public override int BucketWeight => 2;
    public override AmmoType AmmoType => AmmoType.Satchel;
    public override string AmmoIcon => "ui/ammo10.png";
    public override string InventoryIcon => "/ui/weapons/weapon_satchel.png";
    public override int ClipSize => -1;

    public override void Spawn()
    {
        base.Spawn();

        Model = WorldModel;
        AmmoClip = 0;
        WeaponIsAmmo = true;
    }
    public override void SimulateAnimator( PawnAnimator anim )
    {
        anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.HoldItem ); // TODO this is shit
        anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }
}