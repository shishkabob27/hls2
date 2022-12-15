public class ThirdPersonCamera : CameraMode
{
	public override void Update()
	{

		var pawn = Game.LocalPawn as HLPlayer;
		if ( pawn == null ) return;
		if ( pawn.Client.IsUsingVr ) return;

		Rotation = pawn.ViewAngles.ToRotation();
		Camera.FirstPersonViewer = null;

		Vector3 targetPos;
		var center = pawn.Position + Vector3.Up * 64;

		var pos = center;
		var rot = Rotation.FromAxis( Vector3.Up, -16 ) * Camera.Rotation;

		float distance = 130.0f * pawn.Scale;
		targetPos = pos + rot.Right * ((pawn.CollisionBounds.Mins.x + 32) * pawn.Scale);
		targetPos += rot.Forward * -distance;

		var tr = Trace.Ray( pos, targetPos )
			.WithAnyTags( "solid" )
			.Ignore( pawn )
			.Radius( 8 )
			.Run();

		Position = tr.EndPosition;
	}
}
