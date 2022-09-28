[Library( "weapon_gibweapon" )]
[EditorModel( "models/hl1/weapons/world/glock.vmdl" )]
[Title( "GibWeapon" ), Category( "Weapons" )]
partial class GibWeapon : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/glock.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_glock.vmdl";
	public override float PrimaryRate => 0.1f;

	public override int Bucket => 1;
	public override int BucketWeight => 100;
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

		if ( IsServer )
		{
			HLCombat.CreateGibs( Owner.Position, Owner.Position, 0, new BBox( new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) ) );

		}

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );
	}
}
