[Library( "spark_shower" )]
class Sparkshower : HLMovement
{
	public override void Spawn()
	{
		createSparks();
		Vector3 a;
		Vector3 b = new Vector3( Rotation.Angles().yaw, Rotation.Angles().pitch, Rotation.Angles().roll );
		a = Rand.Float( 200f, 300f ) * b;
		a.x += Rand.Float( -100f, 100f );
		a.y += Rand.Float( -100f, 100f );
		if ( a.z >= 0 )
			a.z += 200;
		else
			a.z -= 200;
		Velocity = a;
		GroundBounce = 0.5f;
		WallBounce = 0.5f;
		base.Spawn();
	}

	public async void createSparks()
	{
		for ( int i = 0; i < 16; i++ )
		{
			Particles.Create( "particles/sparkshower.vpcf", Position );
			await GameTask.DelaySeconds( 0.1f );
		}
		this.Delete();
	}
}
