using Sandbox.UI;

public class GUIRootPanel : RootPanel
{
	public static GUIRootPanel Current;

	public GUIRootPanel()
	{
        Current = this;
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
}
