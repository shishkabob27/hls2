[Library( "weapon_snark" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/snarknest.vmdl" )]
[Title( "Snark" ), Category( "Weapons" )]
partial class SnarkWeapon : HLWeapon
{
    //stub
    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/snarknest.vmdl" );
    public override string ViewModelPath => "models/hl1/weapons/view/v_squeak.vmdl";

    public override int Bucket => 4;
    public override int BucketWeight => 4;
    public override AmmoType AmmoType => AmmoType.Snark;
    public override string AmmoIcon => "ui/ammo11.png";
    public override string InventoryIcon => "/ui/weapons/weapon_snark.png";
    public override int ClipSize => 1;

    public override void Spawn()
    {
        base.Spawn();

        Model = Model.Load( "models/hl1/weapons/world/squeak.vmdl" );
        AmmoClip = 0;
    }
    public override void SimulateAnimator( PawnAnimator anim )
    {
        anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Squeak ); // TODO this is shit
        anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }
}