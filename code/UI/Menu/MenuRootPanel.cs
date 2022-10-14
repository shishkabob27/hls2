using Sandbox.UI;



public class MenuRootPanel : RootPanel
{
	public static MenuRootPanel Current;

	[Event.Tick.Client]
	void MenuTick()
	{
		SetClass( "WantsNewButton", HLGame.hl_menu_newer_buttons );
	}
	public MenuRootPanel()
	{
		StyleSheet.Load( "UI/Menu/Menu.scss" );
		Current = this;
		AddChild<Menu>();
	}
	protected override void UpdateScale( Rect screenSize )
	{
		base.UpdateScale( screenSize );
	}
}
