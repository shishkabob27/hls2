[Library("scripted_sentence")]
[HammerEntity]
[Title("scripted_sentence"), Category("Choreo"), Icon("theater_comedy")]
public partial class scripted_sentence : Entity
{
    // stub

    

    [Property("entity"), FGDType("target_destination")]
    public string SpeakerName { get; set; }

    public NPC Speaker { get; set; }

    [Property("sentence")]
    public string SentenceName { get; set; } = "null";

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
            OnEndSentence.Fire(this);
        }
        else
        {
            Log.Error($"[HLS2] The sentence {name} cannot be found, perhaps it hasn't been added yet.");
        }

    }

    [Input]
    void Kill(){
        if (IsServer)
			Delete();
    }
}