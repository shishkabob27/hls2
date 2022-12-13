using Sandbox;
using Sandbox.UI;

public class VRInventory : WorldPanel
{
	public VRInventory()
	{
		SetTemplate("/resource/templates/VR/inventory.html");
		SetClass("is-vr", true);
	}

	public override void Tick()
	{
		base.Tick();

		if (Game.LocalPawn is HLPlayer player && player.Health >=1)
		{
			Transform = player.RightHand.Transform;

			//
			// Offsets
			//
			Rotation *= new Angles(-90, 0, 180).ToRotation();
			Position += Rotation.Forward * 4 + Rotation.Up * 4 - Rotation.Left * 5;
			WorldScale = 0.1f;
			Scale = 5f;

			PanelBounds = new Rect(0, 0, 3000, 3000);
		}
	}
}
