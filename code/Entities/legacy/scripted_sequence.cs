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

    public scripted_sequence()
    {
    }

    [Input]
    void BeginSequence()
    {
        if (FindByName(TargetEntity) is NPC)
        {
            var ent = FindByName(TargetEntity) as NPC;
            ent.Steer.Target = this.Position;
            Log.Info("script sequence target set to " + TargetEntity);
            //TargetEntity.Position = this.Position; //TODO make this do something lol
        }
    }

    [Input]
    void MoveToPosition()
    {
        if (FindByName(TargetEntity) is NPC && FindByName(TargetEntity).IsValid())
        {
            var ent = FindByName(TargetEntity) as NPC;
            ent.Steer.Target = this.Position;
            Log.Info("script sequence target set to " + TargetEntity);
            //TargetEntity.Position = this.Position; //TODO make this do something lol
        }
    }
    public override void Spawn()
    {

        
    }
}
