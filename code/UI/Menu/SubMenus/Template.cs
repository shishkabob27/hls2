using Sandbox.UI;

[UseTemplate( "/UI/Menu/NewGame.html" )]
class Template : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public void One( Panel p )
	{
		ConsoleSystem.Run( "skill 0" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Two( Panel p )
	{
		ConsoleSystem.Run( "skill 1" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Three( Panel p )
	{
		ConsoleSystem.Run( "skill 2" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Four( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "New game" );
	}
}
