using Sandbox.UI;

[UseTemplate( "/Resource/templates/options.html" )]
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
	public bool bCforcePhysics { get; set; }
	public float fChudScale { get; set; }
	public float fCpmColour1 { get; set; }
	public float fCpmColour2 { get; set; }
	public bool bColdexplosion { get; set; }
	public bool bCFixCrouchFootstep { get; set; }
	public bool bCFixMysteryViewbob { get; set; }
	public bool bCFixViewmodelIdle { get; set; }
	public bool bCWONWeaponBob { get; set; }
	public bool bCenableBhop { get; set; }
	public bool bCenableAutojump { get; set; }
	public float fCtimeLimit { get; set; }

	public string bSplayerModel { get; set; } = "player";
	public string bSsprayColour { get; set; } = "orange";
	public string bSsprayIcon { get; set; } = "lambda";
	public string Shudstyle { get; set; } = "hl1";
	public string bSgameMode { get; set; } = "campaign";
	public string bSskill { get; set; } = "campaign";

	public bool bCvrpointer { get; set; }

	public DropDown PMDropdown { get; set; }

	public Panel pmImage { get; set; }

	public Options()
	{
		Style.Left = 0;
		Style.Right = 0;
		Style.Top = 0;
		Style.Bottom = 0;

		var pm = ResourceLibrary.GetAll<Playermodel>();

		foreach ( Playermodel newpm in pm )
		{
			var _ = new Option( newpm.ResourceName, newpm.ResourceName );
			PMDropdown.Options.Add( _ );
		}
		
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
		bCforcePhysics = HLGame.sv_force_physics;
		bColdexplosion = HLGame.hl_classic_explosion;
		bSsprayIcon = HLGame.hl_spray_icon;
		bSsprayColour = HLGame.hl_spray_colour;
		bSplayerModel = HLPlayer.hl_pm;
		fCpmColour1 = HLGame.hl_pm_colour1;
		fCpmColour2 = HLGame.hl_pm_colour2;
		bCFixCrouchFootstep = HLGame.hl_fix_ducking_footsteps;
		//bCFixMysteryViewbob = FirstPersonCamera.hl_fix_mystery_viewbob_code;
		bCFixViewmodelIdle = HLGame.hl_viewmodel_idle_fix;
		//bCWONWeaponBob = FirstPersonCamera.hl_won_viewbob;
		bCenableBhop = WalkController.sv_enablebunnyhopping;
		bCenableAutojump = WalkController.sv_autojump;
		fCtimeLimit = HLGame.hl_dm_time;
		bSgameMode = HLGame.sv_gamemode;
		bSskill = HLGame.skill;
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

		pmImage.Style.SetBackgroundImage( $"/ui/playermodel/{bSplayerModel}.png" );
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
		HLGame.sv_force_physics = bCforcePhysics;
		HLGame.hl_spray_icon = bSsprayIcon;
		HLGame.hl_spray_colour = bSsprayColour;
		HLPlayer.hl_pm = bSplayerModel;
		HLGame.hl_pm_colour1 = (int)fCpmColour1;
		HLGame.hl_pm_colour2 = (int)fCpmColour2;
		WalkController.sv_enablebunnyhopping = bCenableBhop;
		WalkController.sv_autojump = bCenableAutojump;
		HLGame.hl_dm_time = ((int)fCtimeLimit);
		HLGame.sv_gamemode = bSgameMode;
		HLGame.skill = bSskill;

		//FirstPersonCamera.hl_won_viewbob = bCWONWeaponBob;
		HLGame.hl_fix_ducking_footsteps = bCFixCrouchFootstep;
		//FirstPersonCamera.hl_fix_mystery_viewbob_code = bCFixMysteryViewbob;
		HLGame.hl_viewmodel_idle_fix = bCFixViewmodelIdle;

		ConsoleSystem.Run( "hl_viewroll " + bCviewroll );
		ConsoleSystem.Run( "hl_hud_scale " + fChudScale );
		ConsoleSystem.Run( "cc_subtitles " + (bCsubtitle ? 1 : 0) );
		ConsoleSystem.Run( "hl_gui_rescale " + (bCguiscale ? 1 : 0) );
		ConsoleSystem.Run( "hl_pixelfont " + (bCpixelfont ? 1 : 0) );
		ConsoleSystem.Run( "cl_himodels " + (bChimodels ? 1 : 0) );
		ConsoleSystem.Run( "hl_vr_pointer " + (bCvrpointer ? 1 : 0) );
		ConsoleSystem.Run( "hl_ragdoll " + (bCragdolls ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_flashlight " + (bColdTorch ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_explosion " + (bColdexplosion ? 1 : 0) );
		ConsoleSystem.Run( "hl_classic_gibs " + (bColdGibs ? 1 : 0) );
		ConsoleSystem.Run( "sv_force_physics " + (bCforcePhysics ? 1 : 0) );
		ConsoleSystem.Run( "hl_spray_icon " + bSsprayIcon );
		ConsoleSystem.Run( "hl_spray_colour " + bSsprayColour );
		ConsoleSystem.Run( "hl_pm " + bSplayerModel );
		ConsoleSystem.Run( "hl_pm_colour1 " + fCpmColour1 );
		ConsoleSystem.Run( "hl_pm_colour2 " + fCpmColour2 );
		ConsoleSystem.Run( "hl_hud_style " + Shudstyle );
		ConsoleSystem.Run( "hl_fix_ducking_footsteps " + bCFixCrouchFootstep );
		ConsoleSystem.Run( "hl_fix_mystery_viewbob_code " + bCFixMysteryViewbob );
		ConsoleSystem.Run( "hl_viewmodel_idle_fix " + bCFixViewmodelIdle );
		ConsoleSystem.Run( "hl_won_viewbob " + bCWONWeaponBob );
		ConsoleSystem.Run( "sv_enablebunnyhopping " + (bCenableBhop ? 1 : 0) );
		ConsoleSystem.Run( "sv_autojump " + (bCenableAutojump ? 1 : 0) );
		ConsoleSystem.Run( "hl_dm_time " + ((int)fCtimeLimit) );
		ConsoleSystem.Run( "sv_gamemode " + bSgameMode );
		ConsoleSystem.Run( "skill " + bSskill );
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
			if ( Game.LocalPawn is not HLPlayer pl ) return;

			if ( pl.ActiveChild is not Weapon wp ) return;
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
	public void restartGame()
	{
		updateCvars();
		ConsoleSystem.Run( "reset_game" );
	}
	public void spawnScientist()
	{
		ConsoleSystem.Run( "spawnScientist" );
	}
	public void spawnBarney()
	{
		ConsoleSystem.Run( "spawnBarney" );
	}
	public void spawnHeadcrab()
	{
		ConsoleSystem.Run( "spawnHeadcrab" );
	}
	public void spawnGman()
	{
		ConsoleSystem.Run( "spawnGman" );
	}
	public void spawnZombie()
	{
		ConsoleSystem.Run( "spawnZombie" );
	}
}
