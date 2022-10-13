[Library( "env_message" )]
[HammerEntity]
//[EditorSprite( "editor/env_message.vmat" )]
[Title( "env_message" ), Category( "Legacy" ), Icon( "toggle_on" )]
public partial class env_message : Entity
{

	[Property( "message" ), Title( "Message Text" ), Net]
	public string message { get; set; }

	public override void Spawn()
	{
		Transmit = TransmitType.Always;
		base.Spawn();
	}

	[Input]
	public void ShowMessage()
	{

	}

}

