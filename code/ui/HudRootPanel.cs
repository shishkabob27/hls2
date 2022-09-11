using Sandbox.UI;

public class HudRootPanel : HudEntity<RootPanel>
{
	public static HudRootPanel Current;

	public Scoreboard Scoreboard { get; set; }

	public Subtitle Subtitle { get; set; }

	public HudRootPanel()
	{
		Current = this;

		if (IsClient)
		{
			if (Global.IsRunningInVR)
			{
				// Use a world panel - we're in VR
				_ = new VRVitals();
				_ = new VRAmmo();
				_ = new VRInventory();
			}

			// Just display the HUD on-screen
			StyleSheet.FromFile("/resource/styles/hud.scss");
			RootPanel.SetTemplate("/resource/templates/hud.html");

			RootPanel.AddChild<DamageIndicator>();
			RootPanel.AddChild<HitIndicator>();
			RootPanel.AddChild<InventoryBar>();
			RootPanel.AddChild<PickupFeed>();
			RootPanel.AddChild<FlashlightUI>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<KillFeed>();
			Scoreboard = RootPanel.AddChild<Scoreboard>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<VoiceSpeaker>();
			RootPanel.AddChild<Subtitle>();
		}
	}


	/*protected override void UpdateScale( Rect screenSize )
	{
		if (HLGame.hl_hud_scale > 0)
		{
            Scale = HLGame.hl_hud_scale;
        } else
		{
            base.UpdateScale(screenSize);
        }
	}*/
}
