[Library( "weapon_sporelauncher" ), HammerEntity]
[EditorModel( "models/op4/weapons/world/knife.vmdl" )]
[Title( "Spore Launcher" ), Category( "Weapons" )]
class SporeLauncher : HLWeapon
{

    public override int Bucket => 6;
    public override int BucketWeight => 1;
    public override string InventoryIcon => "/ui/op4/weapons/weapon_sporelauncher.png";
    public override string InventoryIconSelected => "/ui/op4/weapons/weapon_sporelauncher_selected.png";
}
