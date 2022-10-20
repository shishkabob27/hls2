/// <summary>
/// A simple version of the Game that loads just the menu and the GUI, used when the map is <empty
/// </summary>
/*
public partial class MenuGame : Game
{
	[Net]
	public MenuPanel Menu { get; set; }
	[Net]
	public HLGUI GUI { get; set; }

	public MenuGame()
	{
		if ( IsServer )
		{
			if ( Global.MapName != "<empty>" )
			{
				new HLGame();
				this.Delete();
				return;
			}
			
			GUI = new HLGUI();
			Menu = new MenuPanel();
		}
	}
}
*/ // this can temporarily go eat a dick.
