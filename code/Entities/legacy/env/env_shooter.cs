[Library("env_shooter")]
[HammerEntity]
[EditorSprite("editor/env_shooter.vmat")]
[Title("env_shooter"), Category("Legacy"), Icon("dirty_lens")]
public partial class env_shooter : Entity
{
    // stub

    [Property(Title = "Number of Gibs")]
    public int m_iGibs { get; set; } = 1;

    [Property(Title = "Gib Velocity")]
    public int m_flVelocity { get; set; } = 1;

    [Input]
    public void Shoot()
    {
        HLCombat.CreateGibs(this.Position, this.Position, Health, new BBox(new Vector3(-16, -16, 0), new Vector3(16, 16, 72)));
    }
}