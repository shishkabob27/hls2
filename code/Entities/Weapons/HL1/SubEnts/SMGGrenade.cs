partial class SMGGrenade : BasePhysics
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/grenade_mp5.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	[GameEvent.Tick.Server]
	public void Simulate()
	{
		var trace = Trace.Ray( Position, Position )
			.Size( 24 )
			.Ignore( this )
			.Ignore( Owner )
			.Run();

		Position = trace.EndPosition;

		if ( trace.Hit == true )
		{
			BlowUp();
		}
	}

	public void BlowUp()
	{

		Sound.FromWorld( "debris", Position );
		HLExplosion.Explosion( this, Owner, Position, 250, 100, 24.0f, "grenade" );
		Delete();
	}
}
