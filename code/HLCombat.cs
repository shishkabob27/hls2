// https://github.com/ValveSoftware/halflife/blob/master/dlls/combat.cpp

public partial class HLCombat
{
	[ConVar.Replicated] public static float max_gibs { get; set; } = 100;
	static public int GibCount = 0;
	static public int GibFadingCount = 0;

	/// <summary>
	/// Weapon holdtypes.
	/// </summary>
	public enum HoldTypes
	{
		None,
		Pistol,
		Python,
		Rifle,
		Shotgun,
		HoldItem,
		Crossbow,
		Egon,
		Gauss,
		Hive,
		RPG,
		Squeak,
		Trip,
		Punch,
		Swing
	}
	// monster to monster relationship types
	public static int R_AL = -2; // (ALLY) pals. Good alternative to R_NO when applicable.
	public static int R_FR = -1; // (FEAR)will run
	public static int R_NO = 0;  // (NO RELATIONSHIP) disregard
	public static int R_DL = 1;  // (DISLIKE) will attack
	public static int R_HT = 2;  // (HATE)will attack this character instead of any visible DISLIKEd characters
	public static int R_NM = 3;  // (NEMESIS)  A monster Will ALWAYS attack its nemsis, no matter what

	/// <summary>
	/// Classes used for NPC/ICombat Classification.
	/// </summary>
	public enum Class
	{
		// For CLASSIFY
		CLASS_NONE,
		CLASS_MACHINE,
		CLASS_PLAYER,
		CLASS_HUMAN_PASSIVE,
		CLASS_HUMAN_MILITARY,
		CLASS_ALIEN_MILITARY,
		CLASS_ALIEN_PASSIVE,
		CLASS_ALIEN_MONSTER,
		CLASS_ALIEN_PREY,
		CLASS_ALIEN_PREDATOR,
		CLASS_INSECT,
		CLASS_PLAYER_ALLY,
		CLASS_PLAYER_BIOWEAPON,
		CLASS_ALIEN_BIOWEAPON,
		CLASS_BARNACLE
	}

	/// <summary>
	/// The class and relationship matrix, decides which NPCs should hate and love eachother.
	/// </summary>
	public static int[,] ClassMatrix = new int[14, 14]
	{			 //   NONE	 MACH	 PLYR	 HPASS	 HMIL	 AMIL	 APASS	 AMONST	APREY	 APRED	 INSECT	PLRALY	PBWPN	ABWPN
	/*NONE*/		{ R_NO  ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO,  R_NO,   R_NO    },
	/*MACHINE*/		{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_DL,  R_DL,   R_DL    },
	/*PLAYER*/		{ R_NO  ,R_DL   ,R_NO   ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO,  R_DL,   R_DL    },
	/*HUMANPASSIVE*/{ R_NO  ,R_NO   ,R_AL   ,R_AL   ,R_HT   ,R_FR   ,R_NO   ,R_HT   ,R_DL   ,R_FR   ,R_NO   ,R_AL,  R_NO,   R_NO    },
	/*HUMANMILITAR*/{ R_NO  ,R_NO   ,R_HT   ,R_DL   ,R_NO   ,R_HT   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_HT,  R_NO,   R_NO    },
	/*ALIENMILITAR*/{ R_NO  ,R_DL   ,R_HT   ,R_DL   ,R_HT   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_DL,  R_NO,   R_NO    },
	/*ALIENPASSIVE*/{ R_NO  ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO,  R_NO,   R_NO    },
	/*ALIENMONSTER*/{ R_NO  ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_DL,  R_NO,   R_NO    },
	/*ALIENPREY   */{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_NO   ,R_NO   ,R_FR   ,R_NO   ,R_DL,  R_NO,   R_NO    },
	/*ALIENPREDATO*/{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_NO   ,R_HT   ,R_DL   ,R_NO   ,R_DL,  R_NO,   R_NO    },
	/*INSECT*/		{ R_FR  ,R_FR   ,R_FR   ,R_FR   ,R_FR   ,R_NO   ,R_FR   ,R_FR   ,R_FR   ,R_FR   ,R_NO   ,R_FR,  R_NO,   R_NO    },
	/*PLAYERALLY*/	{ R_NO  ,R_DL   ,R_AL   ,R_AL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_NO,  R_NO,   R_NO    },
	/*PBIOWEAPON*/	{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_DL   ,R_NO   ,R_DL,  R_NO,   R_DL    },
	/*ABIOWEAPON*/	{ R_NO  ,R_NO   ,R_DL   ,R_DL   ,R_DL   ,R_AL   ,R_NO   ,R_DL   ,R_DL   ,R_NO   ,R_NO   ,R_DL,  R_DL,   R_NO    }
	};

