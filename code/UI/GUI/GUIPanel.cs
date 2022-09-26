public class GUIPanel : HudEntity<GUIRootPanel>
{
    public static GUIPanel Current;

    public Scoreboard Scoreboard { get; set; }

    public Subtitle Subtitle { get; set; }


    public GUIPanel()
    {
        Current = this;

        if ( IsClient )
        {
            RootPanel.AddChild<Options>();
        }
    }
}