[Library( "hl_handgrenade" ), HammerEntity]
[Title( "Hand Grenade" )]
partial class HandGrenade : BasePhysics
{
	public static readonly Model WorldModel = Model.Load("models/hl1/weapons/world/grenade.vmdl");

	Particles GrenadeParticles;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

	}

	public async Task BlowIn( float seconds )
	{
		await Task.DelaySeconds( seconds );

		HLGame.Explosion( this, Owner, Position, 256, 100, 1.0f );
		Delete();
	}
}
