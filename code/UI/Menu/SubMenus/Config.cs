using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Config.html" )]
class Config : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public void Audio( Panel p )
	{

	}
	public async void Video( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<Video>();
		Delete();
	}
	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "Configuration" );
	}
}
