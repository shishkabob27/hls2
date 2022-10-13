using Sandbox.UI;

[UseTemplate( "/UI/Menu/Config.html" )]
class Config : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public void Audio( Panel p )
	{

	}
	public void Video( Panel p )
	{

	}
	public void Done( Panel p )
	{
		BaseButtonClickDown( p );
		Parent.AddChild<Menu>();
		Delete();
	}
}
