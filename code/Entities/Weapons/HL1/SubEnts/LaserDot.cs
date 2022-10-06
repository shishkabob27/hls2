
partial class LaserDot : Entity
{
	Particles spritep;
	public override void Spawn()
	{
		Transmit = TransmitType.Always;
		base.Spawn();
		sprite();
	}
	void sprite()
	{
		using ( Prediction.Off() )
		{

			spritep = Particles.Create( "particles/laserdot.vpcf", this, true );
		}
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ( spritep != null )
		{
			spritep.Destroy( true );
			spritep.Dispose();
		}
	}
}
