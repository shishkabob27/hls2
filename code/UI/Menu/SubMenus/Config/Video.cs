using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Config/Video.html" )]
class Video : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public Video()
	{
		getCvars();
	}


	public void Done( Panel p )
	{
		updateCvars();
		Delete();
		var a = Parent.AddChild<Config>();
		BaseButtonClickDown( p, a, true, "Video" );
	}

	public bool bCviewroll { get; set; } = false;
	public bool bCsubtitle { get; set; }
	public bool bChimodels { get; set; }
	public bool bCragdolls { get; set; }
	public bool bColdTorch { get; set; }
	public bool bColdGibs { get; set; }
	public bool bColdexplosion { get; set; }
	public bool bCWONWeaponBob { get; set; }

	void getCvars()
	{
		bCviewroll = HLGame.hl_viewroll;
		bCsubtitle = HLGame.cc_subtitles == 1;
		bChimodels = HLGame.cl_himodels;
		bCragdolls = HLGame.hl_ragdoll;
		bColdTorch = HLGame.hl_classic_flashlight;
		bColdGibs = HLGame.hl_classic_gibs;
		bColdexplosion = HLGame.hl_classic_explosion;
		bCWONWeaponBob = FirstPersonCamera.hl_won_viewbob;
	}

	bool oldhimdl = false;
	public void updateCvars()
	{
		oldhimdl = HLGame.cl_himodels;
		HLGame.hl_viewroll = bCviewroll;
		HLGame.cc_subtitles = bCsubtitle ? 1 : 0;
		HLGame.cl_himodels = bChimodels;
		HLGame.hl_ragdoll = bCragdolls;
		HLGame.hl_classic_flashlight = bColdTorch;
		HLGame.hl_classic_explosion = bColdexplosion;
		HLGame.hl_classic_gibs = bColdGibs;

		ConsoleSystem.Run( "hl_viewroll " + bCviewroll );
		ConsoleSystem.Run( "cc_subtitles " + (bCsubtitle ? 1 : 0) );
		ConsoleSystem.Run( "cl_himodels " + (bChimodels ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_flashlight " + (bColdTorch ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_explosion " + (bColdexplosion ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_gibs " + (bColdGibs ? 1 : 0) );
		ConsoleSystem.Run( "hl_won_viewbob " + bCWONWeaponBob );
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
}
