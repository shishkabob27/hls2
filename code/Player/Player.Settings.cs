partial class HLPlayer
{
    public static void SaveSettings( PlayerSettingsData data )
    {
        FileSystem.Data.WriteJson( "player_settings.json", data );
    }

    public static PlayerSettingsData LoadSettings()
    {
        return FileSystem.Data.ReadJson<PlayerSettingsData>( "player_settings.json" );
    }
    [ClientRpc]
    public void loadCvars()
    {

        ConsoleSystem.Run( "hl_loadcvar" );
    }
    [ConCmd.Client( "hl_loadcvar", Help = "Update the cvars of the caller" )]
    public static void loadCvar()
    {
        try
        {
            var a = HLPlayer.LoadSettings();
            HLGame.hl_spray_icon = a.SprayImage;
            HLGame.hl_spray_colour = a.SprayColour;
            HLPlayer.hl_pm = a.PlayerModel;
            HLGame.hl_pm_colour1 = a.PlayerModelColour1;
            HLGame.hl_pm_colour2 = a.PlayerModelColour2;

            ConsoleSystem.Run( "hl_spray_icon " + a.SprayImage );
            ConsoleSystem.Run( "hl_spray_colour " + a.SprayColour );
            ConsoleSystem.Run( "hl_pm " + a.PlayerModel );
            ConsoleSystem.Run( "hl_pm_colour1 " + a.PlayerModelColour1 );
            ConsoleSystem.Run( "hl_pm_colour2 " + a.PlayerModelColour2 );
            Log.Info( "Loaded Convars!" );
        }
        catch { }

    }

    [ClientRpc]
    public void saveCvars()
    {

        ConsoleSystem.Run( "hl_savecvar" );
    }

    [ConCmd.Client( "hl_savecvar", Help = "Update the cvars of the caller" )]
    public static void saveCvar()
    {
        Log.Info( "Saved Convars!" );
        var a = new PlayerSettingsData()
        {

            SprayImage = HLGame.hl_spray_icon,
            SprayColour = HLGame.hl_spray_colour,
            PlayerModel = ConsoleSystem.Caller.GetClientData( "hl_pm" ),
            PlayerModelColour1 = HLGame.hl_pm_colour1,
            PlayerModelColour2 = HLGame.hl_pm_colour2
        };
        SaveSettings( a );
    }
}

public class PlayerSettingsData
{
    public bool ClassicTorch { get; set; }
    public bool ClassicExplosions { get; set; }
    public bool ViewRoll { get; set; }
    public string PlayerModel { get; set; }
    public int PlayerModelColour1 { get; set; }
    public int PlayerModelColour2 { get; set; }
    public string SprayImage { get; set; }
    public string SprayColour { get; set; }
}