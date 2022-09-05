using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

[UseTemplate("/resource/templates/options.html")]
public class Options : Panel
{
	public bool MenuOpen;
	public bool bCviewroll;

    [ConVar.Client] public static float cl_rollangle { get; set; } = 0.0f;
    //public Panel MenuPanel;
    //public Panel MenuTabs;
    //public Panel MenuTab;
    //public Panel MainContent;
    //public Label OptionsText;
    //public Label TestText;

    public Options()
	{
        StyleSheet.Load("/resource/styles/options.scss");
        /*
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

        var A = new Button("s");
        MainContent.AddChild(A);

        MainContent.AddChild(Crollangle);

		Crollangle.SetProperty("checked", "AHJ");
        MainContent.AddChild(Cragdoll);
		MainContent.AddChild(Chimodel);
		MainContent.AddChild(Chiaudio);
		MainContent.AddChild(Csubtitile);
		*/
    }

	public override void Tick()
	{
        base.Tick();

        SetClass( "active", MenuOpen );
		cl_rollangle = (bCviewroll ? 0 : 2);
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
