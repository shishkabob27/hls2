using Sandbox.UI;



public class MenuRootPanel : RootPanel
{
	public static MenuRootPanel Current;
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
