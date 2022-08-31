[Library("weapon_gauss"), HammerEntity]
[EditorModel("models/hl1/weapons/world/gauss.vmdl")]
[Title("Gauss"), Category("Weapons")]
partial class Gauss : HLWeapon
{
    //stub

    public override int Bucket => 3;
    public override int BucketWeight => 2;
}