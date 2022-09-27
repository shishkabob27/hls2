[Library( "weapon_cubemap" )]
[Title( "weapon_cubemap" ), Category( "Weapons" )]
partial class weapon_cubemap : HLWeapon
{
    public override string ViewModelPath => "models/shadertest/envballs.vmdl";

    public override int Bucket => 1;
    public override int BucketWeight => 1000;
}
