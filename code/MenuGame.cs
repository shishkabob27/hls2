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
			GUI = new HLGUI();
			Menu = new MenuPanel();
		}
	}
}
