class spark : Entity
{
	public spark( Vector3 position, bool doSound = true )
	{
		Position = position;
		Particles.Create( "particles/spark.vpcf", Position );
		if ( doSound )
		{
			Sound.FromWorld( "spark", Position );
		}
	}
	public spark()
	{
		Particles.Create( "particles/spark.vpcf", Position );
	}
}
