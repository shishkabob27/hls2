using Sandbox;
using Sandbox.UI;

public class VRAmmo : WorldPanel
{
	public VRAmmo()
	{
		SetTemplate("/resource/templates/VR/ammo.html");
		SetClass("is-vr", true);
	}

	public override void Tick()
	{
		base.Tick();

		if (Local.Pawn is HLPlayer player && player.Health >=1)
		{
			Transform = player.RightHand.Transform;

			//
			// Offsets
			//
			Rotation *= new Angles(0, -90, 120).ToRotation();
			Position += Rotation.Forward * 4 + Rotation.Up * 6 - Rotation.Left * 0;
			WorldScale = 0.1f;
			Scale = 5f;

			PanelBounds = new Rect(0, 0, 1920, 1080);
		}
	}
}