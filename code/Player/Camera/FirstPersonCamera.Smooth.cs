partial class FirstPersonCamera
{

	[ConVar.Client] public static float cl_vsmoothing { get; set; } = 0f;
	[ConVar.Client] public static float cl_stepsmooth { get; set; } = 1f;

	static float oldz = 0;
	float lasttime;
	void StairSmooth(Vector3 simorg)
	{
		if ( Local.Pawn is not HLPlayer pawn ) return;

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
			if ( pawn.ActiveChild is Weapon )
			{

				var wep = pawn.ActiveChild as Weapon;
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

		lasttime = Time.Now;
	}

	const int ORIGIN_BACKUP = 64;
	const int ORIGIN_MASK = (ORIGIN_BACKUP - 1);

	Vector3 lastorg = Vector3.Zero;

	void VSmoothing( Vector3 simorg )
	{
		if ( Local.Pawn is not HLPlayer pawn ) return;
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
					delta = ViewInterp.Origins[(foundidx + 1) & ORIGIN_MASK] - ViewInterp.Origins[foundidx & ORIGIN_MASK];
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
						if ( pawn.ActiveChild is Weapon )
						{

							var wep = pawn.ActiveChild as Weapon;
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
		/* TODO: FIX
		if ( pawn.ActiveChild is HLWeapon )
		{

			var wep = pawn.ActiveChild as HLWeapon;
			// Weapon position
			if ( wep.ViewModelEntity is Entity )
			{
				wep.ViewModelEntity.Position += wepchange;
			}
		}
		*/
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
