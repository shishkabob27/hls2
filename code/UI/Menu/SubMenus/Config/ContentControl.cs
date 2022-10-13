using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Config/ContentControl.html" )]
class ContentControl : BaseMenuScreen
{
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
	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Config>();
		BaseButtonClickDown( p, a, true, "Content control" );
	}
}
