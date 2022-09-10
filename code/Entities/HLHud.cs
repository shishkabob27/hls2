public partial class HLHud : HudRootPanel
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
