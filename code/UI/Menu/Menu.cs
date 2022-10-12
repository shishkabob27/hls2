using Sandbox.UI;

[UseTemplate( "/UI/Menu/Menu.html" )]
class Menu : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public async void NewGame( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<NewGame>();
		Delete();
	}
	public void HazardCourse( Panel p )
	{
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_t0" );
	}
	public async void Config( Panel p )
	{
		await BaseButtonClickUp( p, false );

		ConsoleSystem.Run( "open_options" );
	}
	public async void LoadGame( Panel p )
	{
		await BaseButtonClickUp( p );
	}
	public async void Multiplayer( Panel p )
	{
		await BaseButtonClickUp( p );
	}
	public async void CustomGame( Panel p )
	{
		await BaseButtonClickUp( p );
	}
	public void ViewReadme( Panel p )
	{
		BaseButtonClick();
	}
	public async void Previews( Panel p )
	{
		BaseButtonClick();
	}
	public void Quit()
	{
		BaseButtonClick();
	}
}
