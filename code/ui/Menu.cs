using Sandbox.UI;
using Sandbox.UI.Construct;

public class Menu : Panel
{

	public bool MenuOpen;

	public Panel MenuPanel;
	public Label TestText;
	

	public Menu()
	{
		MenuPanel = Add.Panel("menupanel");
		TestText = Add.Label("TODO: Playermodels, Sprays, Admin settings.");
		MenuPanel.AddChild(TestText);
	}

	public override void Tick()
	{
        base.Tick();

        SetClass( "active", MenuOpen );

	}

	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		if (input.Pressed(InputButton.Menu))
		{
			MenuOpen = !MenuOpen;
		}
	}
}
