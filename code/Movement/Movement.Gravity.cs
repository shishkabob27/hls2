
public partial class Movement
{
	public virtual void StartGravity()
	{
		Entity.Velocity -= new Vector3( 0, 0, GetCurrentGravity() * 0.5f ) * Time.Delta;
		Entity.Velocity += new Vector3( 0, 0, Entity.BaseVelocity.z ) * Time.Delta;

		Entity.BaseVelocity = Entity.BaseVelocity.WithZ( 0 );

		CheckVelocity();
	}

	public virtual void FinishGravity()
	{
		Entity.Velocity -= new Vector3( 0, 0, GetCurrentGravity() * 0.5f ) * Time.Delta;
	}

	public virtual float GetCurrentGravity()
	{
		return HL1GameMovement.sv_gravity; // * GameRules.Current.GetGravityMultiplier();
	}

	public void CheckVelocity()
	{
		if ( Entity.Velocity.x > HL1GameMovement.sv_maxvelocity ) Entity.Velocity = Entity.Velocity.WithX( HL1GameMovement.sv_maxvelocity );
		if ( Entity.Velocity.x < -HL1GameMovement.sv_maxvelocity ) Entity.Velocity = Entity.Velocity.WithX( -HL1GameMovement.sv_maxvelocity );

		if ( Entity.Velocity.y > HL1GameMovement.sv_maxvelocity ) Entity.Velocity = Entity.Velocity.WithY( HL1GameMovement.sv_maxvelocity );
		if ( Entity.Velocity.y < -HL1GameMovement.sv_maxvelocity ) Entity.Velocity = Entity.Velocity.WithY( -HL1GameMovement.sv_maxvelocity );

		if ( Entity.Velocity.z > HL1GameMovement.sv_maxvelocity ) Entity.Velocity = Entity.Velocity.WithZ( HL1GameMovement.sv_maxvelocity );
		if ( Entity.Velocity.z < -HL1GameMovement.sv_maxvelocity ) Entity.Velocity = Entity.Velocity.WithZ( -HL1GameMovement.sv_maxvelocity );
	}
}
