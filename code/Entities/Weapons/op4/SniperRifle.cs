[Library( "weapon_sniperrifle" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "weapon_sniperrifle" ), Category( "Weapons" )]
class SniperRifle : HLWeapon
{

    public override int Bucket => 5;
    public override int BucketWeight => 3;
    public override string InventoryIcon => "/ui/op4/weapons/weapon_sniperrifle.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_sniperrifle_selected.png";
}
