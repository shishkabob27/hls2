using Sandbox.UI;

[UseTemplate( "/UI/Menu/NewGame.html" )]
class NewGame : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public void Easy( Panel p )
	{
		BaseButtonClick();
	}
	public void Medium( Panel p )
	{
		BaseButtonClick();
	}
	public void Difficult( Panel p )
	{
		BaseButtonClick();
	}
	public void Cancel( Panel p )
	{
		BaseButtonClickDown( p );
		Parent.AddChild<Menu>();
		Delete();
	}
}
