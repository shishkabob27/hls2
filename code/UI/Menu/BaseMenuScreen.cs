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
			// Find the button that we are clicked into, this is probably the most shit way to do this.
			foreach ( var child in prev.Children )
			{
				foreach ( var child2 in child.Children )
				{
					var b = child2.Children.OfType<Button>().ToList();
					b.RemoveAll( w => (w as Button).Text != buttonText );
					if ( b.Count != 0 )
					{
						c = b.First();
						break;
					}
				}
			}
			if ( c != null )
			{
				PrepareBackAnimation( c );
			}
		}
	}

	public async void PrepareBackAnimation( Panel c )
	{

		await GameTask.DelaySeconds( 0.001f ); // do this because panel c isn't ready yet for some stupid reason
		var d = c.Transitions;
		c.SkipTransitions();
		c.AddClass( "justcl" );
		c.Style.Top = 0;
		c.Style.Left = 0;

		CoolAnimation( c, false );

		await GameTask.DelaySeconds( 0.001f ); // do this because there's no way to turn off SkipTransitions for some stupid reason
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
		// this math sucks but it's supposed to find a way to consistantly get our button to the same spot no matter the position of the button and the resolution or aspect ratio of the screen. 
		var a = -(MenuRootPanel.Current.Children.First().Box.Rect.Position.y / scale2) + ((MenuRootPanel.Current.Box.Rect.Height / scale2) - (p.Box.Rect.Position.y / scale2)) - ((p.Box.Rect.Height / scale2) * (scale * 2));
		var b = (0);
		//Log.Info( a );
		//Log.Info( $"Height: {Screen.Height}" );
		//Log.Info( $"PosY: {p.Box.Rect.Position.y}" );
		//Log.Info( $"SizY: {p.Box.Rect.Height}" );
		//Log.Info( $"Scl1: {scale}" );
		//Log.Info( $"Scl2: {scale2}" );
		//Log.Info( $"MPY: {MenuRootPanel.Current.Children.First().Box.Rect.Position.y}" );

		// I chose these offsets through guessing, they're convars to make changing and previewing adjustments easier  
		p.Style.Top = a + offsetY;
		p.Style.Left = b + offsetX;
	}
	public async Task BaseButtonClickUp( Panel p, bool doanim = true )
	{
		MenuPlaySound( "launch_upmenu1" );
		if ( doanim )
		{
			CoolAnimation( p );
			await Task.DelaySeconds( 0.2f );
		}
	}

	[ConCmd.Server]
	static void chnglvlad( string MAP )
	{
		if ( ConsoleSystem.Caller.IsListenServerHost )
		{
			Game.ChangeLevel( MAP );
		}
	}
}
