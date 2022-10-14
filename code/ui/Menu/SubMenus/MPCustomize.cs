using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/MPCustomize.html" )]
class MPCustomize : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public async void InternetGames( Panel p )
	{
		await BaseButtonClickUp( p );
		Delete();
	}
	public async void Customize( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<Video>();
		Delete();
	}
	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Multiplayer>();
		BaseButtonClickDown( p, a, true, "Customize" );
	}
}
