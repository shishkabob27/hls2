using Sandbox.UI;
using Sandbox.UI.Construct;

public class Menu : Panel
{
	public bool MenuOpen;

	public Panel MenuPanel;
	public Panel MenuTabs;
	public Panel MenuTab;
	public Panel MainContent;
	public Label OptionsText;
	public Label TestText;

	public Menu()
	{
		MenuPanel = Add.Panel("menupanel");
		OptionsText = Add.Label("Options", "optionstext");
		MenuTabs = Add.Panel("menutabs");
		MenuTab = Add.Panel("menutab");
		MainContent = Add.Panel("maincontent");

		TestText = Add.Label("General");

		MenuPanel.AddChild(OptionsText);
		MenuPanel.AddChild(MenuTabs);
		MenuTabs.AddChild(MenuTab);
		MenuPanel.AddChild(MainContent);
		MenuTab.AddChild(TestText);

		//move this to its own file
		var Crollangle = new Checkbox();
		Crollangle.LabelText = "Enable HL1 WON Viewroll";

		var Cragdoll = new Checkbox();
		Cragdoll.LabelText = "Enable HL1 death animations";

		var Chimodel = new Checkbox();
		Chimodel.LabelText = "Enable HD models";

		var Chiaudio = new Checkbox();
		Chiaudio.LabelText = "Enable HD audio";

		var Csubtitile = new Checkbox();
		Csubtitile.LabelText = "Enable subtitles";

		MainContent.AddChild(Crollangle);
		MainContent.AddChild(Cragdoll);
		MainContent.AddChild(Chimodel);
		MainContent.AddChild(Chiaudio);
		MainContent.AddChild(Csubtitile);
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
