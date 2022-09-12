public partial class HLHud : HudPanel
{
	[ClientRpc]
	public void OnPlayerDied( HLPlayer player )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}
