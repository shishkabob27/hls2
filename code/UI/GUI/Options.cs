using Sandbox.UI;

[UseTemplate( "/resource/templates/options.html" )]
public class Options : GUI
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

	public string bSplayerModel { get; set; } = "player";
	public string bSsprayColour { get; set; } = "orange";
	public string bSsprayIcon { get; set; } = "lambda";

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
		ConsoleSystem.Run( "hl_pm " + bSplayerModel );
		ConsoleSystem.Run( "cl_rollangle " + ( bCviewroll ? 2 : 0 ) );
		ConsoleSystem.Run( "hl_hud_scale " + fChudScale );
		ConsoleSystem.Run( "cc_subtitles " + ( bCsubtitle ? 1 : 0 ) );
		ConsoleSystem.Run( "hl_gui_rescale " + ( bCguiscale ? 1 : 0 ) );
		ConsoleSystem.Run( "hl_pixelfont " + ( bCpixelfont ? 1 : 0 ) );
		ConsoleSystem.Run( "cl_himodels " + ( bChimodels ? 1 : 0 ) );
		ConsoleSystem.Run( "hl_vr_pointer " + ( bCvrpointer ? 1 : 0 ) );
		ConsoleSystem.Run( "hl_classic_flashlight " + ( bColdTorch ? 1 : 0 ) );
		ConsoleSystem.Run( "hl_classic_explosion " + ( bColdexplosion ? 1 : 0 ) );
		ConsoleSystem.Run( "hl_spray_icon " + bSsprayIcon );
		ConsoleSystem.Run( "hl_spray_colour " + bSsprayColour );
		updtasync();
		/*
		WalkController.cl_rollangle = ( bCviewroll ? 2 : 0 );
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
		*/

	}
	public async void updtasync()
	{
		await GameTask.DelaySeconds( 0.1f );
		ConsoleSystem.Run( "hl_updatepm" );
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
