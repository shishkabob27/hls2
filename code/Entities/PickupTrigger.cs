
/// <summary>
/// A utility class. Add as a child to your pickupable entities to expand
/// the trigger boundaries. They'll be able to pick up the parent entity
/// using these bounds.
/// </summary>
[Title( "Pickup Trigger" ), Icon( "select_all" )]
public class TouchTrigger : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "trigger" );

		// Set the default size
		SetTriggerSize( 16 );

		// Client doesn't need to know about htis
		Transmit = TransmitType.Never;
	}

	/// <summary>
	/// Set the trigger radius. Default is 16.
	/// </summary>
	public void SetTriggerSize( float radius )
	{
		SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, new Capsule( Vector3.Zero, Vector3.One * 0.1f, radius ) );
	}
}