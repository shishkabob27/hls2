public partial class HLGame
{
    // this can be accessed from anywhere!

    [ConVar.Replicated] public static string hl_skill { get; set; } = "medium";
    [ConVar.Replicated] public static string hl_gamemode { get; set; } = "campaign";
    [ConVar.Replicated] public static float hl_hud_scale { get; set; } = 0;
    [ConVar.Client] public static bool hl_gui_rescale { get; set; } = true;
    [ConVar.Client] public static bool hl_pixelfont  { get; set; } = true;
    [ConVar.Client] public static int cc_subtitles { get; set; } = 0;
    
}