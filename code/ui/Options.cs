using Sandbox.UI;

[UseTemplate( "/resource/templates/options.html" )]
public class Options : GUIPanel
{
	public bool bCviewroll { get; set; }
	public bool bCsubtitle { get; set; }
	public bool bChimodels { get; set; }
	public bool bCguiscale { get; set; } = true;
	public bool bCpixelfont { get; set; } = true;
	public bool bCliveupdate { get; set; } = false;
	public bool bCragdolls { get; set; }
	public bool bColdTorch { get; set; }
	public float fChudScale { get; set; }
	public bool bColdexplosion { get; set; }

	public string bSplayerModel { get; set; }
	public string bSsprayColour { get; set; }
	public string bSsprayIcon { get; set; }

	public bool bCvrpointer { get; set; }

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

		if ( MenuOpen && bCliveupdate )
		{
			updateCvars();
		}
		SetClass( "active", MenuOpen );
	}
	public void updateCvars()
	{
		HLWalkController.cl_rollangle = ( bCviewroll ? 2 : 0 );
		HLGame.hl_hud_scale = fChudScale;
		HLGame.cc_subtitles = bCsubtitle ? 1 : 0;
		HLGame.hl_gui_rescale = bCguiscale;
		HLGame.hl_pixelfont = bCpixelfont;
		HLGame.cl_himodels = bChimodels;
		HLGame.hl_vr_pointer = bCvrpointer;
		HLGame.hl_ragdoll = bCragdolls;
		HLGame.hl_classic_flashlight = bColdTorch;
		HLGame.hl_classic_explosion = bColdexplosion;
		HLGame.hl_spray_icon = bSsprayIcon;
		HLGame.hl_spray_colour = bSsprayColour;
		HLPlayer.hl_pm = bSplayerModel;

	}


	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		if ( input.Pressed( InputButton.Menu ) )
		{
			Position.x = ( Screen.Width / 2 ) - ( Box.Rect.Width / 2 );
			Position.y = ( Screen.Height / 2 ) - ( Box.Rect.Height / 2 );
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
		( a as Advanced ).MenuOpen = true;
	}
}
