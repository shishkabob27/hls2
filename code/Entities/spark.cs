class spark : Entity
{
	public virtual bool DoSound => true;
	public override void Spawn()
	{
		Particles.Create( "particles/spark.vpcf", Position );
		if ( DoSound )
		{
			Sound.FromWorld( "spark", Position );
		}
		base.Spawn();
		Delete();
	}
}
