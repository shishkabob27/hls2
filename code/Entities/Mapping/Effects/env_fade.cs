[Library( "env_fade" )]
[HammerEntity]
[EditorSprite( "editor/snd_event.vmat" )]
[Title( "env_fade" ), Category( "Legacy" ), Icon( "toggle_on" )]
public partial class env_fade : Entity
{
	[Property( "rendercolor" ), Title( "Colour" )]
	public Color FadeColour { get; set; }

	[Property( "duration" ), Title( "Duration" )]
	public float Duration { get; set; }

	[Property( "holdtime" ), Title( "Hold time" )]
	public float HoldTime { get; set; }

	[Input]
	public void Fade()
	{

	}

}
