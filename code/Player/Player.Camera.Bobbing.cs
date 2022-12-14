public partial class HLPlayer
{

	[ConVar.Client] public static float cl_rollspeed { get; set; } = 200.0f;
	[ConVar.Client] public static float cl_rollangle { get; set; } = 2.0f;
	[ConVar.Client] public static float cl_bob { get; set; } = 0.01f;
	[ConVar.Client] public static float cl_bobcycle { get; set; } = 0.8f;
	[ConVar.Client] public static float cl_bobup { get; set; } = 0.5f;



	[ConVar.Client] public static bool hl_won_viewbob { get; set; } = false;
	[ConVar.Client] public static bool hl_fix_mystery_viewbob_code { get; set; } = false;

	void AddViewBob()
	{
		var bob = V_CalcBob();
		var a = Position;
		a.z += bob;
		Position = a;
	}

	void AddViewmodelBob(BaseViewModel ViewModelEntity )
	{
		// Weapon bobbing
		if ( ViewModelEntity is HLViewModel )
		{
			var b = ViewModelEntity.Position;
			for ( var i = 0; i < 3; i++ )
			{
				b[i] += bob * 0.4f * Rotation.Forward[i];
			}

			if ( hl_fix_mystery_viewbob_code ) b.z += bob; // I don't understand, this is in the hl1 code but hl1 doesnt have this? is it broken in the original? i'll just comment it out...

			// pushing the view origin down off of the same X/Z plane as the ent's origin will give the
			// gun a very nice 'shifting' effect when the player looks up/down. If there is a problem
			// with view model distortion, this may be a cause. (SJB). 
			b.z -= 1;

			ViewModelEntity.Position = b;

			var WepRot = ViewModelEntity.Rotation;
			WepRot = WepRot.Angles().WithYaw( WepRot.Angles().yaw - bob * 0.5f ).ToRotation();
			WepRot = WepRot.Angles().WithRoll( WepRot.Angles().roll - bob * 1f ).ToRotation();
			WepRot = WepRot.Angles().WithPitch( WepRot.Angles().pitch - bob * 0.3f ).ToRotation();
			if ( hl_won_viewbob )
				ViewModelEntity.Rotation = WepRot;
			//wep.angle[YAW] -= bob * 0.5;
			//wep.angle[ROLL] -= bob * 1;
			//wep.angle[PITCH] -= bob * 0.3;

		}
	}


	double bobtime;
	float bob;
	float bobcycle;
	float lasttimebob;

	float V_CalcBob()
	{
		Vector3 vel;
		if ( Game.LocalPawn is not HLPlayer player ) return 0;

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
}
