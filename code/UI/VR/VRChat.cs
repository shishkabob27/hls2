using Sandbox;
using Sandbox.UI;

public class VRChat : WorldPanel
{
	public VRChat()
	{
		//SetTemplate("/resource/templates/VR/vitals.html");
		AddChild<ChatBox>();
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
			Rotation *= new Angles(-90, 0, 180).ToRotation();
			Position += Rotation.Forward * 4 + Rotation.Up * 4 - Rotation.Left * 5;
			WorldScale = 0.1f;
			Scale = 5f;

			PanelBounds = new Rect(0, 0, 1920, 1080);
		}
	}
}