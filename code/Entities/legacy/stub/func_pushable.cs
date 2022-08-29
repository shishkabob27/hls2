[Library("func_pushable")]
[HammerEntity, Solid]
[Title("func_pushable"), Category("Brush Entities"), Icon("volume_up")]
public partial class func_pushable : HLMovementBrush
{
    // stub
    [Property]
    public float friction { get; set; } = 0;
    [Property]
    public float bounce { get; set; } = 0;

    public override void Spawn()
    {
        base.Spawn();
        EnableTouch = true;
        GroundBounce = bounce;
        WallBounce = bounce;
    }
}   