
public partial class HL1GameMovement
{
	public virtual void FullNoClipMove( float factor, float maxacceleration )
	{
		float maxspeed = sv_maxspeed * factor;

		WishVelocity = Forward * ForwardMove + Right * RightMove;
		WishVelocity = WishVelocity.WithZ( WishVelocity.z + UpMove * factor );
		WishVelocity *= factor;

		var wishdir = WishVelocity.Normal;
		var wishspeed = WishVelocity.Length;

		//
		// Clamp to server defined max speed
		//
		if ( wishspeed > maxspeed )
		{
			WishVelocity *= maxspeed / wishspeed;
			wishspeed = maxspeed;
		}

		if ( maxacceleration > 0.0 )
		{
			// Set pmove velocity
			Accelerate( wishdir, wishspeed, maxacceleration );
			if ( Velocity.Length < maxspeed + 10 )
			{ 
				StayOnGround();
			}

			float spd = Velocity.Length;
			if ( spd < 1 )
			{
				Velocity = 0;
				return;
			}

			// Bleed off some speed, but if we have less than the bleed
			//  threshhold, bleed the theshold amount.
			float control = (spd < maxspeed / 4) ? (maxspeed / 4) : spd;

			float friction = sv_friction * Player.SurfaceFriction;

			// Add the amount to the drop amount.
			float drop = control * friction * Time.Delta;

			// scale the velocity
			float newspeed = spd - drop;
			if ( newspeed < 0 )
				newspeed = 0;

			// Determine proportion of old speed we are using.
			newspeed /= spd;
			Velocity *= newspeed;
		}
		else
		{
			Velocity = WishVelocity;
		}

		// Just move
		Position += Time.Delta * Velocity;

		// Zero out velocity if in noaccel mode
		if ( maxacceleration < 0.0f )
		{
			Velocity = 0;
		}
	}

	public virtual void FullObserverMove()
	{
		/*
		var mode = Player.ObserverMode;

		if ( mode == ObserverMode.InEye || mode == ObserverMode.Chase )
		{
			var target = Player.ObserverTarget;
			if ( target != null )
			{
				Position = target.Position;
				Rotation = target.Rotation;
				Velocity = target.Velocity;
			}

			return;
		}*/

		if ( true )
			// don't move in fixed or death cam mode
			return;

		if ( sv_spectator_noclip )
		{
			// roam in noclip mode
			FullNoClipMove( sv_spectator_speed, sv_spectator_accelerate );
			return;
		}

		// Copy movement amounts

		float factor = sv_spectator_speed;
		if ( Input.Down( InputButton.Run ) )
		{
			factor /= 2.0f;
		}

		float fmove = ForwardMove * factor;
		float smove = RightMove * factor;

		WishVelocity = Forward * fmove + Right * smove;
		WishVelocity = WishVelocity.WithZ( WishVelocity.z + UpMove );

		var wishdir = WishVelocity.Normal;
		var wishspeed = WishVelocity.Length;

		//
		// Clamp to server defined max speed
		//

		float maxspeed = sv_maxvelocity;

		if ( wishspeed > maxspeed )
		{
			WishVelocity *= MaxSpeed / wishspeed;
			wishspeed = maxspeed;
		}

		// Set pmove velocity, give observer 50% acceration bonus
		Accelerate( wishdir, wishspeed, sv_spectator_accelerate );

		float spd = Velocity.Length;
		if ( spd < 1.0f )
		{
			Velocity = 0;
			return;
		}

		float friction = sv_friction;

		// Add the amount to the drop amount.
		float drop = spd * friction * Time.Delta;

		// scale the velocity
		float newspeed = spd - drop;

		if ( newspeed < 0 )
			newspeed = 0;

		// Determine proportion of old speed we are using.
		newspeed /= spd;

		Velocity *= newspeed;
		CheckVelocity();

		TryPlayerMove();
	}
}
