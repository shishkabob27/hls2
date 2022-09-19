public partial class HLGame
{
    // this can be accessed from anywhere!

    [ConVar.Replicated] public static string hl_skill { get; set; } = "medium";
    [ConVar.Replicated] public static string hl_gamemode { get; set; } = "campaign";
    [ConVar.Client] public static float hl_hud_scale { get; set; } = 0;
    [ConVar.Replicated] public static bool hl_ragdoll { get; set; } = false;
    [ConVar.Replicated] public static float sv_spray_max_distance { get; set; } = 100;
	[ConVar.Replicated] public static float sv_spray_cooldown { get; set; } = 1;
    [ConVar.Client] public static bool hl_gui_rescale { get; set; } = true;
    [ConVar.Client] public static bool hl_pixelfont  { get; set; } = true;
    [ConVar.Client] public static int cc_subtitles { get; set; } = 0;
    [ConVar.Client] public static bool cl_himodels { get; set; } = false;
    [ConVar.Replicated] public static bool hl_classic_explosion { get; set; } = true;
    [ConVar.Replicated] public static bool hl_classic_gibs{ get; set; } = true;
    [ConVar.Client] public static bool hl_vr_pointer { get; set; } = false;
    [ConVar.Client] public static string hl_spray_icon { get; set; } = "lambda";
    [ConVar.ClientData] public static string hl_spray_color { get; set; } = "orange";
    [ConVar.ClientData] public static bool hl_classic_flashlight { get; set; } = false;
}