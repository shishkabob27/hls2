public partial class HL1GameMovement
{
	public float JumpTime { get; set; }


	public virtual bool WishJump()
	{
		return (sv_autojump) ? Input.Down( InputButton.Jump ) | Input.VR.RightHand.ButtonA.IsPressed : Input.Pressed( InputButton.Jump ) | Input.VR.RightHand.ButtonA.WasPressed;
	}

	public virtual bool CanJump()
	{
		if ( IsInAir )
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

		if ( !sv_enablebunnyhopping )
			PreventBunnyJumping();
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

	public virtual float JumpImpulse => 268;

	public virtual void OnJump( float velocity )
	{

	}
	public virtual void PreventBunnyJumping()
	{
		// Speed at which bunny jumping is limited
		float maxscaledspeed = sv_maxspeed;
		if ( maxscaledspeed <= 0.0f )
			return;

		// Current player speed
		float spd = Velocity.Length;
		if ( spd <= maxscaledspeed )
			return;

		// Apply this cropping fraction to velocity
		float fraction = (maxscaledspeed / spd);

		Velocity *= fraction;
	}
}
