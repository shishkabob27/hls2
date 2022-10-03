
partial class Source1GameMovement
{
	public virtual Vector3 GetPlayerMins( bool ducked ) { return Player.GetPlayerMinsScaled( ducked ); }
	public virtual Vector3 GetPlayerMaxs( bool ducked ) { return Player.GetPlayerMaxsScaled( ducked ); }
	public virtual Vector3 GetPlayerViewOffset( bool ducked ) { return Player.GetPlayerViewOffsetScaled( ducked ); }
	public virtual Vector3 GetPlayerExtents( bool ducked ) { return Player.GetPlayerExtentsScaled( ducked ); }

	public virtual Vector3 GetPlayerMins() { return GetPlayerMins( IsDucked ); }
	public virtual Vector3 GetPlayerMaxs() { return GetPlayerMaxs( IsDucked ); }
	public virtual Vector3 GetPlayerViewOffset() { return GetPlayerViewOffset( IsDucked ); }
	public virtual Vector3 GetPlayerExtents() { return GetPlayerExtents( IsDucked ); }
}
