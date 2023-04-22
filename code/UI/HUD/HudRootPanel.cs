using Sandbox.UI;

public class HudRootPanel : RootPanel
{
	public static HudRootPanel Current;
	public HudRootPanel()
	{
		Current = this;
	}
	protected override void UpdateScale( Rect screenSize )
	{
		if ( HLGame.hl_hud_scale > 0 )
		{
			Scale = HLGame.hl_hud_scale;

		}
		else
		{
			base.UpdateScale( screenSize );
			if ( Screen.Width < 640 ) Scale = 0.5f;
			if ( Screen.Width >= 640 ) Scale = 1.0f;
			if ( Screen.Width >= 2650 ) Scale = 1.50f;
		}
	}

	[Event.Client.BuildInput]
	public void ProcessClientInput()
	{
		if ( Input.Pressed( "Menu" ) )
		{

			ConsoleSystem.Run( "menu" );
		}
	}
}
