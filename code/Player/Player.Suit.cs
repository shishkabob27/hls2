partial class HLPlayer 
{
	static int CSUITPLAYLIST = 4;   // max of 4 suit sentences queued up at any time

	static bool SUIT_GROUP = true;
	static bool SUIT_SENTENCE = false;

	static int SUIT_REPEAT_OK = 0;
	static int SUIT_NEXT_IN_30SEC = 30;
	static int SUIT_NEXT_IN_1MIN = 60;
	static int SUIT_NEXT_IN_5MIN = 300;
	static int SUIT_NEXT_IN_10MIN = 600;
	static int SUIT_NEXT_IN_30MIN = 1800;
	static int SUIT_NEXT_IN_1HOUR = 3600;

	static int CSUITNOREPEAT = 32;

	float SuitUpdate;                   // when to play next suit update
	string[] SuitPlayList = new string[CSUITPLAYLIST];// next sentencenum to play for suit update
	int SuitPlayNext;                // next sentence slot for queue storage;
	string[] SuitNoRepeat = new string[CSUITNOREPEAT];       // suit sentence no repeat list
	float[] SuitNoRepeatTime = new float[CSUITNOREPEAT];    // how long to wait before allowing repeat

	public void SimulateSuit()
	{
		CheckSuitUpdate();
	}
	void INITSUIT()
	{
		SuitPlayList = new string[CSUITPLAYLIST];
		SuitNoRepeat = new string[CSUITNOREPEAT];
		SuitNoRepeatTime = new float[CSUITNOREPEAT];
	}
	// add sentence to suit playlist queue. if fgroup is true, then
	// name is a sentence group (HEV_AA), otherwise name is a specific
	// sentence name ie: !HEV_AA0.  If iNoRepeat is specified in
	// seconds, then we won't repeat playback of this word or sentence
	// for at least that number of seconds.

	void SetSuitUpdate( string name, int fgroup, int iNoRepeatTime )
	{
		int i;
		int isentence;
		int iempty = -1;


		// Ignore suit updates if no suit
		if ( !(HasHEV) )
			return;

		if ( HLGame.GameIsMultiplayer() )
		{
			// due to static channel design, etc. We don't play HEV sounds in multiplayer right now.
			return;
		}

		// if name == NULL, then clear out the queue

		if ( name == null )
		{
			for ( i = 0; i < CSUITPLAYLIST; i++ )
				SuitPlayList[i] = "";
			return;
		}
		// get sentence or group number

		/*
		if ( fgroup == null )
		{
			isentence = SENTENCEG_Lookup( name, NULL );
			if ( isentence < 0 )
				return;
		}
		else
			// mark group number as negative
			isentence = -SENTENCEG_GetIndex( name );
		*/

		// check norepeat list - this list lets us cancel
		// the playback of words or sentences that have already
		// been played within a certain time.

		for ( i = 0; i < CSUITNOREPEAT; i++ )
		{

			
			if ( name == SuitNoRepeat[i] )
			{
				// this sentence or group is already in 
				// the norepeat list

				if ( SuitNoRepeatTime[i] < Time.Now )
				{
					// norepeat time has expired, clear it out
					SuitNoRepeat[i] = name;
					SuitNoRepeatTime[i] = 0.0f;
					iempty = i;
					break;
				}
				else
				{
					// don't play, still marked as norepeat
					return;
				}
			}
			// keep track of empty slot
			if ( SuitNoRepeat[i] == null )
			{
				iempty = i;
				break;
			}
		}

		// sentence is not in norepeat list, save if norepeat time was given

		if ( iNoRepeatTime != null )
		{
			if ( iempty < 0 )
				iempty = Game.Random.Int( 0, CSUITNOREPEAT - 1 ); // pick random slot to take over
			SuitNoRepeat[iempty] = name;
			SuitNoRepeatTime[iempty] = iNoRepeatTime + Time.Now;
		}

		// find empty spot in queue, or overwrite last spot

		SuitPlayList[SuitPlayNext++] = name;
		//Log.Info( name );
		if ( SuitPlayNext == CSUITPLAYLIST )
			SuitPlayNext = 0;

		if ( SuitUpdate <= Time.Now )
		{
			if ( SuitUpdate == 0 )
				// play queue is empty, don't delay too long before playback
				SuitUpdate = Time.Now + SUITFIRSTUPDATETIME;
			else
				SuitUpdate = Time.Now + SUITUPDATETIME;
		}

	}
	static float SUITUPDATETIME = 3.5f;
	static float SUITFIRSTUPDATETIME = 0.1f;
	void CheckSuitUpdate()
	{
		int i;
		string isentence = "";
		int isearch = SuitPlayNext;

		// Ignore suit updates if no suit
		if ( !HasHEV )
			return;

		// if in range of radiation source, ping geiger counter
		//UpdateGeigerCounter();

		if ( HLGame.GameIsMultiplayer() )
		{
			// don't bother updating HEV voice in multiplayer.
			return;
		}

		if ( Time.Now >= SuitUpdate )
		{
			// play a sentence off of the end of the queue
			for ( i = 0; i < CSUITPLAYLIST; i++ )
			{
				if ( (isentence = SuitPlayList[isearch]) != null)
					break;

				if ( ++isearch == CSUITPLAYLIST )
					isearch = 0;
			}

			if ( isentence != null )
			{
				SuitPlayList[isearch] = null;
				if ( isentence != null && SuitUpdate > 0 )
				{
					// play sentence number

					//char sentence[CBSENTENCENAME_MAX + 1];
					//strcpy( sentence, "!" );
					//strcat( sentence, gszallsentencenames[isentence] );
					//EMIT_SOUND_SUIT( ENT( pev ), sentence );
					//PlaySound( isentence );
					Sentence.Play( isentence, this );
				}
				else
				{
					// play sentence group
					///EMIT_GROUPID_SUIT( ENT( pev ), -isentence );
				}
				SuitUpdate = Time.Now + SUITUPDATETIME;
			}
			else
				// queue is empty, don't check 
				SuitUpdate = 0;
		}
	}
	void SuitTalkDamage(DamageInfo dmg)
	{
		bool ftrivial = (Health > 75 || LastDamage.Damage < 5);
		bool fmajor = (LastDamage.Damage > 25);
		bool fcritical = (Health < 30);

		// handle all bits set in this damage message,
		// let the suit give player the diagnosis

		// UNDONE: add sounds for types of damage sustained (ie: burn, shock, slash )

		// UNDONE: still need to record damage and heal messages for the following types

		// DMG_BURN	
		// DMG_FREEZE
		// DMG_BLAST
		// DMG_SHOCK
		 

			if ( dmg.HasTag(DamageFlags.Blunt) )
			{
				if ( fmajor )
					SetSuitUpdate( "HEV_DMG4", 0, SUIT_NEXT_IN_30SEC );    // minor fracture
			}
			if ( (dmg.HasTag( DamageFlags.Fall ) | dmg.HasTag( DamageFlags.Crush )) )
			{
				if ( fmajor )
					SetSuitUpdate( "HEV_DMG5", 0, SUIT_NEXT_IN_30SEC );    // major fracture
				else
					SetSuitUpdate( "HEV_DMG4", 0, SUIT_NEXT_IN_30SEC );    // minor fracture
			}

			if ( dmg.HasTag( DamageFlags.Bullet ) )
			{
				if ( LastDamage.Damage > 5 )
					SetSuitUpdate( "HEV_DMG6", 0, SUIT_NEXT_IN_30SEC );    // blood loss detected
																				//else
																				//	SetSuitUpdate("!HEV_DMG0", FALSE, SUIT_NEXT_IN_30SEC);	// minor laceration
			}

			if ( dmg.HasTag( DamageFlags.Slash ) )
			{
				if ( fmajor )
					SetSuitUpdate( "HEV_DMG1", 0, SUIT_NEXT_IN_30SEC );    // major laceration
				else
					SetSuitUpdate( "HEV_DMG0", 0, SUIT_NEXT_IN_30SEC );    // minor laceration
			}

			if ( dmg.HasTag( DamageFlags.Sonic ) )
			{
				if ( fmajor )
					SetSuitUpdate( "HEV_DMG2", 0, SUIT_NEXT_IN_1MIN ); // internal bleeding
			}

			if ( (dmg.HasTag( DamageFlags.Poison ) | dmg.HasTag( DamageFlags.Paralyze )) )
			{
				SetSuitUpdate( "HEV_DMG3", 0, SUIT_NEXT_IN_1MIN ); // blood toxins detected
			}

			if ( dmg.HasTag( DamageFlags.Acid ) )
			{
				SetSuitUpdate( "HEV_DET1", 0, SUIT_NEXT_IN_1MIN ); // hazardous chemicals detected
			}

			if ( dmg.HasTag( DamageFlags.NerveGas ) )
			{
				SetSuitUpdate( "HEV_DET0", 0, SUIT_NEXT_IN_1MIN ); // biohazard detected
			}

			if ( dmg.HasTag( DamageFlags.Radiation ) )
			{
				SetSuitUpdate( "HEV_DET2", 0, SUIT_NEXT_IN_1MIN ); // radiation detected
			}
			if ( dmg.HasTag( DamageFlags.Shock ) )
			{
			}


		if ( !ftrivial && fmajor && healthPrev >= 75 )
		{
			// first time we take major damage...
			// turn automedic on if not on
			SetSuitUpdate( "HEV_MED1", 0, SUIT_NEXT_IN_30MIN );    // automedic on

			// give morphine shot if not given recently
			SetSuitUpdate( "HEV_HEAL7", 0, SUIT_NEXT_IN_30MIN );   // morphine shot
		}

		if ( !ftrivial && fcritical && healthPrev < 75 )
		{

			// already took major damage, now it's critical...
			if ( Health < 6 )
				SetSuitUpdate( "HEV_HLTH3", 0, SUIT_NEXT_IN_10MIN );   // near death
			else if ( Health < 20 )
				SetSuitUpdate( "HEV_HLTH2", 0, SUIT_NEXT_IN_10MIN );   // health critical

			// give critical health warnings
			if ( !(Game.Random.Int( 0, 3 ) == 0) && healthPrev < 50 )
				SetSuitUpdate( "HEV_DMG7", 0, SUIT_NEXT_IN_5MIN ); //seek medical attention
		}

		// if we're taking time based damage, warn about its continuing effects
		if ( dmg.HasTag( DamageFlags.Burn ) && healthPrev < 75 )
		{
			if ( Health < 50 )
			{
				if ( Game.Random.Int( 0, 3 ) != 0 )
					SetSuitUpdate( "HEV_DMG7", 0, SUIT_NEXT_IN_5MIN ); //seek medical attention
			}
			else
				SetSuitUpdate( "HEV_HLTH1", 0, SUIT_NEXT_IN_10MIN );   // health dropping
		}

	}
}
