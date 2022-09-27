﻿partial class HLPlayer
{
	public static void SaveSettings( PlayerSettingsData data )
	{
		FileSystem.Data.WriteJson( "player_settings.json", data );
	}

	public static PlayerSettingsData LoadSettings()
	{
		return FileSystem.Data.ReadJson<PlayerSettingsData>( "player_settings.json" );
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