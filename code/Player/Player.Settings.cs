﻿partial class HLPlayer
{
	public static void SaveSettings( PlayerData data )
	{
		FileSystem.Data.WriteJson( "player_settings.json", data );
	}

	public static PlayerData LoadSettings()
	{
		return FileSystem.Data.ReadJson<PlayerData>( "player_settings.json" );
	}

}

public class PlayerSettingsData
{
	public bool ClassicTorch { get; set; }
	public bool ClassicExplosions { get; set; }
	public bool ViewRoll { get; set; }
	public string PlayerModel { get; set; }
	public float PlayerModelColour1 { get; set; }
	public float PlayerModelColour2 { get; set; }
}