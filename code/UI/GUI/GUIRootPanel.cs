using Sandbox.UI;

[UseTemplate( "/resource/templates/ingamemenu.html" )]
public class GUIRootPanel : RootPanel
{
	public bool MenuOpen;
	public static GUIRootPanel Current;
	public float OverrideScale { get; set; } = 0;

	public GUIRootPanel()
	{
		AcceptsFocus = true;
		Current = this;
		StyleSheet.Load( "resource/styles/GUI.scss" );
		Style.ZIndex = 100;
		Focus();
	}

	public override void Tick()
	{
		base.Tick();
	}

	protected override void UpdateScale( Rect screenSize )
	{
		Scale = 1;
		if ( !HLGame.hl_gui_rescale ) return;
		if ( OverrideScale > 0 )
		{
			Scale = OverrideScale;
			return;
		}
		if ( Screen.Width > 1920 ) Scale = 1.50f;
		if ( Screen.Height > 2650 ) Scale = 2.00f;
	}
	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.Menu ) )
		{

			ConsoleSystem.Run( "open_options" );
		}
	}
	[ConCmd.Client]
	static void open_options()
	{
		if ( Current.Children.OfType<Options>().Count() == 0 )
		{
			var a = Current.AddChild<Options>();
			a.MenuOpen = true;
			a.Focus();

			// create it offscreen, it'll fix itself, this is so we do not see it snap into place.
			a.Position.x = (Screen.Width * 3);
			a.Position.y = (Screen.Height * 3);

		}
		else
		{
			Current.Children.OfType<Options>().First().Delete();
		}
	}
}
