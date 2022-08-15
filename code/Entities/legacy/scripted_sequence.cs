[Library("scripted_sequence")]
[HammerEntity]
[Title("scripted_sequence"), Category("Legacy"), Icon("volume_up")]
public partial class scripted_sequence : Entity
{
    /// <summary>
    /// The target entity.
    /// </summary>
    [Property("m_iszEntity"), FGDType("target_destination")]
    public string TargetEntity { get; set; }

    [Property("m_iszPlay")]
    public string ActionAnimation { get; set; }

    public NPC TargetNPC;
    public scripted_sequence()
    {
    }

    
    protected Output OnEndSequence { get; set; }
    
    [Input]
    void BeginSequence()
    {
        if (FindByName(TargetEntity) is NPC)
        {
            TargetNPC = FindByName(TargetEntity) as NPC;
            Log.Info(TargetNPC.CurrentSequence.Name);
            TargetNPC.Steer.Target = this.Position;
            TargetNPC.CurrentSequence.Name = ActionAnimation;
            
            Log.Info("script sequence target set to " + TargetEntity);
            //TargetEntity.Position = this.Position; //TODO make this do something lol
        }
    }
    
    [Input]
    void MoveToPosition()
    {
        if (FindByName(TargetEntity) is NPC && FindByName(TargetEntity).IsValid())
        {
            TargetNPC = FindByName(TargetEntity) as NPC;
            TargetNPC.Steer.Target = this.Position;
            Log.Info("script sequence target set to " + TargetEntity);
            //TargetEntity.Position = this.Position; //TODO make this do something lol
        }
    }
    public override void Spawn()
    {

        
    }
    
    [Event.Tick.Server]
    public void Tick()
    {
        if (TargetNPC != null && TargetNPC.CurrentSequence.IsFinished == true && TargetNPC.Position == this.Position)
        {
            OnEndSequence.Fire(this);
        }
    }
}
