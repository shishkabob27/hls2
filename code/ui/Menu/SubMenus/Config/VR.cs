using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Config/VR.html" )]
class VR : BaseMenuScreen
{
	public void Done( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Config>();
		BaseButtonClickDown( p, a, true, "Virtual Reality" );
	}
}
