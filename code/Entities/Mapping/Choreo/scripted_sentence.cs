[Library("scripted_sentence")]
[HammerEntity]
[EditorSprite( "editor/scripted_sentence.vmat" )]
[Title("scripted_sentence"), Category("Choreo"), Icon("theater_comedy")]
public partial class scripted_sentence : Entity
{
    // stub

    

    [Property("entity"), FGDType("target_destination")]
    public string SpeakerName { get; set; }

    public NPC Speaker { get; set; }

    [Property("sentence")]
    public string SentenceName { get; set; } = "null";

	[Property( "listener" ), FGDType( "target_destination" )]
	public Entity  Listener { get; set; }

	protected Output OnEndSentence { get; set; }
    protected Output OnBeginSentence { get; set; }
    
    [Input]
    void BeginSentence()
    {
        if (Speaker is not NPC || !Speaker.IsValid)
        {
            Speaker = FindByName(SpeakerName) as NPC;
        }
        if (Speaker is not NPC || !Speaker.IsValid)
            return;
        // use sentences.txt? maybe?
        var name = SentenceName.Replace("!", "");
        
        //Log.Info(soundas.ResourceName);
        if (ResourceLibrary.TryGet<SoundEvent>("sounds/hl1/SENTENCES/" + name + ".sound", out var soundas))
        {
            OnBeginSentence.Fire(this);
            Speaker.SpeakSound(name);
			Speaker.SentenceListener = Listener;
            OnEndSentence.Fire(this);

			Subtitle.DisplaySubtitle( To.Everyone, name);
		}
        else
        {
            Log.Error($"[HLS2] The sentence {name} cannot be found, perhaps it hasn't been added yet.");
        }

    }

    [Input]
    void Kill(){
        if (Game.IsServer)
			Delete();
    }
}
