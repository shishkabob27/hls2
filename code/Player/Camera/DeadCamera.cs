public class DeadCamera : CameraMode
{
	Vector3 FocusPoint;

	public override void Activated()
	{
		base.Activated();

		FocusPoint = CurrentView.Position;
	}

	public override void Update()
	{
		var player = Local.Client;
		if ( player == null ) return;
		if ( player is not HLPlayer ply ) return;

		// lerp the focus point
		FocusPoint = Vector3.Lerp( FocusPoint, GetSpectatePoint(), 1.0f );

		Position = FocusPoint + new Vector3( 0f, 0f, 24f );
		Rotation = ( ply.ViewAngles + new Angles( 0, 0, 80 ) ).ToRotation();

		Viewer = Local.Pawn;
	}

	public virtual Vector3 GetSpectatePoint()
	{
		if ( Local.Pawn is Player player && player.Corpse.IsValid() )
		{
			return player.Corpse.GetBoneTransform( player.Corpse.GetBoneIndex( "bip_01_pelvis" ) ).Position;
		}

		return Local.Pawn.Position;
	}
}
