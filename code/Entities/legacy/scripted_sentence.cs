[Library("scripted_sentence")]
[HammerEntity]
[Title("scripted_sentence"), Category("Choreo"), Icon("volume_up")]
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
        if (Speaker is not NPC)
        {
            Speaker = FindByName(SpeakerName) as NPC;
        }
        // use sentences.txt? maybe?
        var name = SentenceName.Replace("!", "");
        OnBeginSentence.Fire(this);
        Log.Info($"playing {name}");
        Speaker.SpeakSound(name);
        OnEndSentence.Fire(this);

    }
}