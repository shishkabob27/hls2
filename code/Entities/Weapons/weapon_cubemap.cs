[Library("weapon_cubemap")]
[Title("weapon_cubemap"), Category( "Weapons" )]
partial class weapon_cubemap : HLWeapon
{
	public static readonly Model WorldModel = Model.Load("models/hl1/weapons/world/glock.vmdl");
	public override string ViewModelPath => "models/shadertest/envballs.vmdl";
	
	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack();
	}

	public override void AttackPrimary()
	{
	}
}
