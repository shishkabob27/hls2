using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Config/Audio.html" )]
class AudioMenu : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}


	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Config>();
		BaseButtonClickDown( p, a, true, "Audio" );
	}
}
