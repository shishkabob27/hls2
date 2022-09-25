using Sandbox.UI;

public class HudRootPanel : RootPanel
{
    public static HudRootPanel Current;
    public HudRootPanel()
    {
        Current = this;
    }
    protected override void UpdateScale( Rect screenSize )
    {
        if ( HLGame.hl_hud_scale > 0 )
        {
            Scale = HLGame.hl_hud_scale;
        }
        else
        {
            base.UpdateScale( screenSize );
        }
    }
}
