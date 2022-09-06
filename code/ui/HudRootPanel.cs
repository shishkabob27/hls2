using Sandbox.UI;

public class HudRootPanel : RootPanel
{
	public static HudRootPanel Current;

	public Scoreboard Scoreboard { get; set; }

	public Subtitle Subtitle { get; set; }

	public HudRootPanel()
	{
		Current = this;

		StyleSheet.Load( "/resource/styles/hud.scss" );
		SetTemplate( "/resource/templates/hud.html" );

		AddChild<DamageIndicator>();
		AddChild<HitIndicator>();

		AddChild<InventoryBar>();
		AddChild<PickupFeed>();

		AddChild<FlashlightUI>();

		AddChild<ChatBox>();
		AddChild<KillFeed>();
		Scoreboard = AddChild<Scoreboard>();
		AddChild<VoiceList>();
		AddChild<VoiceSpeaker>();
		AddChild<Subtitle>();

	}

	public override void Tick()
	{
		base.Tick();
	}

	protected override void UpdateScale( Rect screenSize )
	{
		if (HLGame.hl_hud_scale > 0)
		{
            Scale = HLGame.hl_hud_scale;
        } else
		{
            base.UpdateScale(screenSize);
        }
	}
}
