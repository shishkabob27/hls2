partial class HLPlayer 
{
	public static void Save(PlayerData data){
		FileSystem.Data.WriteJson("player_data.json", data);
	}

	public static PlayerData Load(){
		return FileSystem.Data.ReadJson<PlayerData>("player_data.json");
	}

}

public class PlayerData
{
	public float HP { get; set; }
	public float Armor { get; set; }
	public bool HEV { get; set; }
}