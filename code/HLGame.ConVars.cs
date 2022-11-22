public partial class HLGame
{
	// this can be accessed from anywhere!

	[ConVar.Replicated] public static string skill { get; set; } = "medium";
	[ConVar.Replicated] public static string sv_gamemode { get; set; } = "campaign";
	[ConVar.Replicated] public static int hl_dm_time { get; set; } = 10;
	[ConVar.Replicated] public static int hl_respawn_time { get; set; } = 3;
	[ConVar.Replicated] public static bool hl_force_multiplayer_gamerules { get; set; } = false;
	public static bool GameIsMultiplayer()
	{
		if ( hl_force_multiplayer_gamerules ) return true;
		var a = true;
		switch ( sv_gamemode )
		{
			case "campaign":
				a = false;
				break;
			case "deathmatch": 
			case "ctf": 
			default:
				a = true;
				break;
		}
		return a;
	}

	[ConVar.Client] public static float hl_hud_scale { get; set; } = 0;
	[ConVar.Replicated] public static bool hl_ragdoll { get; set; } = false;
	[ConVar.Replicated] public static float sv_spray_max_distance { get; set; } = 100;
	[ConVar.Replicated] public static float sv_spray_cooldown { get; set; } = 1;
	[ConVar.Replicated] public static bool hl_extended_mapvote { get; set; } = false;
	[ConVar.Client] public static bool hl_gui_rescale { get; set; } = true;
	[ConVar.Client] public static bool hl_pixelfont { get; set; } = true;
	[ConVar.Client] public static int cc_subtitles { get; set; } = 0;
	[ConVar.Client] public static bool cl_himodels { get; set; } = false;
	[ConVar.Client] public static bool cl_righthand { get; set; } = true;
	[ConVar.ClientData] public static bool hl_classic_explosion { get; set; } = true;
	[ConVar.Replicated] public static bool hl_classic_gibs { get; set; } = true;
	[ConVar.Client] public static bool hl_vr_pointer { get; set; } = false;
	[ConVar.ClientData] public static string hl_spray_icon { get; set; } = "lambda";
	[ConVar.ClientData] public static string hl_spray_colour { get; set; } = "orange";
	[ConVar.ClientData] public static bool hl_classic_flashlight { get; set; } = false;
	[ConVar.ClientData] public static int hl_pm_colour1 { get; set; } = 32;
	[ConVar.ClientData] public static int hl_pm_colour2 { get; set; } = 32;
	[ConVar.ClientData] public static bool hl_viewroll { get; set; } = false;
	[ConVar.Replicated] public static bool hl_legacyfootsteps { get; set; } = true;
	[ConVar.Replicated] public static bool sv_force_physics { get; set; } = false;
	[ConVar.Replicated] public static bool sv_classic_noclip{ get; set; } = false;
	[ConVar.Replicated] public static int hl_vr_crouch_height{ get; set; } = 45;

	// MENU
	[ConVar.Client] public static bool hl_menu_newer_buttons { get; set; } = false;
	[ConVar.Client] public static bool hl_menu_lock_res { get; set; } = false;
	[ConVar.Client] public static bool hl_menu_fourbythree { get; set; } = false;
	// FIXES
	[ConVar.Client] public static bool hl_viewmodel_idle_fix { get; set; } = false;
	[ConVar.Replicated] public static bool hl_fix_ducking_footsteps { get; set; } = false;

	// TWEAKS 
	[ConVar.Replicated] public static bool hl_dynamic_light { get; set; } = false;


}
