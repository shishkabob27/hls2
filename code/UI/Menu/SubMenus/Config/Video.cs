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
	public bool bChimodels { get; set; }
	public bool bCragdolls { get; set; }
	public bool bColdTorch { get; set; }
	public bool bColdGibs { get; set; }
	public bool bColdexplosion { get; set; }
	public bool bCWONWeaponBob { get; set; }
	public float fChudScale { get; set; }

	void getCvars()
	{
		bCviewroll = HLGame.hl_viewroll;
		bChimodels = HLGame.cl_himodels;
		bCragdolls = HLGame.hl_ragdoll;
		bColdTorch = HLGame.hl_classic_flashlight;
		bColdGibs = HLGame.hl_classic_gibs;
		bColdexplosion = HLGame.hl_classic_explosion;
		//bCWONWeaponBob = FirstPersonCamera.hl_won_viewbob;
		fChudScale = HLGame.hl_hud_scale;
	}

	bool oldhimdl = false;
	public void updateCvars()
	{
		oldhimdl = HLGame.cl_himodels;
		HLGame.hl_viewroll = bCviewroll;
		HLGame.cl_himodels = bChimodels;
		HLGame.hl_ragdoll = bCragdolls;
		HLGame.hl_classic_flashlight = bColdTorch;
		HLGame.hl_classic_explosion = bColdexplosion;
		HLGame.hl_classic_gibs = bColdGibs;
		HLGame.hl_hud_scale = fChudScale;

		ConsoleSystem.Run( "hl_viewroll " + bCviewroll );
		ConsoleSystem.Run( "cl_himodels " + (bChimodels ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_flashlight " + (bColdTorch ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_explosion " + (bColdexplosion ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_gibs " + (bColdGibs ? 1 : 0) );
		ConsoleSystem.Run( "hl_won_viewbob " + bCWONWeaponBob );
		ConsoleSystem.Run( "hl_hud_scale " + fChudScale );
		updtasync();
	}
	public async void updtasync()
	{
		await GameTask.DelaySeconds( 0.1f );
		ConsoleSystem.Run( "hl_savecvar" );

		if ( oldhimdl != bChimodels )
		{
			if ( Game.LocalPawn is not HLPlayer pl ) return;

			if ( pl.ActiveChild is not Weapon wp ) return;
			wp.ActiveStart( pl );
		}
	}
}
