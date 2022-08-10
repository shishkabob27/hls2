
namespace Sandbox
{
	public class DeadCamera : CameraMode
	{
		Vector3 lastPos;

		public override void Activated()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			Position = pawn.Position + 10f;
			Rotation = pawn.EyeRotation;

			lastPos = Position;
		}

		public override void Update()
		{
			var pawn = Local.Pawn;
			if ( pawn == null ) return;

			var eyePos = pawn.EyePosition;
			if ( eyePos.Distance( lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
			{
				//Position = pawn.Position + 10f;
			}
			else
			{
				//Position = eyePos;
			}

			Rotation = pawn.EyeRotation;

			Viewer = pawn;
			lastPos = Position;
		}
	}
}
