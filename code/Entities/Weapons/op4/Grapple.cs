[Library( "weapon_grapple" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "Barnacle Grapple" ), Category( "Weapons" )]
class Grapple : HLWeapon
{

    public override int Bucket => 0;
    public override int BucketWeight => 4;
    public override string InventoryIcon => "/ui/op4/weapons/weapon_grapple.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_grapple_selected.png";
}
