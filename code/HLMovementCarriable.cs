class HLMovementCarriable : BaseCarriable
{

	private Vector3 mins = Vector3.Zero;
	private Vector3 maxs = Vector3.Zero;

	public Vector3 minsOverride;
	public Vector3 maxsOverride;

	public new Vector3 AngularVelocity;
	protected float SurfaceFriction;

	// Config
	public float bGirth = 1 * 0.8f;
	public float bHeight = 1;


	[ConVar.Replicated] public static float sv_gravity { get; set; } = 800;
	[ConVar.Replicated] public static float sv_friction { get; set; } = 4;
	[ConVar.Replicated] public static float sv_stopspeed { get; set; } = 100;
	public float Friction { get; set; } = 1.0f;
	public float GroundBounce { get; set; } = 0.1f;
	public float WallBounce { get; set; } = 0.1f;
	public float GroundAngle { get; set; } = 46.0f;
	public float Gravity { get; set; } = 1.0f;
	public bool DontSleep = false;

	Entity lastTouch;
	Vector3 lastHitNormal;

	[Event.Tick.Server]
	public void Tick()
	{
		Simulate();
	}

	public void Simulate()
	{
		if ( Owner is HLPlayer ) return; // Don't do physics if we are being carried
		if ( HLUtils.PlayerInRangeOf( Position, 2048 ) == false && !DontSleep )
			return;
		try
		{
			CalcGroundEnt();
			ApplyGravity();
			ApplyFriction( sv_friction * SurfaceFriction );
			ApplyAngularFriction( sv_friction * SurfaceFriction );
			Move();
		}
		catch
		{
		}
	}

	public void Move()
	{
		mins = new Vector3( -bGirth, -bGirth, 0 );
		maxs = new Vector3( +bGirth, +bGirth, bHeight );
		if ( minsOverride != Vector3.Zero )
		{
			mins = minsOverride;
		}
		if ( maxsOverride != Vector3.Zero )
		{
			maxs = maxsOverride;
		}

		if ( !HL1GameMovement.sv_use_sbox_movehelper )
		{
			//Move2();
			return;
		}

		NewMoveHelper mover = new( Position, Velocity );

		mover.Trace = mover.Trace
			.Size( mins, maxs )
			.Ignore( this )
			.WithoutTags( "player" );
		mover.GroundBounce = GroundBounce;
		mover.WallBounce = WallBounce;
		mover.TryMove( Time.Delta );
		if ( mover.HitWall || mover.HitFloor )
		{
			if ( mover.TraceResult.Normal != lastHitNormal )
			{
				this.Touch( this );
			}
			lastHitNormal = mover.TraceResult.Normal;
		}

		lastTouch = mover.TraceResult.Entity;
		Position = mover.Position;
		Velocity = mover.Velocity;
		Rotation = (Rotation.Angles() + (new Angles( AngularVelocity.x, AngularVelocity.y, AngularVelocity.z ) * Time.Delta)).ToRotation();
	}
	public void ApplyGravity()
	{
		Velocity -= new Vector3( 0, 0, (sv_gravity * Gravity) * 0.5f ) * Time.Delta;
		Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;

		BaseVelocity = BaseVelocity.WithZ( 0 );
	}
	public void CalcGroundEnt()
	{


		mins = new Vector3( -bGirth, -bGirth, 0 );
		maxs = new Vector3( +bGirth, +bGirth, bHeight );
		if ( minsOverride != Vector3.Zero )
		{
			mins = minsOverride;
		}
		if ( maxsOverride != Vector3.Zero )
		{
			maxs = maxsOverride;
		}
		SurfaceFriction = 1.0f;
		var point = Position - Vector3.Up * 2;
		var vBumpOrigin = Position;
		//if ( GroundEntity != null ) // and not underwater
		//{
		//bMoveToEndPos = true;
		//point.z -= 18;
		//}


		var pm = TraceBBox( vBumpOrigin, point, mins, maxs, 4.0f );

		if ( pm.Entity == null || Vector3.GetAngle( Vector3.Up, pm.Normal ) > GroundAngle )
		{
			ClearGroundEntity();
			if ( Velocity.z > 0 )
			{
				SurfaceFriction = 0.25f;
			}
		}
		else
		{
			UpdateGroundEntity( pm );
		}

	}
	public Vector3 TraceOffset = 0;

	/// <summary>
	/// Traces the bbox and returns the trace result.
	/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var tr = Trace.Ray( start + TraceOffset, end + TraceOffset )
					.Size( mins, maxs )
					.WithAnyTags( "solid" )
					.Ignore( this )
					.Run();

		tr.EndPosition -= TraceOffset;
		return tr;
	}
	/// <summary>
	/// We have a new ground entity
	/// </summary>
	public void UpdateGroundEntity( TraceResult tr )
	{
		var GroundNormal = tr.Normal;

		// VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
		// A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
		// This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
		SurfaceFriction = tr.Surface.Friction * 1.25f;
		if ( SurfaceFriction > 1 ) SurfaceFriction = 1;

		//if ( tr.Entity == GroundEntity ) return;

		Vector3 oldGroundVelocity = default;
		if ( GroundEntity != null ) oldGroundVelocity = GroundEntity.Velocity;

		bool wasOffGround = GroundEntity == null;

		GroundEntity = tr.Entity;

		if ( GroundEntity != null )
		{
			BaseVelocity = GroundEntity.Velocity;
		}
		if ( wasOffGround )
		{

			//this.StartTouch(this);
		}

	}

	/// <summary>
	/// We're no longer on the ground, remove it
	/// </summary>
	public void ClearGroundEntity()
	{

		if ( GroundEntity == null ) return;
		this.EndTouch( this );
		GroundEntity = null;
		var GroundNormal = Vector3.Up;
		SurfaceFriction = 1.0f;
	}

	public void ApplyFriction( float frictionAmount = 1.0f )
	{
		// If we are in water jump cycle, don't apply friction
		//if ( player->m_flWaterJumpTime )
		//   return;

		// Not on ground - no friction
		if ( GroundEntity == null )
			return;
		frictionAmount += (Friction - 1);

		// Calculate speed
		var speed = Velocity.Length;
		if ( speed < 0.1f ) return;

		// Bleed off some speed, but if we have less than the bleed
		//  threshold, bleed the threshold amount.
		float control = (speed < sv_stopspeed) ? sv_stopspeed : speed;

		// Add the amount to the drop amount.
		var drop = control * Time.Delta * frictionAmount;

		// scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			Velocity *= newspeed;
		}

		// mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
	}
	public void ApplyAngularFriction( float frictionAmount = 1.0f )
	{
		// If we are in water jump cycle, don't apply friction
		//if ( player->m_flWaterJumpTime )
		//   return;

		// Not on ground - no friction
		if ( GroundEntity == null )
			return;
		frictionAmount += (Friction - 1);

		// Calculate speed
		var speed = AngularVelocity.Length;
		if ( speed < 0.1f ) return;

		// Bleed off some speed, but if we have less than the bleed
		//  threshold, bleed the threshold amount.
		float control = (speed < sv_stopspeed) ? sv_stopspeed : speed;

		// Add the amount to the drop amount.
		var drop = control * Time.Delta * frictionAmount;

		// scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			AngularVelocity *= newspeed;
		}

		// mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
	}
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
		Move();

		// Down results.
		var vecDownPos = Position;
		var vecDownVel = Velocity;

		// Reset original values.
		Position = vecPos;
		Velocity = vecVel;

		// Move up a stair height.
		vecEndPos = Position;
		vecEndPos.z += HL1GameMovement.sv_stepsize + DIST_EPSILON;

		var trace = TraceBBox( Position, vecEndPos, mins, maxs );
		Position = trace.EndPosition;

		// Slide move up.
		Move();

		// Move down a stair (attempt to).
		vecEndPos = Position;
		vecEndPos.z -= HL1GameMovement.sv_stepsize + DIST_EPSILON;

		trace = TraceBBox( Position, vecEndPos, mins, maxs );

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
			Velocity = Velocity.WithZ( vecDownVel.z );
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
			pm = TraceBBox( Position, end, mins, maxs );

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
					var stuck = TraceBBox( pm.EndPosition, pm.EndPosition, mins, maxs );
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
				GroundEntity == null )
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
						ClipVelocity( original_velocity, planes[i], out new_velocity, 1 + HL1GameMovement.sv_bounce * (1 - SurfaceFriction) );
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
						out var a,
						1 );
					Velocity = a;
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
