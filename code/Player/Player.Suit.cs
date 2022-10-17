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
				iempty = i;
		}

		// sentence is not in norepeat list, save if norepeat time was given

		if ( iNoRepeatTime != null )
		{
			if ( iempty < 0 )
				iempty = Rand.Int( 0, CSUITNOREPEAT - 1 ); // pick random slot to take over
			SuitNoRepeat[iempty] = name;
			SuitNoRepeatTime[iempty] = iNoRepeatTime + Time.Now;
		}

		// find empty spot in queue, or overwrite last spot

		SuitPlayList[SuitPlayNext++] = name;
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

		if ( Time.Now >= SuitUpdate && SuitUpdate > 0 )
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
				SuitPlayList[isearch] = "";
				if ( isentence != "" )
				{
					// play sentence number

					//char sentence[CBSENTENCENAME_MAX + 1];
					//strcpy( sentence, "!" );
					//strcat( sentence, gszallsentencenames[isentence] );
					//EMIT_SOUND_SUIT( ENT( pev ), sentence );
					PlaySoundFromScreen( isentence );
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
}
