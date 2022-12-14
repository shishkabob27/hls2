using Sandbox;
using Sandbox.UI;

public class VRVitals : WorldPanel
{
	public VRVitals()
	{
		SetTemplate("/resource/templates/VR/vitals.html");
		SetClass("is-vr", true);
	}

	public override void Tick()
	{
		base.Tick();

		if (Local.Pawn is HLPlayer player && player.Health >=1)
		{
			Transform = player.LeftHand.Transform;

			//
			// Offsets
			//
			Rotation *= new Angles(-180, -90, 45).ToRotation();
			Position += Rotation.Forward * 4 + Rotation.Up * 4 - Rotation.Left * 6;
			WorldScale = 0.1f;
			Scale = 5f;

			PanelBounds = new Rect(0, 0, 1920, 1080);
		}
	}
}