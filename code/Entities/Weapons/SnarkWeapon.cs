[Library("weapon_snark"), HammerEntity]
[EditorModel("models/hl1/weapons/world/snarknest.vmdl")]
[Title("Snark"), Category("Weapons")]
partial class SnarkWeapon : HLWeapon
{
    //stub
    public override int Bucket => 4;
    public override int BucketWeight => 4;
    public override AmmoType AmmoType => AmmoType.Snark;
	public override string AmmoIcon => "ui/ammo11.png";
    public override int ClipSize => 1;

}