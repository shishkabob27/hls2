[Library( "weapon_cubemap" )]
[Title( "weapon_cubemap" ), Category( "Weapons" )]
partial class CubeMapWeapon : Weapon
{
    public override string ViewModelPath => "models/shadertest/envballs.vmdl";

    public override int Bucket => 1;
    public override int BucketWeight => 1000;
	public override AmmoType AmmoType => AmmoType.None;
	public override int ClipSize => 0;
}
