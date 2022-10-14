using Sandbox.UI;

[UseTemplate( "/UI/Menu/Menu.html" )]
class Menu : BaseMenuScreen
{
	public bool checkAltButtonColour { get; set; } = false;
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
		ConsoleSystem.Run( "chnglvlad " + GameInfo.trainmap );
	}
	public async void Config( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<Config>();
		Delete();
	}
	public async void LoadGame( Panel p )
	{
		await BaseButtonClickUp( p );
	}
	public async void Multiplayer( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<Multiplayer>();
		Delete();
	}
	public async void CustomGame( Panel p )
	{
		await BaseButtonClickUp( p );
	}
	public void ViewReadme( Panel p )
	{
	}
	public async void Previews( Panel p )
	{
	}
	public void Quit()
	{
		Local.Client.Kick();
	}
}
