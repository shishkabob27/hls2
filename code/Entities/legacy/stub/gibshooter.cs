[Library("gibshooter")]
[HammerEntity]
[EditorSprite("editor/gibshooter.vmat")]
[Title("gibshooter"), Category("Legacy")]
public partial class gibshooter : Entity
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