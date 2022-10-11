[Library( "env_fade" )]
[HammerEntity]
[EditorSprite( "editor/snd_event.vmat" )]
[Title( "env_fade" ), Category( "Legacy" ), Icon( "toggle_on" )]
public partial class env_fade : Entity
{
	[Property( "rendercolor" ), Title( "Colour" ), Net]
	public Color FadeColour { get; set; }

	[Property( "duration" ), Title( "Duration" ), Net]
	public float Duration { get; set; }

	[Property( "holdtime" ), Title( "Hold time" ), Net]
	public float HoldTime { get; set; }

	float DurationCL;
	float HoldTimeCL;
	Color FadeColourCL;

	float TimeCurrent;
	FadeRenderHook hook;

	public override void Spawn()
	{
		Transmit = TransmitType.Always;
		base.Spawn();
	}

	[Input]
	public void Fade()
	{
		rpcfade( Duration, HoldTime, FadeColour );
	}
	[ClientRpc]
	void rpcfade( float dur, float hldt, Color fdCl )
	{
		DurationCL = dur;
		HoldTimeCL = hldt;
		FadeColourCL = fdCl;

		hook = Map.Camera.FindOrCreateHook<FadeRenderHook>();
		hook.MyColor = FadeColourCL.WithAlpha( 1 );
		hook.TimeCurrentF = 0;
		hook.DurationF = DurationCL;
		hook.HoldTimeF = HoldTimeCL;

	}
}

[SceneCamera.AutomaticRenderHook]
public partial class FadeRenderHook : RenderHook
{
	RenderAttributes attributes = new();
	public Color MyColor { get; set; }

	public float DurationF;
	public float HoldTimeF;
	public float TimeCurrentF;
	public override void OnStage( SceneCamera target, Stage renderStage )
	{
		Enabled = true;
		if ( renderStage == Stage.AfterPostProcess )
		{
			var a = Material.UI.Basic;
			a.OverrideTexture( "Texture", Texture.White );
			attributes.Set( "Texture", Texture.White );
			Graphics.DrawQuad( new Rect( 0, 0, Screen.Width, Screen.Height ),
				a,
				MyColor.WithAlpha( (1 + HoldTimeF) - (TimeCurrentF / DurationF) ), attributes );
			TimeCurrentF += Time.Delta;
		}
	}
}
