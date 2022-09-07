using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

[UseTemplate("/resource/templates/test.html")]
public class Advanced : GUIPanel
{


	public Advanced()
	{
		StyleSheet.Load("/resource/styles/options.scss");
		Style.Left = 0;
		Style.Right = 0;
		Style.Top = 0;
		Style.Bottom = 0;
		Focus();
	}

	public override void Tick()
	{
		base.Tick();
		Drag();
        SetClass("active", MenuOpen);
    }

	[Event.BuildInput]
	public void ProcessClientInput(InputBuilder input)
	{
		if (input.Pressed(InputButton.Menu))
        {
            Style.Left = (Screen.Width / 2) - (Box.Rect.Width / 2);
            Style.Top = (Screen.Height / 2) - (Box.Rect.Height / 2); 
			MenuOpen = !MenuOpen;
		}
	}
}
