[Library( "weapon_eagle" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "weapon_eagle" ), Category( "Weapons" )]
class DEagle : HLWeapon
{

    public override int Bucket => 1;
    public override int BucketWeight => 3;

    public override string InventoryIcon => "/ui/op4/weapons/weapon_eagle.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_eagle_selected.png";
}
