using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

[UseTemplate("/resource/templates/options.html")]
public class Options : Panel
{
    public int ZIndex = 1;
	public bool Dragging;
    public bool MenuOpen;
	public bool bCviewroll { get; set; }
	public bool bCsubtitle { get; set; }
	public bool bCliveupdate { get; set; }
	public float fChudScale { get; set; }
	public Panel MenuPanel { get; set; }

	//[ConVar.Client] public static float cl_rollangle { get; set; } = 0.0f;
	//public Panel MenuPanel;
	//public Panel MenuTabs;
	//public Panel MenuTab;
	//public Panel MainContent;
	//public Label OptionsText;
	//public Label TestText;


	public Options()
	{
		StyleSheet.Load("/resource/styles/options.scss");
		Style.Left = 0;
		Style.Right = 0;
		Style.Top = 0;
		Style.Bottom = 0;
		//AcceptsFocus = true;
		Focus();
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
		if (MenuOpen && bCliveupdate)
		{
			updateCvars();
		}
        SetClass("active", MenuOpen);
    }
	public void updateCvars()
	{

        HLWalkController.cl_rollangle = (bCviewroll? 2 : 0);
		HLGame.hl_hud_scale = fChudScale;
        HLGame.cc_subtitles = (bCsubtitle? 1 : 0);
	}


	[Event.BuildInput]
	public void ProcessClientInput(InputBuilder input)
	{
		if (input.Pressed(InputButton.Menu))
        {
            Style.Left = (Screen.Width / 2) - (Box.Rect.Width / 2);
            Style.Top = (Screen.Height / 2) - (Box.Rect.Height / 2); 
			MenuOpen = !MenuOpen;
		}
	}
    public void OK()
    {
		updateCvars();
		Close();
    }
    public void Close()
	{
		MenuOpen = !MenuOpen;
	}
	float xoff = 0;
	float yoff = 0;
	public void Drag()
	{
		if (!Dragging) return;
		Style.Left = Parent.MousePosition.x - xoff;
		Style.Top = Parent.MousePosition.y - yoff;
	}
	public void down()
	{
		Focus();
		Parent.SortChildren(x => x.HasFocus ? 0 : 1);
		if (Style.Left == null)
		{
            Style.Left = 0;
            Style.Top = 0;
        }
		xoff = (float)(Parent.MousePosition.x - Box.Rect.Left);
        yoff = (float)(Parent.MousePosition.y - Box.Rect.Top);
        Dragging = true;
	}
	public void up()
    {
        Dragging = false;
    }
}
