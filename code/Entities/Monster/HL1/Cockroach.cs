[Library( "monster_cockroach" ), HammerEntity]
[EditorModel( "models/hl1/monster/roach.vmdl" )]
[Title( "Cockroach" ), Category( "Monsters" ), Icon( "person" )]
public class Cockroach : NPC
{

	public override string Category => "Animals";
	public new float EyeHeight = 1;
	public new bool Unstick = false;

	//ROACH_IDLE
	//ROACH_BORED
	//ROACH_SCARED_BY_ENT
	//ROACH_SCARED_BY_LIGHT
	//ROACH_SMELL_FOOD
	//ROACH_EAT	
	string m_iMode = "ROACH_IDLE";
	float m_flLastLightLevel;
	float m_flNextSmellTime;
	bool m_fLightHacked;
	bool COND_SEE_FEAR;


	public override void Spawn()
	{
		base.Spawn();
		Health = 1;
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -1, -1, 0 ), new Vector3( 1, 1, 2 ) );
		SetModel( "models/hl1/monster/roach.vmdl" );
		entFOV = 0.5f;
		SleepDist = 128;
		DontSee = true;
		CannotBeSeen = true;
		EnableHitboxes = true;
		Tags.Add( "npc" );
	}

	public override int Classify()
	{
		return (int)HLCombat.Class.CLASS_INSECT;
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );
		if ( other is HLPlayer && IsServer )
		{
			Kill();
		}
	}

	public override void Think()
	{

		switch ( m_iMode )
		{
			case "ROACH_IDLE":
			case "ROACH_EAT":
				{
					// if not moving, sample environment to see if anything scary is around. Do a radius search 'look' at random.
					if ( Rand.Int( 0, 3 ) == 1 )
					{
						Look( 150 );
						if ( COND_SEE_FEAR )
						{
							// if see something scary
							//ALERT ( at_aiconsole, "Scared\n" );
							Eat( 30 + (Rand.Int( 0, 14 )) );// roach will ignore food for 30 to 45 seconds
							PickNewDest( "ROACH_SCARED_BY_ENT" );
							SetActivity( "ACT_WALK" );
						}
						else if ( Rand.Int( 0, 149 ) == 1 )
						{
							// if roach doesn't see anything, there's still a chance that it will move. (boredom)
							//ALERT ( at_aiconsole, "Bored\n" );
							PickNewDest( "ROACH_BORED" );
							SetActivity( "ACT_WALK" );

							if ( m_iMode == "ROACH_EAT" )
							{
								// roach will ignore food for 30 to 45 seconds if it got bored while eating. 
								Eat( 30 + (Rand.Int( 0, 14 )) );
							}
						}
					}

					// don't do this stuff if eating!
					if ( m_iMode == "ROACH_IDLE" )
					{
						if ( FShouldEat() )
						{
							Listen();
						}
					}

					break;
				}
		}
	}

	void SetActivity( string activity )
	{

	}

	void PickNewDest( string mode )
	{

	}

	void Look( int iDistance )
	{

	}

	void Listen()
	{

	}

	void GETENTITYILLUM()
	{

	}

	void Eat( int time )
	{

	}

	bool FShouldEat()
	{
		return false;
	}
}
