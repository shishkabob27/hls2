using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

[UseTemplate("/resource/templates/options.html")]
public class Options : GUIPanel
{
	public bool bCviewroll { get; set; }
	public bool bCsubtitle { get; set; }
	public bool bCliveupdate { get; set; } = true;
	public float fChudScale { get; set; }

	public Options()
	{
		Style.Left = 0;
		Style.Right = 0;
		Style.Top = 0;
		Style.Bottom = 0;
		Focus();
	}
	public override void Close()
	{
        MenuOpen = !MenuOpen;
    }
	public override void Tick()
	{
		base.Tick();
		Drag();

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
    public void openTestdialog()
    {
        var a = Parent.AddChild<Advanced>();
		(a as Advanced).MenuOpen = true;
    }
}
