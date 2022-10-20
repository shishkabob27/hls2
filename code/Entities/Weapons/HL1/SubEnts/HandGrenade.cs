[Library( "ggrenade" )]
partial class HandGrenade : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/grenade.vmdl" );
	public Movement c;
	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		c = Components.Create<Movement>();
		c.Friction = 0.8f;
		c.Gravity = 0.5f;

		c.minsOverride = CollisionBounds.Mins;
		c.maxsOverride = CollisionBounds.Maxs;

	}

	public async Task BlowIn( float seconds )
	{
		await Task.DelaySeconds( seconds );

		Sound.FromWorld( "debris", Position );
		HLExplosion.Explosion( this, Owner, Position, 256, 100, 24.0f, "grenade" );
		Delete();
	}
}
