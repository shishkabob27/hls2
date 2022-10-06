
partial class LaserDot : Entity
{
	public override void Spawn()
	{
		sprite();
		base.Spawn();
	}
	[ClientRpc]
	void sprite()
	{
		Particles.Create( "particles/laserdot.vpcf", this, true );
	}
}
