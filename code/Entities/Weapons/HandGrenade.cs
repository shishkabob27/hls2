partial class HandGrenade : HLMovement
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/grenade.vmdl" );
	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		Friction = 0.8f;
		Gravity = 0.5f;

		minsOverride = CollisionBounds.Mins;
		maxsOverride = CollisionBounds.Maxs;

	}

	public async Task BlowIn( float seconds )
	{
		await Task.DelaySeconds( seconds );

		HLExplosion.Explosion( this, Owner, Position, 256, 100, 24.0f, "grenade" );
		Delete();
	}
}
