using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Video.html" )]
class Video : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}


	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Config>();
		BaseButtonClickDown( p, a, true, "Video" );
	}
}
