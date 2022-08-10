    /// <summary>
	/// Plays a sound event from a point. The point can be this entity or a specified entity's position.
	/// </summary>
	[Library( "ambient_generic" )]
	[HammerEntity]
	[EditorSprite( "editor/snd_event.vmat" )]
	[VisGroup( VisGroup.Sound )]
	[Title( "ambient_generic" ), Category( "Legacy" ), Icon( "volume_up" )]
	public partial class SoundEventEntity : Entity
	{
		/// <summary>
		/// Name of the sound to play.
		/// </summary>
		[Property( "message" ), FGDType( "sound" )]
		[Net] public string message { get; set; }

		/// <summary>
		/// The entity to use as the origin of the sound playback. If not set, will play from this snd_event_point.
		/// </summary>
		[Property( "sourceEntityName" ), FGDType( "target_destination" )]
		[Net] public string SourceEntityName { get; set; }

		/// <summary>
		/// Start the sound on spawn
		/// </summary>
		[Property( "startOnSpawn" )]
		[Net] public bool StartOnSpawn { get; set; }

		/// <summary>
		/// Stop the sound before starting to play it again
		/// </summary>
		[Property( "stopOnNew", Title = "Stop before repeat" )]
		[Net] public bool StopOnNew { get; set; }

		public Sound PlayingSound { get; protected set; }

		public SoundEventEntity()
		{
			Transmit = TransmitType.Always;
		}

		/// <summary>
		/// Start the sound event. If an entity name is provided, the sound will originate from that entity
		/// </summary>
		[Input]
		protected void StartSound()
		{
			OnStartSound();
		}

		/// <summary>
		/// Stop the sound event
		/// </summary>
		[Input]
		protected void StopSound()
		{
			OnStopSound();
		}

		public override void ClientSpawn()
		{
			if ( StartOnSpawn )
			{
				StartSound();
			}
		}

		[ClientRpc]
		protected void OnStartSound()
		{
			var source = FindByName( SourceEntityName, this );

			if ( StopOnNew )
			{
				PlayingSound.Stop();
				PlayingSound = default;
			}
	
			PlayingSound = Sound.FromEntity( message, source );
		}

		[ClientRpc]
		protected void OnStopSound()
		{
			PlayingSound.Stop();
			PlayingSound = default;
		}
	}