	/// <summary>
	/// Create HL1 Gibs.
	/// </summary>
	/// <param name="Position">Position that the gibs spawn at</param>
	/// <param name="DMGPos">The position that the gibbing damage was dealt, decides which direction the gibs go flying</param>
	/// <param name="Health">The health the gibbing object was at when it gibbed, decides gib velocity. Greater than -50 has multiplier of 0.7, greater than -200 has a multiplier of 2, everything else has a multiplier of 4</param>
	/// <param name="bbox">The BBox the gibs are spawned in.</param>
	/// <param name="Colour">Gib colour, 0 = Red (HUMAN), 1 = Yellow (ALIEN)</param>
	/// <param name="Count">Amount to spawn, 4 by default</param>
	public static void CreateGibs( Vector3 Position, Vector3 DMGPos, float Health, BBox bbox, int Colour = 0, int Count = 4 )
	{
		Sound.FromWorld( "bodysplat", Position );

		Vector3 attackDir = (DMGPos - new Vector3( 0, 0, 10 ) - Position).Normal;
		if ( Colour == 0 ) CreateHeadGib( Position, DMGPos, Health ); // no heads for aliens :(

		for ( int i = 0; i < Count; i++ )
		{
			var gib = new HLGib();
			gib.AngularVelocity = new Angles( Rand.Float( 100, 300 ), 0, Rand.Float( 100, 200 ) );

			gib.Velocity = attackDir * -1;
			gib.Velocity += new Vector3( Rand.Float( -0.25f, 0.25f ), Rand.Float( -0.25f, 0.25f ), Rand.Float( -0.25f, 0.25f ) );
			gib.Velocity = gib.Velocity * Rand.Float( 300f, 400f );

			if ( Health > -50 )
			{
				gib.Velocity = gib.Velocity * 0.7f;
			}
			else if ( Health > -200 )
			{
				gib.Velocity = gib.Velocity * 2;
			}
			else
			{
				gib.Velocity = gib.Velocity * 4;
			}

			gib.Position = bbox.RandomPointInside + Position - bbox.Mins;
			gib.Rotation = Rotation.LookAt( Vector3.Random.Normal );

			gib.Spawn( Colour );
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="Position">Position the gib spawns at</param>
	/// <param name="DMGPos">The position that the gibbing damage was dealt, decides which direction the gib go flying</param>
	/// <param name="Health">The health the gibbing object was at when it gibbed, decides gib velocity. Greater than -50 has multiplier of 0.7, greater than -200 has a multiplier of 2, everything else has a multiplier of 4</param>
	/// <param name="Colour">Gib colour, 0 = Red (HUMAN), 1 = Yellow (ALIEN), Alien doesn't have a head gib so this probably doesn't do anything.</param>
	static void CreateHeadGib( Vector3 Position, Vector3 DMGPos, float Health, int Colour = 0 )
	{
		Vector3 attackDir = (DMGPos - new Vector3( 0, 0, 10 ) - Position).Normal;
		var skullGib = new HLGib();

		skullGib.Position = Position;
		skullGib.Rotation = Rotation.LookAt( Vector3.Random.Normal );

		var player = HLUtils.FindPlayerInBox( Position, 2048 );

		// 5% chance head will be thrown at player's face.
		if ( player is HLPlayer && Rand.Float( 0, 100 ) <= 5 )
		{
			skullGib.Velocity = (((player as HLPlayer).CollisionWorldSpaceCenter + new Vector3( 0, 0, 72 )) - skullGib.CollisionWorldSpaceCenter).Normal * 500;
			skullGib.Velocity = skullGib.Velocity.WithZ( skullGib.Velocity.z + 100 );
		}
		else
		{

			//skullGib.Velocity = attackDir * -1;
			//skullGib.Velocity += new Vector3(Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f));
			//skullGib.Velocity = skullGib.Velocity * Rand.Float(300f, 400f);
			skullGib.Velocity = new Vector3( Rand.Float( -100, 100 ), Rand.Float( -100, 100 ), Rand.Float( 200, 300 ) );
		}
		if ( Health > -50 )
		{
			skullGib.Velocity = skullGib.Velocity * 0.7f;
		}
		else if ( Health > -200 )
		{
			skullGib.Velocity = skullGib.Velocity * 2;
		}
		else
		{
			skullGib.Velocity = skullGib.Velocity * 4;
		}

		skullGib.AngularVelocity = new Angles( Rand.Float( 100, 300 ), 0, Rand.Float( 100, 200 ) );

		skullGib.Spawn( "models/hl1/gib/hgib/hgib_skull1.vmdl" );
	}
}
