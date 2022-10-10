[Library( "weapon_displacer" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "Displacer Cannon" ), Category( "Weapons" )]
class Displacer : HLWeapon
{

    public override int Bucket => 5;
    public override int BucketWeight => 2;
    public override string InventoryIcon => "/ui/op4/weapons/weapon_displacer.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_displacer_selected.png";
}
