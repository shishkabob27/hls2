partial class SMGGrenade : BasePhysics
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/grenade_mp5.vmdl" );

	public SMGGrenade()
	{
		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	[Event.Tick.Server]
	public void Simulate()
	{
		var trace = Trace.Ray( Position, Position )
			.HitLayer( CollisionLayer.Water, true )
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
		HLGame.Explosion( this, Owner, Position, 250, 100, 1.0f );
		Delete();
	}
}
