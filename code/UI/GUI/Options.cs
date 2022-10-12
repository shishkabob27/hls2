using Sandbox.UI;

[UseTemplate( "/resource/templates/options.html" )]
public class Options : GUIPanel
{
	public bool bCviewroll { get; set; } = false;
	public bool bCsubtitle { get; set; }
	public bool bChimodels { get; set; }
	public bool bCguiscale { get; set; } = true;
	public bool bCpixelfont { get; set; } = true;
	public bool bCliveupdate { get; set; } = false;
	public bool bCragdolls { get; set; }
	public bool bColdTorch { get; set; }
	public bool bColdGibs { get; set; }
	public float fChudScale { get; set; }
	public float fCpmColour1 { get; set; }
	public float fCpmColour2 { get; set; }
	public bool bColdexplosion { get; set; }

	public string bSplayerModel { get; set; } = "player";
	public string bSsprayColour { get; set; } = "orange";
	public string bSsprayIcon { get; set; } = "lambda";
	public string Shudstyle { get; set; } = "hl1";

	public bool bCvrpointer { get; set; }

	public Options()
	{
		Style.Left = 0;
		Style.Right = 0;
		Style.Top = 0;
		Style.Bottom = 0;

		getCvars();
		Focus();
	}
	void getCvars()
	{
		bCviewroll = HLGame.hl_viewroll;
		fChudScale = HLGame.hl_hud_scale;
		bCsubtitle = HLGame.cc_subtitles == 1;
		bCguiscale = HLGame.hl_gui_rescale;
		bCpixelfont = HLGame.hl_pixelfont;
		bChimodels = HLGame.cl_himodels;
		bCvrpointer = HLGame.hl_vr_pointer;
		bCragdolls = HLGame.hl_ragdoll;
		bColdTorch = HLGame.hl_classic_flashlight;
		bColdGibs = HLGame.hl_classic_gibs;
		bColdexplosion = HLGame.hl_classic_explosion;
		bSsprayIcon = HLGame.hl_spray_icon;
		bSsprayColour = HLGame.hl_spray_colour;
		bSplayerModel = HLPlayer.hl_pm;
		fCpmColour1 = HLGame.hl_pm_colour1;
		fCpmColour2 = HLGame.hl_pm_colour2;
		Shudstyle = HLGame.hl_hud_style;
	}
	public override void Close()
	{
		Delete();
	}
	bool oldm = false;
	public override void Tick()
	{
		base.Tick();
		Drag();

		if ( MenuOpen )
		{
			if ( oldm != MenuOpen && Box.Rect.Width != 0 )
			{
				oldm = MenuOpen;
				Position.x = (Screen.Width / 2) - (Box.Rect.Width / 2);
				Position.y = (Screen.Height / 2) - (Box.Rect.Height / 2);
			}
			if ( bCliveupdate ) updateCvars();
		}
		SetClass( "active", MenuOpen );
	}
	bool oldhimdl = false;
	public void updateCvars()
	{
		oldhimdl = HLGame.cl_himodels;
		HLGame.hl_viewroll = bCviewroll;
		HLGame.hl_hud_scale = fChudScale;
		HLGame.cc_subtitles = bCsubtitle ? 1 : 0;
		HLGame.hl_gui_rescale = bCguiscale;
		HLGame.hl_pixelfont = bCpixelfont;
		HLGame.cl_himodels = bChimodels;
		HLGame.hl_vr_pointer = bCvrpointer;
		HLGame.hl_ragdoll = bCragdolls;
		HLGame.hl_classic_flashlight = bColdTorch;
		HLGame.hl_classic_explosion = bColdexplosion;
		HLGame.hl_classic_gibs = bColdGibs;
		HLGame.hl_spray_icon = bSsprayIcon;
		HLGame.hl_spray_colour = bSsprayColour;
		HLPlayer.hl_pm = bSplayerModel;
		HLGame.hl_pm_colour1 = (int)fCpmColour1;
		HLGame.hl_pm_colour2 = (int)fCpmColour2;
		HLGame.hl_hud_style = Shudstyle;

		ConsoleSystem.Run( "hl_viewroll " + bCviewroll );
		ConsoleSystem.Run( "hl_hud_scale " + fChudScale );
		ConsoleSystem.Run( "cc_subtitles " + (bCsubtitle ? 1 : 0) );
		ConsoleSystem.Run( "hl_gui_rescale " + (bCguiscale ? 1 : 0) );
		ConsoleSystem.Run( "hl_pixelfont " + (bCpixelfont ? 1 : 0) );
		ConsoleSystem.Run( "cl_himodels " + (bChimodels ? 1 : 0) );
		ConsoleSystem.Run( "hl_vr_pointer " + (bCvrpointer ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_flashlight " + (bColdTorch ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_explosion " + (bColdexplosion ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_gibs " + (bColdGibs ? 1 : 0) );
		ConsoleSystem.Run( "hl_spray_icon " + bSsprayIcon );
		ConsoleSystem.Run( "hl_spray_colour " + bSsprayColour );
		ConsoleSystem.Run( "hl_pm " + bSplayerModel );
		ConsoleSystem.Run( "hl_pm_colour1 " + fCpmColour1 );
		ConsoleSystem.Run( "hl_pm_colour2 " + fCpmColour2 );
		ConsoleSystem.Run( "hl_hud_style " + Shudstyle );
		updtasync();

		/*
		*/
	}
	public async void updtasync()
	{
		await GameTask.DelaySeconds( 0.1f );
		ConsoleSystem.Run( "hl_updatepm" );
		ConsoleSystem.Run( "hl_savecvar" );

		if ( oldhimdl != bChimodels )
		{
			if ( Local.Pawn is not HLPlayer pl ) return;

			if ( pl.ActiveChild is not HLWeapon wp ) return;
			wp.ActiveStart( pl );
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
