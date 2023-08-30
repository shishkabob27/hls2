using Sandbox.UI;

public class HudPanel : HudEntity<HudRootPanel>
{
	public static HudPanel Current;

	public Scoreboard Scoreboard { get; set; }

	public Subtitle Subtitle { get; set; }


	public HudPanel()
	{
		Current = this;

		if ( Game.IsClient )
		{
			if ( Game.IsRunningInVR )
			{
				// Use a world panel - we're in VR
				StyleSheet.FromFile( "/Resource/Styles/VR.scss" );
				_ = new VRVitals();
				_ = new VRAmmo();
				_ = new VRInventory();
				_ = new VRChat();
			}
			else
			{
				// Just display the HUD on-screen
				RootPanel.StyleSheet.Load( "/Resource/Styles/hud.scss" );

				var bottomLeftPanel = RootPanel.AddChild<Panel>( "bottom-left" );
				bottomLeftPanel.AddChild<HealthHud>();
				bottomLeftPanel.AddChild<ArmourHud>();

				var bottomRightPanel = RootPanel.AddChild<Panel>( "bottom-right" );
				bottomRightPanel.AddChild<Ammo>();

				RootPanel.AddChild<DamageIndicator>();
				RootPanel.AddChild<Crosshair>();
				RootPanel.AddChild<HitIndicator>();
				RootPanel.AddChild<InventoryBar>();
				RootPanel.AddChild<PickupFeed>();
				RootPanel.AddChild<FlashlightUI>();
				RootPanel.AddChild<ChatBox>();
				RootPanel.AddChild<KillFeed>();
				Scoreboard = RootPanel.AddChild<Scoreboard>();
				RootPanel.AddChild<VoiceList>();
				RootPanel.AddChild<VoiceSpeak>();

				Subtitle = RootPanel.AddChild<Subtitle>();

				if ( HLGame.sv_gamemode == "ctf" )
				{
					RootPanel.AddChild<FlagUI>();
					RootPanel.AddChild<TeamSelector>();
				}

				if ( HLGame.GameIsMultiplayer() )
				{
					RootPanel.AddChild<GameHud>();
				}

				if ( HLGame.sv_gamemode == "campaign" ) RootPanel.AddChild<ChapterText>();
			}
			
		}
	}
}
