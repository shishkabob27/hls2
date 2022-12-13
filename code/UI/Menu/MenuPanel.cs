using Sandbox;

public class MenuPanel : HudEntity<MenuRootPanel>
{
	public static MenuPanel Current;


	public MenuPanel()
	{
		Current = this;
	}
}
