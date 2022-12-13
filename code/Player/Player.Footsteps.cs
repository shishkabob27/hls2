partial class HLPlayer
{
	TimeSince lastFootStep;
	bool StepLeft;
	void FootstepSounds()
	{
		if ( Game.IsClient ) return;

		float velrun;
		float velwalk;
		float flduck;
		float fvol;

		float speed = Velocity.Length;

		if ( (IsDucked) || IsOnLadder )
		{
			velwalk = 60;       // These constants should be based on cl_movespeedkey * cl_forwardspeed somehow
			velrun = 80;        // UNDONE: Move walking to server
			flduck = 100;
		}
		else
		{
			velwalk = 120;
			velrun = 210;
			flduck = 0;
		}
		bool fWalking = speed < velrun;

		if ( (IsOnLadder || (IsGrounded)) &&
			(Velocity.Length > 0.0) &&
			(speed >= velwalk) )
		{


			if ( lastFootStep > 0.35 && IsOnLadder )
			{
				lastFootStep = 0;
				PlaySound( "pl_ladder" ).SetVolume( 0.5f );
			}

			if ( !HLGame.hl_legacyfootsteps ) return;
			if ( IsOnLadder ) return;
			if ( lastFootStep > (fWalking ? 0.4 : 0.3) )
			{
				StepLeft = !StepLeft;
				lastFootStep = 0;
				fvol = fWalking ? 0.2f : 0.5f;
				if ( IsDucked )
				{
					if ( HLGame.hl_fix_ducking_footsteps ) fvol *= 0.35f; else return;
					if ( HLGame.hl_fix_ducking_footsteps ) lastFootStep = -0.1f;
				}
				plfootstep( Position, fvol, (StepLeft ? 0 : 1) );
			}
		}
	}
}
