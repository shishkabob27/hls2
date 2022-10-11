partial class HLPlayer
{
	Vector3 prevVel = Vector3.Zero;
	const int PLAYER_FATAL_FALL_SPEED = 1024;// approx 20 metres
	const int PLAYER_MAX_SAFE_FALL_SPEED = 580;// approx 5 metres
	const float DAMAGE_FOR_FALL_SPEED = (float)100 / (PLAYER_FATAL_FALL_SPEED - PLAYER_MAX_SAFE_FALL_SPEED);// damage per unit per second.
	const int PLAYER_MIN_BOUNCE_SPEED = 200;
	const float PLAYER_FALL_PUNCH_THRESHHOLD = (float)350; // won't punch player's screen/make scrape noise unless player falling at least this fast.

	[ConVar.ClientData] public static bool hl_won_fall_damage_sound { get; set; } = false;
	[ConVar.Replicated] public static int mp_falldamage { get; set; } = 0;

	void FallDamageThink()
	{
		if ( IsClient ) return;
		var FallSpeed = -prevVel.z;
		if ( GroundEntity != null && FallSpeed >= PLAYER_FALL_PUNCH_THRESHHOLD )
		{
			float fvol = 0;
			var b = punchangle;

			if ( WaterEntity != null || WaterLevel > 0 )
			{

			}
			else if ( FallSpeed > PLAYER_MAX_SAFE_FALL_SPEED )
			{
				float flFallDamage = (FallSpeed - PLAYER_MAX_SAFE_FALL_SPEED) * DAMAGE_FOR_FALL_SPEED;

				if ( HLGame.GameIsMultiplayer() && mp_falldamage == 0 )
				{
					flFallDamage = 10;
				}

				if ( flFallDamage > Health )
				{
					Sound.FromWorld( "bodysplat", Position );
				}

				if ( flFallDamage > 0 )
				{
					// original hl1 dll had a bug that played these two sounds and the same time so i guess we can have it here if above won cvar is on
					if ( Client.GetClientData( "hl_won_fall_damage_sound" ).ToBool() ) Sound.FromWorld( "pl_fallpain2", Position );
					Sound.FromWorld( "pl_fallpain", Position );
					var a = new DamageInfo
					{
						Damage = flFallDamage,
						Flags = DamageFlags.Fall,
					};
					TakeDamage( a );
					fvol = 1;
					b.x = 0;
				}
			}
			b.z = FallSpeed * 0.013f;   // punch z axis

			if ( b[0] > 8 )
			{
				b[0] = 8;
			}
			punchangle = b;
		}
		prevVel = Velocity;
	}
}
