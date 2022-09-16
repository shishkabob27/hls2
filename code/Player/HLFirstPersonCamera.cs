
namespace Sandbox
{
	public class HLFirstPersonCamera : CameraMode
	{
		Vector3 lastPos;

		public override void Activated()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			Position = pawn.EyePosition;
			Rotation = pawn.EyeRotation;

			lastPos = Position;
		}

		public override void Update()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;
			Viewer = pawn;
			if (pawn.Client.IsUsingVr) return;

			var eyePos = pawn.EyePosition;

			Position = eyePos;

			Rotation = pawn.EyeRotation;

			lastPos = Position;
		}
	}
}
