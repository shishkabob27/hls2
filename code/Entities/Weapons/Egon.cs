﻿[Library("weapon_egon"), HammerEntity]
[EditorModel("models/hl1/weapons/world/egon.vmdl")]
[Title("Egon"), Category("Weapons")]
partial class Egon : HLWeapon
{
    //stub

    public override string ViewModelPath => "models/hl1/weapons/view/v_egon.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 3;
    public override AmmoType AmmoType => AmmoType.Uranium;
    public override string AmmoIcon => "ui/ammo7.png";

}