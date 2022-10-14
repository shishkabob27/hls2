using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Config.html" )]
class Config : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public async void Audio( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<AudioMenu>();
		Delete();
	}
	public async void Video( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<Video>();
		Delete();
	}
	public async void VR( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<VR>();
		Delete();
	}
	public async void ContentControl( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<ContentControl>();
		Delete();
	}
	public void Misc( Panel p )
	{
		ConsoleSystem.Run( "open_options" );
	}
	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "Configuration" );
	}
}
