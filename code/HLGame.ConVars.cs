public partial class HLGame : Game
{
    // this can be accessed from anywhere!

    [ConVar.Replicated] public static string hl_difficulty { get; set; } = "medium";
    [ConVar.Replicated] public static string hl_gamemode { get; set; } = "campaign";
    [ConVar.Replicated] public static float hl_hud_scale { get; set; } = 0;
    [ConVar.Client] public static int cc_subtitles { get; set; } = 0;
}