[Library( "monster_satchel" )]
partial class Satchel : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/satchel.vmdl" );
	public Movement c;
	public override void Spawn()
	{

		base.Spawn();
		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		c = Components.Create<Movement>();
		c.Friction = 0.9f;
		c.bHeight = 8;
		c.minsOverride = CollisionBounds.Mins;
		c.maxsOverride = CollisionBounds.Maxs;
	}
	public override void Touch( Entity other )
	{
		PlaySound( "g_bounce" );
		base.StartTouch( other );
	}
	public void Explode()
	{

		Sound.FromWorld( "debris", Position );
		HLExplosion.Explosion( this, Owner, Position, 256, 100, 24.0f, "grenade" );
		Delete();
	}
}
