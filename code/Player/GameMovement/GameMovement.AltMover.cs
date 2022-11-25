public partial class HL1GameMovement
{
	public const int MAX_CLIP_PLANES = 5;
	public const float DIST_EPSILON = 0.3125f;
	public virtual void StepMove2()
	{
		var vecEndPos = Position + Velocity * Time.Delta;

		// Try sliding forward both on ground and up 16 pixels
		//  take the move that goes farthest
		var vecPos = Position;
		var vecVel = Velocity;

		// Slide move down.
		TryPlayerMove();

		// Down results.
		var vecDownPos = Position;
		var vecDownVel = Velocity;

		// Reset original values.
		Position = vecPos;
		Velocity = vecVel;

		// Move up a stair height.
		vecEndPos = Position;
		vecEndPos.z += sv_stepsize + DIST_EPSILON;

		var trace = TraceBBox( Position, vecEndPos );
		Position = trace.EndPosition;

		// Slide move up.
		TryPlayerMove();

		// Move down a stair (attempt to).
		vecEndPos = Position;
		vecEndPos.z -= sv_stepsize + DIST_EPSILON;

		trace = SetupBBoxTrace( Position, vecEndPos ).Ignore( Pawn ).Run();

		// If we are not on the ground any more then use the original movement attempt.
		if ( trace.Normal.z < 0.7f )
		{
			Position = vecDownPos;
			Velocity = vecDownVel;
			return;
		}

		// If the trace ended up in empty space, copy the end over to the origin.
		if ( !trace.StartedSolid /* && !trace.allsolid */)
		{
			Position = trace.EndPosition;
		}

		// Copy this origin to up.
		var vecUpPos = Position;

		// decide which one went farther
		float flDownDist = (vecDownPos.x - vecPos.x) * (vecDownPos.x - vecPos.x) + (vecDownPos.y - vecPos.y) * (vecDownPos.y - vecPos.y);
		float flUpDist = (vecUpPos.x - vecPos.x) * (vecUpPos.x - vecPos.x) + (vecUpPos.y - vecPos.y) * (vecUpPos.y - vecPos.y);
		if ( flDownDist > flUpDist )
		{
			Position = vecDownPos;
			Velocity = vecDownVel;
		}
		else
		{
			// copy z value from slide move
			var a = Velocity;
			a.z = vecDownVel.z;
			Velocity = a;
		}
	}
	public int Move2()
	{
		int bumpcount, numbumps;
		Vector3 dir;
		float d;
		int numplanes;
		var planes = new Vector3[MAX_CLIP_PLANES];
		Vector3 primal_velocity, original_velocity;
		Vector3 new_velocity;
		int i, j;
		TraceResult pm;
		Vector3 end;
		float time_left, allFraction;
		int blocked;

		numbumps = 4;

		blocked = 0;
		numplanes = 0;

		original_velocity = Velocity;
		primal_velocity = Velocity;

		allFraction = 0;
		time_left = Time.Delta;

		new_velocity = 0;

		for ( bumpcount = 0; bumpcount < numbumps; bumpcount++ )
		{
			if ( Velocity.Length == 0 )
				break;

			// Assume we can move all the way from the current origin to the
			// end point.
			end = Position + Velocity * time_left;
			pm = SetupBBoxTrace( Position, end ).Ignore( Pawn ).Run();

			allFraction += pm.Fraction;

			// If we started in a solid object, or we were in solid space
			//  the whole way, zero out our velocity and return that we
			//  are blocked by floor and wall.
			if ( pm.StartedSolid )
			{
				Velocity = 0;
				// entity is trapped in another solid
				return 4;
			}

			// If we moved some portion of the total distance, then
			//  copy the end position into the pmove.origin and 
			//  zero the plane counter.
			if ( pm.Fraction > 0 )
			{
				if ( numbumps > 0 && pm.Fraction == 1 )
				{
					// There's a precision issue with terrain tracing that can cause a swept box to successfully trace
					// when the end position is stuck in the triangle.  Re-run the test with an uswept box to catch that
					// case until the bug is fixed.
					// If we detect getting stuck, don't allow the movement
					var stuck = TraceBBox( pm.EndPosition, pm.EndPosition );
					if ( stuck.StartedSolid || stuck.Fraction != 1.0f )
					{
						// Log.Info( "Player will become stuck!!!\n" );
						Velocity = 0;
						break;
					}

					// actually covered some distance
					Position = pm.EndPosition;
					original_velocity = Velocity;
					numplanes = 0;
				}
			}

			// If we covered the entire distance, we are done
			//  and can return.
			if ( pm.Fraction == 1 )
				break; // moved the entire distance

			if ( pm.Normal.z > 0.7f )
				blocked |= 1;   // floor

			if ( pm.Normal.z == 0 )
				blocked |= 2;   // step / wall

			time_left -= time_left * pm.Fraction;

			// Did we run out of planes to clip against?
			if ( numplanes >= MAX_CLIP_PLANES )
			{
				// this shouldn't really happen
				// Stop our movement if so.
				Velocity = 0;
				break;
			}

			planes[numplanes] = pm.Normal;
			numplanes++;

			// reflect player velocity 
			// Only give this a try for first impact plane because you can get yourself stuck in an acute corner by jumping in place
			//  and pressing forward and nobody was really using this bounce/reflection feature anyway...
			if ( numplanes == 1 &&
				!Player.IsNoclipping &&
				!Player.IsObserver &&
				Player.GroundEntity == null )
			{
				for ( i = 0; i < numplanes; i++ )
				{
					if ( planes[i][2] > 0.7f )
					{
						ClipVelocity( original_velocity, planes[i], out new_velocity, 1 );
						original_velocity = new_velocity;
					}
					else
					{
						ClipVelocity( original_velocity, planes[i], out new_velocity, 1 + sv_bounce * (1 - Player.SurfaceFriction) );
					}
				}

				Velocity = new_velocity;
				original_velocity = new_velocity;
			}
			else
			{
				for ( i = 0; i < numplanes; i++ )
				{
					ClipVelocity(
						original_velocity,
						planes[i],
						out var new_velocity2,
						1 );
					Velocity = new_velocity2;
					for ( j = 0; j < numplanes; j++ )
					{
						if ( j != i )
						{
							// Are we now moving against this plane?
							if ( Vector3.Dot( Velocity, planes[j] ) < 0 )
								break;  // not ok
						}
					}

					if ( j == numplanes ) // Didn't have to clip, so we're ok
						break;
				}

				if ( i == numplanes )
				{
					// go along the crease
					if ( numplanes != 2 )
					{
						Velocity = 0;
						break;
					}

					dir = Vector3.Cross( planes[0], planes[1] );
					dir = dir.Normal;
					d = Vector3.Dot( dir, Velocity );
					Velocity = d * dir;
				}

				//
				// if original velocity is against the original velocity, stop dead
				// to avoid tiny occilations in sloping corners
				//
				d = Vector3.Dot( Velocity, primal_velocity );
				if ( d <= 0 )
				{
					//Con_DPrintf("Back\n");
					Velocity = 0;
					break;
				}
			}
		}

		if ( allFraction == 0 )
			Velocity = 0;

		/*
		// Check if they slammed into a wall
		float fSlamVol = 0.0f;
		float fLateralStoppingAmount = primal_velocity.Length2D() - mv->m_vecVelocity.Length2D();
		if ( fLateralStoppingAmount > PLAYER_MAX_SAFE_FALL_SPEED * 2.0f )
		{
			fSlamVol = 1.0f;
		}
		else if ( fLateralStoppingAmount > PLAYER_MAX_SAFE_FALL_SPEED )
		{
			fSlamVol = 0.85f;
		}
		PlayerRoughLandingEffects( fSlamVol );
		*/

		return blocked;
	}
	public int ClipVelocity( Vector3 vIn, Vector3 normal, out Vector3 vOut, float overbounce )
	{
		var angle = normal.z;

		var blocked = 0x00;
		if ( angle > 0 )
			blocked |= 0x01;
		if ( angle == 0 )
			blocked |= 0x02;

		var backoff = Vector3.Dot( vIn, normal ) * overbounce;

		vOut = 0;
		for ( var i = 0; i < 3; i++ )
		{
			var change = normal[i] * backoff;
			vOut[i] = vIn[i] - change;
		}

		var adjust = Vector3.Dot( vOut, normal );
		if ( adjust < 0 )
		{
			vOut -= normal * adjust;
		}

		return blocked;
	}
}
