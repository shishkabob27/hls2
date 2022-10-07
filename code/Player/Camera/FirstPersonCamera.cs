public partial class FirstPersonCamera : CameraMode
{
	Vector3 lastPos;
	[ConVar.Client] public static float cl_rollspeed { get; set; } = 200.0f;
	[ConVar.Client] public static float cl_rollangle { get; set; } = 2.0f;
	[ConVar.Client] public static float cl_bob { get; set; } = 0.01f;
	[ConVar.Client] public static float cl_bobcycle { get; set; } = 0.8f;
	[ConVar.Client] public static float cl_bobup { get; set; } = 0.5f;
	[ConVar.Client] public static float cl_vsmoothing { get; set; } = 0f;
	[ConVar.Client] public static float cl_stepsmooth { get; set; } = 1f;
	[ConVar.Client] public static bool hl_won_viewbob { get; set; } = false;
	[ConVar.Client] public static bool hl_fix_mystery_viewbob_code { get; set; } = false;


	const int ORIGIN_BACKUP = 64;
	const int ORIGIN_MASK = (ORIGIN_BACKUP - 1);

	Vector3 lastorg = Vector3.Zero;

	static float oldz = 0;
	float lasttime;


	public override void Activated()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Position = pawn.EyePosition;
		Rotation = pawn.EyeRotation;

		lastPos = Position;
	}

	public override void Update()
	{
		var pawn = Local.Pawn as HLPlayer;
		if ( pawn == null ) return;
		Viewer = pawn;
		if ( pawn.Client.IsUsingVr ) return;
		Vector3 simorg = pawn.EyePosition;
		var eyePos = pawn.EyePosition;

		Position = eyePos;
		var bob = V_CalcBob();
		var a = Position;
		a.z += bob;
		Position = a;

		Rotation = pawn.EyeRotation;

		Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + CalculateRoll( Rotation, pawn.Velocity, cl_rollangle, cl_rollspeed ) ).ToRotation();
		Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + pawn.punchangle.z ).ToRotation();
		Rotation = Rotation.Angles().WithPitch( Rotation.Angles().pitch + pawn.punchangle.x ).ToRotation();
		Rotation = Rotation.Angles().WithYaw( Rotation.Angles().yaw + pawn.punchangle.y ).ToRotation();


		Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + pawn.punchanglecl.z ).ToRotation();
		Rotation = Rotation.Angles().WithPitch( Rotation.Angles().pitch + pawn.punchanglecl.x ).ToRotation();
		Rotation = Rotation.Angles().WithYaw( Rotation.Angles().yaw + pawn.punchanglecl.y ).ToRotation();


		pawn.punchanglecl = pawn.punchanglecl.Approach( 0, Time.Delta * 14.3f ); // was Delta * 10, 14.3 matches hl1 the most
																				 //Log.Info( pawn.punchangle );

		lastPos = Position;


		if ( pawn.ActiveChild is HLWeapon )
		{

			var wep = pawn.ActiveChild as HLWeapon;
			// Weapon position
			if ( wep.ViewModelEntity is Entity )
			{

				wep.ViewModelEntity.Position = Position;
				wep.ViewModelEntity.Rotation = Rotation;
			}
			// Weapon bobbing
			if ( wep.ViewModelEntity is HLViewModel )
			{
				var viewmodel = wep.ViewModelEntity as HLViewModel;
				var b = viewmodel.Position;
				for ( var i = 0; i < 3; i++ )
				{
					b[i] += bob * 0.4f * Rotation.Forward[i];
				}

				if ( hl_fix_mystery_viewbob_code ) b.z += bob; // I don't understand, this is in the hl1 code but hl1 doesnt have this? is it broken in the original? i'll just comment it out...

				// pushing the view origin down off of the same X/Z plane as the ent's origin will give the
				// gun a very nice 'shifting' effect when the player looks up/down. If there is a problem
				// with view model distortion, this may be a cause. (SJB). 
				b.z -= 1;

				viewmodel.Position = b;

				var WepRot = viewmodel.Rotation;
				WepRot = WepRot.Angles().WithYaw( WepRot.Angles().yaw - bob * 0.5f ).ToRotation();
				WepRot = WepRot.Angles().WithRoll( WepRot.Angles().roll - bob * 1f ).ToRotation();
				WepRot = WepRot.Angles().WithPitch( WepRot.Angles().pitch - bob * 0.3f ).ToRotation();
				if ( hl_won_viewbob )
					viewmodel.Rotation = WepRot;
				//wep.angle[YAW] -= bob * 0.5;
				//wep.angle[ROLL] -= bob * 1;
				//wep.angle[PITCH] -= bob * 0.3;

			}
		}
		Vector3 wepchange = Vector3.Zero;
		if ( cl_stepsmooth > 0 && pawn.GroundEntity != null && simorg[2] - oldz > 0 )
		{
			float steptime;

			steptime = Time.Now - lasttime;
			if ( steptime < 0 )
				//FIXME		I_Error ("steptime < 0");
				steptime = 0;

			oldz += steptime * 150;
			if ( oldz > simorg[2] )
				oldz = simorg[2];
			if ( simorg[2] - oldz > 18 )
				oldz = simorg[2] - 18;
			if ( pawn.ActiveChild is HLWeapon )
			{

				var wep = pawn.ActiveChild as HLWeapon;
				// Weapon position
				if ( wep.ViewModelEntity is Entity )
				{

					wep.ViewModelEntity.Position = wep.ViewModelEntity.Position.WithZ( wep.ViewModelEntity.Position.z + oldz - simorg[2] );
				}
			}
			Position = Position.WithZ( Position.z + oldz - simorg[2] );
		}
		else
		{
			oldz = simorg[2];
		}

		Vector3 delta2;

		//VectorSubtract( pparams->simorg, lastorg, delta );

		delta2 = simorg - lastorg;

		if ( delta2.Length != 0.0 )
		{
			ViewInterp.Origins[ViewInterp.CurrentOrigin & ORIGIN_MASK] = simorg;
			ViewInterp.OriginTime[ViewInterp.CurrentOrigin & ORIGIN_MASK] = Time.Now;
			ViewInterp.CurrentOrigin++;

			//VectorCopy( pparams->simorg, lastorg );
			lastorg = simorg;
		}
		if ( cl_vsmoothing != 0 )
		{
			int foundidx = 0;
			int i;
			float t;

			if ( cl_vsmoothing < 0.0 )
			{
				cl_vsmoothing = 0.0f;
			}

			t = Time.Now - cl_vsmoothing;

			for ( i = 1; i < ORIGIN_MASK; i++ )
			{
				foundidx = ViewInterp.CurrentOrigin - 1 - i;
				if ( ViewInterp.OriginTime[foundidx & ORIGIN_MASK] <= t )
					break;
			}

			if ( i < ORIGIN_MASK && ViewInterp.OriginTime[foundidx & ORIGIN_MASK] != 0.0 )
			{
				// Interpolate
				Vector3 delta;
				float frac;
				float dt;
				Vector3 neworg = Vector3.Zero;

				dt = ViewInterp.OriginTime[(foundidx + 1) & ORIGIN_MASK] - ViewInterp.OriginTime[foundidx & ORIGIN_MASK];
				if ( dt > 0.0 )
				{
					frac = (t - ViewInterp.OriginTime[foundidx & ORIGIN_MASK]) / dt;
					frac = Math.Min( 1.0f, frac );
					//VectorSubtract( ViewInterp.Origins[(foundidx + 1) & ORIGIN_MASK], ViewInterp.Origins[foundidx & ORIGIN_MASK], delta );
					delta = ViewInterp.Origins[(foundidx + 1) & ORIGIN_MASK] - ViewInterp.Origins[(foundidx + 1) & ORIGIN_MASK];
					neworg = VectorMA( ViewInterp.Origins[foundidx & ORIGIN_MASK], frac, delta );

					// Dont interpolate large changes
					if ( delta.Length < 64 )
					{
						//VectorSubtract( neworg, pparams->simorg, delta );
						delta = neworg - simorg;

						//VectorAdd( pparams->simorg, delta, pparams->simorg );
						simorg += delta;
						//VectorAdd( pparams->vieworg, delta, pparams->vieworg );
						//viewmodel.Position += delta;
						if ( pawn.ActiveChild is HLWeapon )
						{

							var wep = pawn.ActiveChild as HLWeapon;
							// Weapon position
							if ( wep.ViewModelEntity is Entity )
							{

								wep.ViewModelEntity.Position += delta;
							}
						}
						//VectorAdd( view->origin, delta, view->origin );
						Position += delta;

					}
				}
			}
		}
		if ( pawn.ActiveChild is HLWeapon )
		{

			var wep = pawn.ActiveChild as HLWeapon;
			// Weapon position
			if ( wep.ViewModelEntity is Entity )
			{

				wep.ViewModelEntity.Position += wepchange;
			}
		}
		V_CalcShake();

		lasttime = Time.Now;
	}
	public virtual float CalculateRoll( Rotation angles, Vector3 velocity, float rollangle, float rollspeed )
	{
		if ( !HLGame.hl_viewroll ) return 0.0f;
		float sign;
		float side;
		float value;
		//QAngle a = angles; //.AngleVectors(out var forward, out var right, out var up);
		//a.AngleVectors(out var forward, out var right, out var up);
		var forward = angles.Forward;
		var right = angles.Right;
		var up = angles.Up;

		side = velocity.Dot( right );

		sign = side < 0 ? -1 : 1;

		side = Math.Abs( side );

		value = rollangle;

		if ( side < rollspeed )
		{
			side = side * value / rollspeed;
		}
		else
		{
			side = value;
		}

		return side * sign;
	}
	double bobtime;
	float bob;
	float bobcycle;
	float lasttimebob;
	float V_CalcBob()
	{
		Vector3 vel;
		if ( Local.Pawn is not HLPlayer player ) return 0;

		if ( player.GroundEntity == null || Time.Now == lasttimebob )
		{
			// just use old value
			return bob;
		}

		lasttimebob = Time.Now;

		bobtime += Time.Delta;
		bobcycle = (float)(bobtime - (int)(bobtime / cl_bobcycle) * cl_bobcycle);
		bobcycle /= cl_bobcycle;

		if ( bobcycle < cl_bobup )
		{
			bobcycle = (float)Math.PI * bobcycle / cl_bobup;
		}
		else
		{
			bobcycle = (float)Math.PI + (float)Math.PI * (bobcycle - cl_bobup) / (1.0f - cl_bobup);
		}

		// bob is proportional to simulated velocity in the xy plane
		// (don't count Z, or jumping messes it up)
		//VectorCopy( pparams->simvel, vel );
		vel = player.Velocity.WithZ( 0 );
		//vel[2] = 0;

		bob = (float)Math.Sqrt( vel[0] * vel[0] + vel[1] * vel[1] ) * cl_bob;
		bob = bob * 0.3f + bob * 0.7f * (float)Math.Sin( bobcycle );
		bob = Math.Min( bob, 4 );
		bob = Math.Max( bob, -7 );
		return bob;

	}
	Vector3 VectorMA( Vector3 va, float scale, Vector3 vb )
	{
		Vector3 vc = Vector3.Zero;
		vc[0] = va[0] + scale * vb[0];
		vc[1] = va[1] + scale * vb[1];
		vc[2] = va[2] + scale * vb[2];
		return vc;
	}
	public float Shake_ENDTIME { get; set; } = 0;
	public float Shake_DURATION { get; set; } = 0;
	public float Shake_NEXTSHAKE { get; set; } = 0;
	public float Shake_AMPLITUDE { get; set; } = 0;
	public float Shake_FREQUENCY { get; set; } = 0;
	public Vector3 Shake_OFFSET { get; set; }
	public float Shake_ANGLE { get; set; }
	void V_CalcShake()
	{
		float shakeFraction;
		float shakeFrequency;

		if ( (Time.Now > Shake_ENDTIME) || Shake_AMPLITUDE <= 0 || Shake_FREQUENCY <= 0 || Shake_ENDTIME <= 0 )
			return;

		if ( Time.Now > Shake_NEXTSHAKE )
		{
			Shake_NEXTSHAKE = Time.Now + (1.0f / Shake_FREQUENCY);
			var a = Shake_OFFSET;
			for ( int i = 0; i < 3; i++ )
			{
				a[i] = Rand.Float( Shake_AMPLITUDE * -1, Shake_AMPLITUDE );
			}
			Shake_OFFSET = a;
			Shake_ANGLE = Rand.Float( Shake_AMPLITUDE * -0.25f, Shake_AMPLITUDE * 0.25f );
		}

		shakeFraction = (Shake_ENDTIME - Time.Now) / Shake_DURATION;

		// Ramp up
		if ( shakeFraction != 0 )
		{
			shakeFrequency = (Shake_FREQUENCY / shakeFraction);
		}
		else
		{
			shakeFrequency = 0;
		}

		// Settle
		shakeFraction *= shakeFraction;
		float angle = Time.Now * shakeFrequency;
		if ( angle > 1e8 )
		{
			angle = 1e8F;
		}
		shakeFraction *= MathF.Sin( angle );//Time.Now * shakeFrequency

		// Apply
		Position += Shake_OFFSET * shakeFraction;
		Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + Shake_ANGLE * shakeFraction ).ToRotation();

		// Lower the shake amplitute over time
		Shake_AMPLITUDE -= Shake_AMPLITUDE * (Time.Delta / (Shake_DURATION * Shake_FREQUENCY));

		if ( Global.IsRunningInVR && shakeFraction != 0 )
		{
			Input.VR.RightHand.TriggerHapticVibration( Shake_DURATION, shakeFrequency, shakeFraction );
			Input.VR.LeftHand.TriggerHapticVibration( Shake_DURATION, shakeFrequency, shakeFraction );
		}
		//Input.TriggerHapticVibration( Shake_DURATION, shakeFrequency, shakeFraction ); cant rumble controllers what the?

	}

}
public static class ViewInterp
{


	const int ORIGIN_BACKUP = 64;
	const int ORIGIN_MASK = (ORIGIN_BACKUP - 1);
	public static Vector3[] Origins = new Vector3[ORIGIN_BACKUP];
	public static float[] OriginTime = new float[ORIGIN_BACKUP];

	public static Vector3[] Angles = new Vector3[ORIGIN_BACKUP];
	public static float[] AngleTime = new float[ORIGIN_BACKUP];

	public static int CurrentOrigin = 0;
	public static int CurrentAngle = 0;

}
