[Library("weapon_hornetgun"), HammerEntity]
[EditorModel("models/hl1/weapons/world/hornet.vmdl")]
[Title("Hornet Gun"), Category("Weapons")]
partial class HornetGun : HLWeapon
{
    //stub

    public override string ViewModelPath => "models/hl1/weapons/view/v_hgun.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 4;
    public override AmmoType AmmoType => AmmoType.None; // we do this so we can always select the weapon.
	public override string AmmoIcon => "ui/ammo8.png";
    public override int ClipSize => 1;

}