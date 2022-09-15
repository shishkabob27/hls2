[Library("func_pushable")]
[HammerEntity, Solid]
[Title("func_pushable"), Category("Brush Entities"), Icon("volume_up")]
public partial class func_pushable : HLMovementBrush
{
    // stub
    [Property]
    public float friction { get; set; } = 80;
    [Property]
    public float bounce { get; set; } = 0;

    public override void Spawn()
    {
        base.Spawn();
        EnableTouch = true;

        frictionmv = 80;
        GroundBounce = bounce;
        WallBounce = bounce;
    }
    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);
        string surfName = PhysicsBody?.GetDominantSurface();
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