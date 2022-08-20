using Sandbox.UI;
using Sandbox.UI.Construct;

public class Menu : Panel
{

	public bool MenuOpen;

	public Panel MenuPanel;
	public Panel MenuTabs;
	public Panel MainContent;
	public Label OptionsText;
	public Label TestText;

	public Menu()
	{
		MenuPanel = Add.Panel("menupanel");
		OptionsText = Add.Label("Options", "optionstext");
		MenuTabs = Add.Panel("menutabs");
		MainContent = Add.Panel("maincontent");

		TestText = Add.Label("TODO: Playermodels, Sprays, Admin settings.");

		
		MenuPanel.AddChild(OptionsText);
		MenuPanel.AddChild(MenuTabs);
		MenuPanel.AddChild(MainContent);
		MainContent.AddChild(TestText);
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
