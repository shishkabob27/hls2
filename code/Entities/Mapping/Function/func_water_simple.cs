
/// <summary>
/// Generic water volume. Make sure to have light probe volume envelop the volume of the water for the water to gain proper lighting.
/// </summary>
[Library( "func_water_simple" )]
[HammerEntity, Solid]
[HideProperty( "enable_shadows" )]
[HideProperty( "SetColor" )]
[Title( "Water HL1" ), Category( "Gameplay" ), Icon( "water" )]
public partial class func_water_simple : Water
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		CreatePhysics();
		EnableDrawing = true;
	}

	public override void ClientSpawn()
	{
		Host.AssertClient();
		base.ClientSpawn();

		CreatePhysics();
	}

	void CreatePhysics()
	{
		var physicsGroup = SetupPhysicsFromModel( PhysicsMotionType.Keyframed, true );
		physicsGroup.SetSurface( "water" );
	}
}
