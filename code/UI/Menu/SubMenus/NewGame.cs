using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/NewGame.html" )]
class NewGame : BaseMenuScreen
{
	public void Easy( Panel p )
	{
		ConsoleSystem.Run( "skill 0" );
		ConsoleSystem.Run( "chnglvlad "+ GameInfo.startmap );
	}
	public void Medium( Panel p )
	{
		ConsoleSystem.Run( "skill 1" );
		ConsoleSystem.Run( "chnglvlad "+ GameInfo.startmap );
	}
	public void Difficult( Panel p )
	{
		ConsoleSystem.Run( "skill 2" );
		ConsoleSystem.Run( "chnglvlad "+ GameInfo.startmap );
	}
	public void Cancel( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "New game" );
	}
}
