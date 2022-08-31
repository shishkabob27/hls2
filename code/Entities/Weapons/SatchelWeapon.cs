[Library("weapon_satchel"), HammerEntity]
[EditorModel("models/hl1/weapons/world/satchel.vmdl")]
[Title("Satchel"), Category("Weapons")]
partial class SatchelWeapon : HLWeapon
{
    //stub

    public override int Bucket => 4;
    public override int BucketWeight => 2;
    public override AmmoType AmmoType => AmmoType.Satchel;

}