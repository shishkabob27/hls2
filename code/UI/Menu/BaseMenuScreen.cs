using Sandbox.UI;
[UseTemplate( "/UI/Menu/NewGame.html" )]
public class BaseMenuScreen : Panel
{
	Sound CurrentSound;

	Sound MenuPlaySound( string snd )
	{
		CurrentSound.Stop();
		if ( !string.IsNullOrEmpty( snd ) )
		{
			Vector2 vector = Box.Rect.Center / Screen.Size;
			vector -= (Vector2)0.5;
			vector *= 2f;
			Vector3 position = new Vector3( 1f, 0f - vector.x, 0f - vector.y );
			CurrentSound = Sound.FromWorld( snd, position ).SetVolume( 0.5f );
		}
		return CurrentSound;
	}
	public void BaseButtonClick()
	{
		MenuPlaySound( "launch_select2" );
	}
	public async void BaseButtonClickDown( Panel p, Panel prev, bool doanim = true, string buttonText = "d" )
	{
		var offsetY = menu_offsetY;
		var offsetX = menu_offsetX;
		MenuPlaySound( "launch_dnmenu1" );
		if ( doanim )
		{
			Button c = null;
			foreach ( var child in prev.Children )
			{
				foreach ( var child2 in child.Children )
				{
					var b = child2.Children.OfType<Button>().ToList();
					b.RemoveAll( w => (w as Button).Text != buttonText );
					if ( b.Count != 0 )
					{

						Log.Info( $"CBox: {b.First().Box.Rect.Position.y}" );
						c = b.First();
						break;
					}
				}
			}
			if ( c != null )
			{
				ISvYET( c );
			}
		}
	}
	public async Task isthis( Panel c )
	{
		int i = 0;
		while ( c.Box.Rect.Position.y == 0 && i < 1000 )
		{

			await GameTask.NextPhysicsFrame();//.DelaySeconds( 0.001f );
			Log.Info( i );
			i++;
		}

	}

	public async void ISvYET( Panel c )
	{
		await isthis( c );
		var d = c.Transitions;
		c.SkipTransitions();
		c.AddClass( "justcl" );
		c.Style.Top = 0;
		c.Style.Left = 0;

		CoolAnimation( c, false );

		await GameTask.DelaySeconds( 0.001f );
		c.Transitions.Entries = d.Entries;
		CoolAnimation( c, true );
	}

	[ConVar.Client]
	public static float menu_offsetX { get; set; } = 395;
	[ConVar.Client]
	public static float menu_offsetY { get; set; } = -605;
	public void CoolAnimation( Panel p, bool anim = true )
	{
		var offsetY = menu_offsetY;
		var offsetX = menu_offsetX;
		if ( anim )
		{
			if ( p.HasClass( "justcl" ) )
			{
				p.RemoveClass( "justcl" );
				p.AddClass( "justcldwn" );
				offsetX = 0;
			}
			else
			{
				p.RemoveClass( "justcldwn" );
				p.AddClass( "justcl" );
				if ( p.Parent.Parent.HasClass( "controlsscreen" ) ) // we're in a submenu so fix up our x offset
				{
					offsetX += 42;
				}
			}
		}



		var scale = 3.451f;
		var scale2 = MenuRootPanel.Current.Scale;
		var a = (0) + ((MenuRootPanel.Current.Box.Rect.Height / scale2) - (p.Box.Rect.Position.y / scale2)) - ((p.Box.Rect.Height / scale2) * (scale * 2));
		var b = (0);
		//Log.Info( a );
		//Log.Info( $"Height: {Screen.Height}" );
		//Log.Info( $"PosY: {p.Box.Rect.Position.y}" );
		//Log.Info( $"SizY: {p.Box.Rect.Height}" );
		//Log.Info( $"Scl1: {scale}" );
		//Log.Info( $"Scl2: {scale2}" );
		p.Style.Top = a + offsetY;
		p.Style.Left = b + offsetX;
	}
	public async Task BaseButtonClickUp( Panel p, bool doanim = true )
	{
		MenuPlaySound( "launch_upmenu1" );
		if ( doanim )
		{
			CoolAnimation( p );
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
