public partial class MenuGame : Game
{
	MenuPanel Menu;
	public MenuGame()
	{
		if ( IsServer )
		{
			Menu = new MenuPanel();
		}
	}
}
