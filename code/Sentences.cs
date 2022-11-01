public partial class Sentence : Entity
{
	public Sound ThisSound;
	public string SoundName;

	public int Type = 0; // 1 pos // 2 ent // 3 screen
	public bool Finished = false;
	Entity PlayingEnt;
	static public Sentence Play(string toplay, Entity ent)
	{
		var me = new Sentence();
		me.Transmit = TransmitType.Always;
		me.ThisSound = Sound.FromEntity( toplay, ent );
		me.PlayingEnt = ent;
		me.SoundName = toplay;
		me.Type = 2;
		return me;
	}

	[ConCmd.Server]
	static void sentenceplay(string ply)
	{
		Sentence.Play( ply, ConsoleSystem.Caller.Pawn );
	}

	int countMax { get; set; } = 1;
	int count { get; set; } = 1;
	[Event.Tick.Server]
	void CheckIfDone()
	{
		if ( ResourceLibrary.TryGet<SoundEvent>( "sounds/hl1/SENTENCES/" + SoundName + ".sound", out var soundas ) )
		{
			countMax = soundas.Sounds.Count;
		}
		if ( ResourceLibrary.TryGet<SoundEvent>( "sounds/hl1/fvox/" + SoundName + ".sound", out var soundas2 ) )
		{
			countMax = soundas2.Sounds.Count;
		}
		//Log.Info( count );
		//Log.Info( countMax );
		if ( count >= countMax )
		{
			Finished = true;
			Delete();
			return;
		}
		if (ThisSound.Finished)
		{
			switch ( Type )
			{
				case 1:
					count++;
					break;
				case 2:
					count++;
					ThisSound = Sound.FromEntity( SoundName, PlayingEnt );
					break;
				case 3:
					count++;
					ThisSound = Sound.FromScreen( SoundName );
					break;
				default:
				break;
			}
		}
	}
}
