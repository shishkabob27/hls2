[Library( "weapon_m249" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "weapon_m249" ), Category( "Weapons" )]
class M249 : HLWeapon
{

    public override int Bucket => 5;
    public override int BucketWeight => 1;
    public override string InventoryIcon => "/ui/op4/weapons/weapon_m249.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_m249_selected.png";
}
