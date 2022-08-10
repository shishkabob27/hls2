public partial class HLHud : HudEntity<HudRootPanel>
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
