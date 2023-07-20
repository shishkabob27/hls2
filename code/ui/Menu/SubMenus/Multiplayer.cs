using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Multiplayer.html" )]
class Multiplayer : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public async void InternetGames( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<InternetGames>();
		Delete();
	}

	public async void Customize( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<MPCustomize>();
		Delete();
	}
	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "Multiplayer" );
	}
}
