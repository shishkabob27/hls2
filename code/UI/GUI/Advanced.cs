using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

[UseTemplate("/resource/templates/test.html")]
public class Advanced : GUIPanel
{


	public Advanced()
	{
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

	[Event.Client.BuildInput]
	public void ProcessClientInput()
	{
		if (Input.Pressed(InputButton.Menu))
        {
            Style.Left = (Screen.Width / 2) - (Box.Rect.Width / 2);
            Style.Top = (Screen.Height / 2) - (Box.Rect.Height / 2); 
			MenuOpen = !MenuOpen;
		}
	}
}
