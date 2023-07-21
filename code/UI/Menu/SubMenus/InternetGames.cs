using Sandbox.UI;
using Sandbox.UI.GameMenu;
using System.Linq.Expressions;

[UseTemplate( "/UI/Menu/SubMenus/InternetGames.html" )]
class InternetGames : BaseMenuScreen
{
	public Panel serverList { get; set; }

	public InternetGames()
	{
		serverList = new ServerList();
		AddChild( serverList );
	}

	public async void CreateGame( Panel p )
	{
		await BaseButtonClickUp( p );
		Parent.AddChild<CreateGame>();
		Delete();
	}

	public void ServerRefresh()
	{
		var list = serverList as ServerList;
		list.ServerPanel.Refresh();
	}

	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Multiplayer>();
		BaseButtonClickDown( p, a, true, "Internet games" );
	}

	
}
