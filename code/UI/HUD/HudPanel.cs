using Sandbox.UI;

public class HudPanel : HudEntity<HudRootPanel>
{
    public static HudPanel Current;

    public Scoreboard Scoreboard { get; set; }

    public Subtitle Subtitle { get; set; }


    public HudPanel()
    {
        Current = this;

        if ( IsClient )
        {
            if ( Global.IsRunningInVR )
            {
                // Use a world panel - we're in VR
                _ = new VRVitals();
                _ = new VRAmmo();
                _ = new VRInventory();
                _ = new VRChat();
            }

            // Just display the HUD on-screen
            StyleSheet.FromFile( "/resource/styles/hud.scss" );
            RootPanel.SetTemplate( "/resource/templates/hud.html" );

            RootPanel.AddChild<DamageIndicator>();
            RootPanel.AddChild<Crosshair>();
            RootPanel.AddChild<HitIndicator>();
            RootPanel.AddChild<InventoryBar>();
            RootPanel.AddChild<PickupFeed>();
            RootPanel.AddChild<FlashlightUI>();
            RootPanel.AddChild<Chat>();
            RootPanel.AddChild<KillFeed>();
            Scoreboard = RootPanel.AddChild<Scoreboard>();
            RootPanel.AddChild<VoiceList>();
            RootPanel.AddChild<VoiceSpeaker>();
            RootPanel.AddChild<Subtitle>();

            if ( HLGame.gamemode == "ctf" )
            {
                RootPanel.AddChild<FlagUI>();
            }

            if (HLGame.gamemode == "campaign") RootPanel.AddChild<ChapterText>();


        }
    }
}
