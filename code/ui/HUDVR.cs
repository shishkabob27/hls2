using Sandbox;
using Sandbox.UI;

public class VRHUDVitals : WorldPanel
{
	public VRHUDVitals()
	{
		SetTemplate("/resource/templates/VR/vitals.html");
		SetClass("is-vr", true);
	}

	public override void Tick()
	{
		base.Tick();

		if (Local.Pawn is HLPlayer player)
		{
			Transform = player.LeftHand.Transform;

			//
			// Offsets
			//
			Rotation *= new Angles(-180, -90, 45).ToRotation();
			Position += Rotation.Forward * 5 + Rotation.Up * 6 - Rotation.Left * 12;
			WorldScale = 0.1f;
			Scale = 2.0f;

			PanelBounds = new Rect(0, 0, 1920, 1080);
		}
	}
}