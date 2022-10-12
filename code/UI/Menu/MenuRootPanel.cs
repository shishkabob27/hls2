using Sandbox.UI;


[UseTemplate( "/UI/Menu/Menu.html" )]
public class MenuRootPanel : RootPanel
{
	public static MenuRootPanel Current;
	public MenuRootPanel()
	{
		Current = this;
	}
	protected override void UpdateScale( Rect screenSize )
	{
		base.UpdateScale( screenSize );
	}
}
