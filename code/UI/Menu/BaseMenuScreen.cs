using Sandbox.UI;
[UseTemplate( "/UI/Menu/NewGame.html" )]
public class BaseMenuScreen : Panel
{
	public void BaseButtonClick()
	{
		PlaySound( "launch_select2" );
	}
	public async Task BaseButtonClickDown( Panel p, bool doanim = true )
	{
		var offsetY = menu_offsetY;
		var offsetX = menu_offsetX;
		PlaySound( "launch_dnmenu1" );
		if ( doanim )
		{
			var scale = 3.451f;
			var scale2 = MenuRootPanel.Current.Scale;
			var a = (0) + ((MenuRootPanel.Current.Box.Rect.Height / scale2) - (p.Box.Rect.Position.y / scale2)) - ((p.Box.Rect.Height / scale2) * (scale * 2));
			var b = (0);
			Log.Info( a );
			p.Style.Top = a + offsetY;
			p.Style.Left = b + offsetX;
			AddClass( "justcldwn" );
			p.Style.Top = 0;
			p.Style.Left = 0;
			await GameTask.DelaySeconds( 0.2f );
		}
	}
	[ConVar.Client]
	public static float menu_offsetX { get; set; } = 390;
	[ConVar.Client]
	public static float menu_offsetY { get; set; } = -613;
	public async Task BaseButtonClickUp( Panel p, bool doanim = true )
	{
		PlaySound( "launch_upmenu1" );
		if ( doanim )
		{
			var offsetY = menu_offsetY;
			var offsetX = menu_offsetX;
			if ( p.HasClass( "justcl" ) )
			{
				p.RemoveClass( "justcl" );
				p.AddClass( "justcldwn" );
				offsetX = 0;
			}
			else
			{
				p.AddClass( "justcl" );
			}

			var scale = 3.451f;
			var scale2 = MenuRootPanel.Current.Scale;
			var a = (0) + ((MenuRootPanel.Current.Box.Rect.Height / scale2) - (p.Box.Rect.Position.y / scale2)) - ((p.Box.Rect.Height / scale2) * (scale * 2));
			var b = (0);
			Log.Info( a );
			p.Style.Top = a + offsetY;
			p.Style.Left = b + offsetX;
			await GameTask.DelaySeconds( 0.2f );
		}
	}

	[ConCmd.Server]
	static void chnglvlad( string MAP )
	{
		if ( ConsoleSystem.Caller.IsListenServerHost )
		{
			Global.ChangeLevel( MAP );
		}
	}
}
