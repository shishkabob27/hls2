[Library("env_render")]
[HammerEntity]
[BoxSize( 32 )]
[RenderFields]
[Title("env_render"), Category("Legacy"), Icon("volume_up")]
public partial class env_render : Entity
{

	Color RenderColor { get; set; } = Color.FromBytes( 255, 255, 0 );

	// stub
	[Input]
    void Activate()
    {

    }
}
