using Sandbox.UI;

[UseTemplate( "/UI/Menu/NewGame.html" )]
class NewGame : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public void Easy( Panel p )
	{
		ConsoleSystem.Run( "skill 0" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Medium( Panel p )
	{
		ConsoleSystem.Run( "skill 1" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Difficult( Panel p )
	{
		ConsoleSystem.Run( "skill 2" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Cancel( Panel p )
	{
		BaseButtonClickDown( p );
		Parent.AddChild<Menu>();
		Delete();
	}
}
