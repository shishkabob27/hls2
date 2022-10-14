using Sandbox.UI;



public class MenuRootPanel : RootPanel
{
	public static MenuRootPanel Current;



	[Event.Tick.Client]
	void MenuTick()
	{
		SetClass( "WantsNewButton", HLGame.hl_menu_newer_buttons );
		SetClass( "LockRes", HLGame.hl_menu_lock_res );


	}
	public MenuRootPanel()
	{
		StyleSheet.Load( "UI/Menu/Menu.scss" );
		Current = this;
		AddChild<Menu>();
	}
	protected override void UpdateScale( Rect screenSize )
	{
		if ( HLGame.hl_menu_lock_res )
		{
			Scale = 0.44444445f;
		}
		else
		{

			base.UpdateScale( screenSize );
		}
	}
}
