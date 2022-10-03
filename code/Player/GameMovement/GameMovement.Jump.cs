public partial class Source1GameMovement
{
	public float JumpTime { get; set; }

	public virtual bool WishJump()
	{
		return Input.Pressed( InputButton.Jump );
	}

	public virtual bool CanJump()
	{
		if ( IsInAir )
			return false;

		if ( IsDucked )
			return false;

		// Yeah why not.
		return true;

	}

	/// <summary>
	/// Returns true if we succesfully made a jump.
	/// </summary>
	/// <returns></returns>
	public virtual bool CheckJumpButton()
	{
		if ( !CheckWaterJumpButton() )
			return false;

		if ( !CanJump() )
			return false;

		ClearGroundEntity();


		AddEvent( "jump" );

		float startz = Velocity.z;
		Velocity = Velocity.WithZ( JumpImpulse );

		if ( IsDucking )
			Velocity = Velocity.WithZ( Velocity.z + startz );

		FinishGravity();
		OnJump( Velocity.z - startz );

		return true;
	}

	public virtual float JumpImpulse => 100;

	public virtual void OnJump( float velocity )
	{

	}
}
