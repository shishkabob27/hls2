using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Template.html" )]
class Template : BaseMenuScreen
{
	public void One( Panel p )
	{
	}
	public void Two( Panel p )
	{
	}
	public void Three( Panel p )
	{
	}
	public void Four( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "Quit" );
	}
}
