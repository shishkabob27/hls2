[Library("func_breakable")]
[HammerEntity, Solid]
[Title("func_breakable"), Category("Brush Entities"), Icon("volume_up")]
public partial class func_breakable : BrushEntity
{
    // stub

    [Property]
    public float StartingHealth { get; set; } = 10;
    public override void Spawn()
    {
        base.Spawn();
        Health = StartingHealth;
    }

    [Event.Tick.Server]
    public void Tick()
    {
        if (Health <= 0)
        {
            Break();
        }
    } 
}   