using Sandbox.UI;

[UseTemplate( "/resource/templates/ingamemenu.html" )]
public class GUIRootPanel : RootPanel
{
    public bool MenuOpen;
    public static GUIRootPanel Current;
    public float OverrideScale { get; set; } = 0;

    public GUIRootPanel()
    {
        Current = this;
        StyleSheet.Load( "resource/styles/ingamemenu.scss" );
        Style.ZIndex = 100;
        Focus();
        AddChild<Options>();
        Focus();
    }

    public override void Tick()
    {
        base.Tick();
    }

    protected override void UpdateScale( Rect screenSize )
    {
        Scale = 1;
        if ( !HLGame.hl_gui_rescale ) return;
        if ( OverrideScale > 0 )
        {
            Scale = OverrideScale;
            return;
        }
        if ( Screen.Width > 1920 ) Scale = 1.50f;
        if ( Screen.Height > 2650 ) Scale = 2.00f;
    }
    [Event.BuildInput]
    public void ProcessClientInput( InputBuilder input )
    {
        if ( input.Pressed( InputButton.Menu ) )
        {
            MenuOpen = !MenuOpen;
        }
    }
}
