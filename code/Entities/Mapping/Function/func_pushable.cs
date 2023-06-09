[Library("func_pushable")]
[HammerEntity, Solid]
[Title("func_pushable"), Category("Brush Entities"), Icon("place_item")]
public partial class func_pushable : HLMovementBrush
{
    [Property]
    new public float Health { get; set; } = 0;
    // stub
    [Property]
    public float friction { get; set; } = 80;
    [Property]
    public float bounce { get; set; } = 0;

    public bool Invincible { get; set; } = false;

    public override void Spawn()
    {
        base.Spawn();
        EnableTouch = true;

        frictionmv = 80;
        GroundBounce = bounce;
        WallBounce = bounce;
        if (Health <= 0)
        {
            Invincible = true;
        }
    }

    [GameEvent.Tick.Server]
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
        if (Game.IsServer && Health > 0f && LifeState == LifeState.Alive)
        {
            Health -= info.Damage;
            if (Health <= 0f)
            {
                Health = 0f;
                OnKilled();
            }
        }
        string surfName = PhysicsBody?.SurfaceMaterial;
        var surface = Surface.FindByName(surfName);
        if (surface == null) surface = Surface.FindByName("default");
        surface.GetBounceSound(Position, 2f);
    }
    public override void OnKilled()
    {
        Breakables.Break(this);
        Delete();
    }
}   
