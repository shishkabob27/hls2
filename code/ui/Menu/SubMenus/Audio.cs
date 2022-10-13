using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Audio.html" )]
class Audio : BaseMenuScreen
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
