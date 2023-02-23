
partial class HL1GameMovement
{
	public Transform? GroundTransform { get; set; }
	void RestoreGroundPos()
	{
		if ( GroundEntity == null || GroundEntity.IsWorld || GroundTransform == null )
			return;

		var worldTrns = GroundEntity.Transform.ToWorld( GroundTransform.Value );
		Position = worldTrns.Position;
		Rotation = worldTrns.Rotation;
		//if ( Entity.GroundEntity == null || Entity.GroundEntity.IsWorld )
		//return;

		//var Position = GroundEntity.Transform.ToWorld( GroundTransform );
		//Pos = Position.Position;
	}

	void SaveGroundPos()
	{

		if ( GroundEntity == null || GroundEntity.IsWorld )
		{
			GroundTransform = null;
			return;
		}

		GroundTransform = GroundEntity.Transform.ToLocal( new Transform( Position + Vector3.Up * 1f, Rotation ) );

		//if ( Entity.GroundEntity == null || Entity.GroundEntity.IsWorld )
		//return;

		//GroundTransform = GroundEntity.Transform.ToLocal( new Transform( Pos, Rot ) );
	}
}
