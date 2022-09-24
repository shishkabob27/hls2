/// <summary>
/// Plays a sound event from a point. The point can be this entity or a specified entity's position.
/// </summary>
[Library("ambient_generic")]
[HammerEntity]
[EditorSprite("editor/ambient_generic.vmat")]
[VisGroup(VisGroup.Sound)]
[Title("ambient_generic"), Category("Sound"), Icon("volume_up")]
public partial class SoundEventEntity : Entity
{

    [Flags]
    public enum Flags
    {
        Playeverywhere = 1,
        StartSilent = 16,
        IsNOTLooped = 32,
    }


    [Property("spawnflags", Title = "Spawn Settings")]
    public Flags SpawnSettings { get; set; }
    /// <summary>
    /// Name of the sound to play.
    /// </summary>
    [Property("message"), FGDType("sound")]
    [Net] public string message { get; set; }

    /// <summary>
    /// The entity to use as the origin of the sound playback. If not set, will play from this snd_event_point.
    /// </summary>
    [Property("sourceEntityName"), FGDType("target_destination")]
    [Net] public string SourceEntityName { get; set; }

    /// <summary>
    /// Start the sound on spawn
    /// </summary>
    [Property("startOnSpawn")]
    [Net] public bool StartOnSpawn { get; set; }

    /// <summary>
    /// Stop the sound before starting to play it again
    /// </summary>
    [Property("stopOnNew", Title = "Stop before repeat")]
    [Net] public bool StopOnNew { get; set; }

    public Sound PlayingSound { get; protected set; }
    public SoundFile EventSound;
    public SoundEventEntity()
    {
        Transmit = TransmitType.Always;
    }

    /// <summary>
    /// Start the sound event. If an entity name is provided, the sound will originate from that entity
    /// </summary>
    [Input]
    protected void PlaySound()
    {
        OnStartSound();
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

    [Input]
    protected void ToggleSound()
    {
        OnStartSound();
    }

    [Input]
    void Kill()
    {
        if (IsServer)
            Delete();
    }
    public override void ClientSpawn()
    {
        if (StartOnSpawn) // || SpawnSettings.HasFlag(Flags.StartSilent) == false) broken. fix later
        {
            StartSound();
        }
    }
    int ticker = 0;
    [ClientRpc]
    protected void OnStartSound()
    {

        var source = FindByName(SourceEntityName, this);

        if (StopOnNew)
        {
            PlayingSound.Stop();
            PlayingSound = default;
        }
        var replacename = message;
        replacename = replacename.Replace("sounds/", "sounds/hl1/");
        replacename = replacename.Replace(".vsnd", ".sound");
        replacename = replacename.Replace("!", "");
        Log.Info($"starting sound {replacename}");
        Sound.FromScreen(message);
        //PlayingSound = Sound.FromEntity( message, source );
        //EventSound = SoundFile.Load(replacename);

        using (Prediction.Off())
        {
            if (SpawnSettings.HasFlag(Flags.Playeverywhere))
            {
                PlayingSound = Sound.FromScreen(replacename);
            }
            else
            {
                PlayingSound = Sound.FromWorld(replacename, Position);
            }
        }

    }

    //[ClientRpc]
    protected void OnStopSound()
    {
        PlayingSound.Stop();
        PlayingSound = default;
    }

    [Event.Tick.Server]
    void tick()
    {
        ticker += 1;
        if (PlayingSound.Finished == true && PlayingSound.ElapsedTime != 0 && SpawnSettings.HasFlag(Flags.IsNOTLooped) == false & ticker > 5)
        {
            ticker = 0;
            PlayingSound.Stop();
            PlayingSound = default;
            OnStartSound();
        }
    }
}