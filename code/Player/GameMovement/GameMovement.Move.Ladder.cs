partial class HL1GameMovement
{
	protected Vector3 LadderNormal { get; set; }
	public virtual float LadderDistance => 2;
	public virtual float ClimbSpeed => 200;

	public virtual void FullLadderMove()
	{
		CheckWater();

		if ( WishJump() )
		{
			CheckJumpButton();
		}

		Velocity -= BaseVelocity;
		TryPlayerMove();
		Velocity += BaseVelocity;
	}

	public virtual bool LadderMove()
	{
		if ( Player.IsNoclipping )
			return false;

		if ( !GameHasLadders )
			return false;

		Vector3 wishdir;

		// If I'm already moving on a ladder, use the previous ladder direction
		if ( Player.IsOnLadder )
		{
			wishdir = -LadderNormal;
		}
		else
		{
			// otherwise, use the direction player is attempting to move
			if ( ForwardMove != 0 || RightMove != 0 )
			{
				wishdir = Player.ViewAngles.ToRotation().Forward * ForwardMove + Player.ViewAngles.ToRotation().Right * RightMove;
				wishdir = wishdir.Normal;
			}
			else
			{
				// Player is not attempting to move, no ladder behavior
				return false;
			}
		}

		// wishdir points toward the ladder if any exists
		var end = Position + wishdir * LadderDistance;

		var pm = SetupBBoxTrace( Position, end )
			.WithTag( "ladder" )
			.Run();

		if ( pm.Fraction == 1 )
			return false;

		Player.IsOnLadder = true;
		LadderNormal = pm.Normal;
		// On ladder, convert movement to be relative to the ladder

		var floor = Position;
		floor.z += GetPlayerMins().z - 1;

		float climbSpeed = ClimbSpeed;

		float forwardSpeed = 0, rightSpeed = 0;
		if ( Input.Down( "Backward" ) )
			forwardSpeed -= climbSpeed;

		if ( Input.Down( "Forward" ) )
			forwardSpeed += climbSpeed;

		if ( Input.Down( "Left" ) )
			rightSpeed -= climbSpeed;

		if ( Input.Down( "Right" ) )
			rightSpeed += climbSpeed;

		if ( Input.Down( "Jump" ) )
		{
			//Player.MoveType = MoveType.MOVETYPE_WALK;
			// player->SetMoveCollide( MOVECOLLIDE_DEFAULT );

			Velocity = pm.Normal * 270;
		}
		else
		{
			if ( forwardSpeed != 0 || rightSpeed != 0 )
			{
				var velocity = Player.ViewAngles.Forward * forwardSpeed;
				velocity += Right * rightSpeed;

				Vector3 temp = Vector3.Up;
				var perp = Vector3.Cross( temp, pm.Normal );
				perp = perp.Normal;

				// decompose velocity into ladder plane
				float normal = Vector3.Dot( velocity, pm.Normal );

				// This is the velocity into the face of the ladder
				var cross = pm.Normal * normal;

				// This is the player's additional velocity
				var lateral = velocity - cross;

				// This turns the velocity into the face of the ladder into velocity that
				// is roughly vertically perpendicular to the face of the ladder.
				// NOTE: It IS possible to face up and move down or face down and move up
				// because the velocity is a sum of the directional velocity and the converted
				// velocity through the face of the ladder -- by design.
				temp = Vector3.Cross( pm.Normal, perp );

				Velocity = lateral + temp * -normal;

				// On ground moving away from the ladder
				if ( IsGrounded && normal > 0 )
				{
					Velocity += pm.Normal * ClimbSpeed;
				}
			}
			else
			{
				Velocity = 0;
			}
		}
		return true;
	}

	public virtual bool GameHasLadders => true;
}
