public partial class HLHud : HudPanel
{
	[ClientRpc]
	public void OnPlayerDied( HLPlayer player )
	{
		Game.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Game.AssertClient();
	}
}
