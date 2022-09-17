﻿[Library("func_train")]
[HammerEntity]
[Title("func_train"), Category("Brush Entities"), Icon("volume_up")]
public partial class func_train : BrushEntity
{

    [Property("target"), FGDType("target_destination")]
    public string Target { get; set; } = "";

    public override void Spawn()
    {
        base.Spawn();
    }

    [Event.Tick.Server] 
    void tick()
    {
        if (Enabled)
        {
            
            Position = Position.LerpTo(Entity.FindAllByName(Target).First().Position, 0.2f);
            if (Position.AlmostEqual(Entity.FindAllByName(Target).First().Position, 16))
            {
                Position = Entity.FindAllByName(Target).First().Position;
                Target = (Entity.FindAllByName(Target).First() as path_corner).Target;
            }
            
        }
    }
    /// <summary>
    /// Enables the entity.
    /// </summary>
    [Input]
    public void StartForward()
    {
        Enabled = true;
    }
    /// <summary>
    /// Enables the entity.
    /// </summary>
    [Input]
    public void Enable()
    {
        Enabled = true;
    }

    /// <summary>
    /// Disables the entity, so that it would not fire any outputs.
    /// </summary>
    [Input]
    public void Disable()
    {
        Enabled = false;
    }

    /// <summary>
    /// Toggles the enabled state of the entity.
    /// </summary>
    [Input]
    public void Toggle()
    {
        Enabled = !Enabled;
    }
    // stub
}