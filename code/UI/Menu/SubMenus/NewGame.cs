using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/NewGame.html" )]
class NewGame : BaseMenuScreen
{
	public void Easy( Panel p )
	{
		ConsoleSystem.Run( "skill 0" );
		Game.Menu.StartServerAsync( 1, "Half-Life Campaign (Easy)", GameInfo.startmap );
		Game.Menu.Lobby.Public = false;
		Game.Menu.HideMenu();
	}
	public void Medium( Panel p )
	{
		ConsoleSystem.Run( "skill 1" );
		Game.Menu.StartServerAsync( 1, "Half-Life Campaign (Medium)", GameInfo.startmap );
		Game.Menu.Lobby.Public = false;
		Game.Menu.HideMenu();
	}
	public void Difficult( Panel p )
	{
		ConsoleSystem.Run( "skill 2" );
		Game.Menu.StartServerAsync( 1, "Half-Life Campaign (Difficult)", GameInfo.startmap );
		Game.Menu.Lobby.Public = false;
		Game.Menu.HideMenu();
	}
	public void Cancel( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "New game" );
	}
}
