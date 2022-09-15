[Library("func_breakable")]
[HammerEntity, Solid]
[Title("func_breakable"), Category("Brush Entities"), Icon("volume_up")]
public partial class func_breakable : BrushEntity
{
    // stub
    public override void Spawn()
    {
        base.Spawn();
        if (Health <= 0)
        {
            Health = 1;
        }
    }

    [Event.Tick.Server]
    public void Tick()
    {
        if (Health <= 0)
        {
            Log.Info("1");
            Breakables.Break(this);
        }
    }
    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);
    }
    public override void OnKilled()
    { 
        Breakables.Break(this);
        Delete();
    }
}   