using Sandbox.UI;
using Sandbox.UI.GameMenu;
using System.Linq.Expressions;

[UseTemplate( "/UI/Menu/SubMenus/CreateGame.html" )]
class CreateGame : BaseMenuScreen
{

	public string sMap { get; set; }
	public int iMaxPlayers { get; set; } = 1;
	public bool cCampaign { get; set; } = true;
	public bool cDeathmatch { get; set; } = false;

	public PackageList mapList { get; set; }
	public Button MapName { get; set; }

	public CreateGame()
	{
		mapList = ChildrenOfType<PackageList>().First();
	}

	public void ChangeMap()
	{
		mapList.SetClass( "open", true );
	}

	public async Task StartServerAsync()
	{
		if ( sMap == null )
			return;

		var gamemode = cCampaign ? "Campaign" : "Deathmatch";
		
		var lobby = await Game.Menu.CreateLobbyAsync(iMaxPlayers);
		var name = $"{lobby.Owner}'s Half-Life {gamemode} server";
		lobby.Title = name;
		lobby.Map = sMap;
		lobby.ConVars.Add( "sv_gamemode", gamemode.ToLower() );
		_ = lobby.LaunchGameAsync();
	}

	public void ChangeGamemode(bool Campaign)
	{
		if (Campaign)
		{
			cCampaign = true;
			cDeathmatch = false;
		}
		else
		{
			cCampaign = false;
			cDeathmatch = true;
		}
	}

	public override void Tick()
	{
		base.Tick();
		mapList.OnSelected = ( map ) =>
		{
			sMap = map.FullIdent;
			MapName.Text = "Map: " + map.Title;
			mapList.SetClass( "open", false );
		};
	}

	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<InternetGames>();
		BaseButtonClickDown( p, a, true, "Create game" );
	}

}
