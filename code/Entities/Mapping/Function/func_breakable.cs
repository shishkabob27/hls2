[Library("func_breakable")]
[HammerEntity, Solid]
[Title("func_breakable"), Category("Brush Entities"), Icon("sound_detection_glass_break")]
public partial class func_breakable : BrushEntity
{
    [Property]
    new public float Health { get; set; } = 0;
    public bool Invincible { get; set; } = false;
    // stub
    public override void Spawn()
    {
        base.Spawn();
        if (Health <= 0)
        {
            Invincible = true;
        }
    }

    [Event.Tick.Server]
    public void Tick()
    {
        if (Health <= 0 && !Invincible)
        {
            Log.Info("1");
            Breakables.Break(this);
        }
    }
    public override void TakeDamage(DamageInfo info)
    {
        LastAttacker = info.Attacker;
        LastAttackerWeapon = info.Weapon;
        if (IsServer && Health > 0f && LifeState == LifeState.Alive)
        {
            Health -= info.Damage;
            if (Health <= 0f)
            {
                Health = 0f;
                OnKilled();
            }
        }
        string surfName = PhysicsBody?.GetDominantSurface();
        var surface = Surface.FindByName(surfName);
        if (surface == null) surface = Surface.FindByName("default");
        surface.GetBounceSound(Position, 2f);
    }
    public override void OnKilled()
    {
        if (Invincible) return;
        Breakables.Break(this);
        Delete();
    }
}   