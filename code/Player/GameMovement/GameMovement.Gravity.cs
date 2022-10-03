
public partial class Source1GameMovement
{
	public virtual void StartGravity()
	{
		Velocity -= new Vector3( 0, 0, GetCurrentGravity() * 0.5f ) * Time.Delta;
		Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;

		BaseVelocity = BaseVelocity.WithZ( 0 );

		CheckVelocity();
	}

	public virtual void FinishGravity()
	{
		Velocity -= new Vector3( 0, 0, GetCurrentGravity() * 0.5f ) * Time.Delta;
	}

	public virtual float GetCurrentGravity()
	{
		return sv_gravity; // * GameRules.Current.GetGravityMultiplier();
	}

	public void CheckVelocity()
	{
		if ( Velocity.x > sv_maxvelocity ) Velocity = Velocity.WithX( sv_maxvelocity );
		if ( Velocity.x < -sv_maxvelocity ) Velocity = Velocity.WithX( -sv_maxvelocity );

		if ( Velocity.y > sv_maxvelocity ) Velocity = Velocity.WithY( sv_maxvelocity );
		if ( Velocity.y < -sv_maxvelocity ) Velocity = Velocity.WithY( -sv_maxvelocity );

		if ( Velocity.z > sv_maxvelocity ) Velocity = Velocity.WithZ( sv_maxvelocity );
		if ( Velocity.z < -sv_maxvelocity ) Velocity = Velocity.WithZ( -sv_maxvelocity );
	}
}
