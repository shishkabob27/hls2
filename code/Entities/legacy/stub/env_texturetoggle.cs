[Library("env_texturetoggle")]
[HammerEntity]
[EditorSprite("editor/snd_event.vmat")]
[Title("env_texturetoggle"), Category("Legacy"), Icon("toggle_on")]
public partial class TextureToggle : Entity
{
    [Property( "target" ), Title( "Target" )]
	public EntityTarget TargetEntity { get; set; }

    [Input]
	public void IncrementTextureIndex(){
	}

}