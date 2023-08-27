[Library( "speaker" )]
[HammerEntity]
[Title( "speaker" ), Category("Legacy"), Icon("speaker")]
public class speaker : Entity
{
	// stub

	[Property] public int preset { get; set; }
	[Property] public int health { get; set; }
	[Property] public float radius { get; set; }

	[Property] public int delaymin { get; set; }
	[Property] public int delaymax { get; set; }

	[Flags]
	public enum Flags
	{
		StartSilent = 1
	}

	public TimeSince timeSinceLastSound;

	public TimeUntil timeUntilNextSound;

	//use sentences.txt maybe?
	string[] c1a0_preset = {"dadeda (e95) agent (t0) coomer (t0), report to (e70) topside tactical operations center (e100)",
		"dadeda inspection team, to(e70) radioactive materials handling bay (e100)",
		"dadeda (e95) agent sixteen, report to (e70) administration sub level two (e100)",
		"doop (e95) doctor johnson, please call observation tank one (e100)",
		"doop (e95 p96) doctor west (t0), please report to (e70) lambda (s0) reactor complex (e100)",
		"doop (e95) doctor cross, call seven two nine please (e100)",
		"doop (e95) sargeant (s0) bailey (s0), to (e70) topside checkpoint bravo (e100)",
		"dadeda (e95) cryogenic safety crew, report status at eleven hundred please (e100)",
		"dadeda (e95) coded message for captain black, command and (s0 e100 t0) communication center (e100)",
		"dadeda (e95 p102) sector c (e100) science personnel, report to (e70) anomalous materials test lab (e100)",
		"doop (e95 p103) doctor freeman , (e95) to (e70) anomalous materials test lab immediately (e100)",
		"bloop (e95 p98) attention . experimental propulsion lab test fire in (s0 e100 t0) ninety minutes (e100)",
		"bloop (e95 p98) hydro plant now (s0 e100 t0) operating at (s0 e100 t0) sixty percent (e100)",
		"bloop (e95) black mesa topside temperature is ninety three degrees (e100)",
		"bloop (e95) launch officer reports, alpha satellite deploy is (t0 s0 e100) nominal (e100)",
		"bloop (e95) doctor birdwell reports(e95) superconducting interchange is(t0 s0 e100) activated (e100)",
		"bloop (e95 p104) attention. report any security violation, to(e70) administration sub level one (e100)",
		"bloop (e95) attention. service personnel please clear helicopter hangar one (e100)",
		"bloop (e95) shield inspection(s0) crew reports primary reactor nominal (e100)",
		"bloop (e95 p98) transportation control reports all systems on time (e100)",
		"dadeda (e95) doctor victor, report to(e70) supercooled(e100) laser lab please (e100)",
		"dadeda (e95 p104) sargeant guthrie, report to(e70) topside motorpool immediately (e100)",
		"deeoo (e95 p98) uranium shipment inspection team, to (e100) sector d (e100)"
	};

	[Property( Title = "Spawn Settings" )]
	public Flags spawnflags { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		timeUntilNextSound = Game.Random.Float( delaymin, delaymax );
	}

	[GameEvent.Tick.Server]
	public void Tick()
	{
		if (timeUntilNextSound > 0 || !Enabled)
			return;

		PlaySound();

	}

	async void PlaySound()
	{
		var sentence_group = "";
		var sentenceindex = 0;
		var sentence = "";
		switch ( preset )
		{
			case 1: sentenceindex = Game.Random.Int( 0, c1a0_preset.Length );
				sentence_group = "C1A0_";
				sentence = c1a0_preset[Game.Random.Int(0, c1a0_preset.Length)];
				break;
			default: sentenceindex = 0; break;
		}

		
		var sentence_name = $"{sentence_group}{sentenceindex}";

		if ( sentence_name == "0")
			return;

		timeUntilNextSound = Game.Random.Float(delaymin, delaymax);

		Subtitle.DisplaySubtitle( sentence_name );

		//for each word in sentence, look up the sound file and play it

		sentence = sentence.Replace( ".", "_period" );
		sentence = sentence.Replace( ",", "_comma" );

		Log.Info( "Playing VOX" );
		Log.Info( sentence_name );
		Log.Info( sentence );

		while ( sentence != "" )
		{
			var word = sentence.Split( " " )[0];
			Log.Info( word );
			
			if ( word == "" )
				break;

			sentence = sentence.Remove( 0, word.Length + 1 );

			var soundpath = $"sounds/hl1/vox/{word}.sound";


			//check if sound exists
			if( ResourceLibrary.TryGet( soundpath, out SoundEvent sound ))
			{
				var soundword = Sound.FromEntity( To.Everyone, soundpath, this ).SetVolume( 1.0f ).SetPitch( 1.0f );
				await Task.DelayRealtime( 100 );
				while ( soundword.IsPlaying )
				{
					await Task.DelayRealtime( 1000 );
				}
			}

			

		};

		timeUntilNextSound = Game.Random.Float( delaymin, delaymax );
	}

	/// <summary>
	/// The (initial) enabled state of the logic entity.
	/// </summary>
	[Property]
	public bool Enabled { get; set; } = true;

	/// <summary>
	/// Enables the entity.
	/// </summary>
	[Input]
	public void Enable()
	{
		Enabled = true;
	}

	/// <summary>
	/// Disables the entity, so that it would not fire any outputs.
	/// </summary>
	[Input]
	public void Disable()
	{
		Enabled = false;
	}

	/// <summary>
	/// Toggles the enabled state of the entity.
	/// </summary>
	[Input]
	public void Toggle()
	{
		Enabled = !Enabled;
	}

	[Input]
	public void Kill()
	{
		if (Game.IsServer)
			Delete();
	}

}
