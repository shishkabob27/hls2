[Library( "weapon_shockrifle" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "weapon_shockrifle" ), Category( "Weapons" )]
class ShockRifle : HLWeapon
{

    public override int Bucket => 6;
    public override int BucketWeight => 2;
    public override string InventoryIcon => "/ui/op4/weapons/weapon_shockrifle.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_shockrifle_selected.png";
}
