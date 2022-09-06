using Sandbox;
using Sandbox.UI;

public class GUIRootPanel : RootPanel
{
    public bool MenuOpen;
    public static GUIRootPanel Current;

	public GUIRootPanel()
	{
        Current = this;
		StyleSheet.Load("resource/styles/ingamemenu.scss");
		Focus();
        AddChild<Options>();
    }

	public override void Tick()
	{
        base.Tick();
	}

	protected override void UpdateScale( Rect screenSize )
	{
		Scale = 1;
    }
    [Event.BuildInput]
    public void ProcessClientInput(InputBuilder input)
    {
        if (input.Pressed(InputButton.Menu))
        {
            MenuOpen = !MenuOpen;
        }
    }
}
