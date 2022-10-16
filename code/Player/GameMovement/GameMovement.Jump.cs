public partial class HL1GameMovement
{
	public float JumpTime { get; set; }


	public virtual bool WishJump()
	{
		return (sv_autojump) ? Input.Down( InputButton.Jump ) | Input.VR.RightHand.ButtonA.IsPressed : Input.Pressed( InputButton.Jump ) | Input.VR.RightHand.ButtonA.WasPressed;
	}

	public virtual bool CanJump()
	{

		if ( sv_wings )
			return true;

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

		if ( sv_enablebackhopping )
			AllowAcceleratedBackHopping();

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
	public virtual void AllowAcceleratedBackHopping()
	{
		// We give a certain percentage of the current forward movement as a bonus to the jump speed.  That bonus is clipped
		// to not accumulate over time.
		var forward = Rotation.Forward.WithZ( 0 ).Normal;
		//float flSpeedBoostPerc = (!pMoveData->m_bIsSprinting && !player->m_Local.m_bDucked) ? 0.5f : 0.1f;
		float flSpeedBoostPerc = (!Input.Down(InputButton.Run) && !IsDucking) ? 0.5f : 0.1f;
		float flSpeedAddition = MathF.Abs( ForwardMove * flSpeedBoostPerc );
		float flMaxSpeed = sv_maxspeed + (sv_maxspeed * flSpeedBoostPerc);
		float flNewSpeed = (flSpeedAddition + Velocity.Length);

		// If we're over the maximum, we want to only boost as much as will get us to the goal speed
		if ( flNewSpeed > flMaxSpeed )
		{
			flSpeedAddition -= flNewSpeed - flMaxSpeed;
		}

		if ( ForwardMove < 0.0f )
			flSpeedAddition *= -1.0f;
		// Add it on
		Velocity = Velocity + (forward * flSpeedAddition);

	}
}
