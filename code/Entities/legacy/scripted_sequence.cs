[Library("scripted_sequence")]
[HammerEntity]
[Title("scripted_sequence"), Category("Legacy"), Icon("volume_up")]
public partial class scripted_sequence : Entity
{
    /// <summary>
    /// The target entity.
    /// </summary>
    [Property("m_iszEntity")]
    public Entity TargetEntity { get; set; }
    
    public scripted_sequence()
    {
        TargetEntity.Position = this.Position; //TODO make this do something lol
    }
}
