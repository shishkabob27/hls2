
/// <summary>
/// Generic water volume. Make sure to have light probe volume envelop the volume of the water for the water to gain proper lighting.
/// </summary>
[Library( "func_water_simple" )]
[HammerEntity, Solid]
[HideProperty( "enable_shadows" )]
[HideProperty( "SetColor" )]
[Title( "Water HL1" ), Category( "Gameplay" ), Icon( "water" )]
public partial class func_water_simple : BrushEntity
{
	/// <summary>
	/// Handles water level and buoyancy.
	/// </summary>
	public WaterController WaterController = new WaterController();
	public func_water_simple()
	{
		Tags.Add( "water" );

		WaterController.WaterEntity = this;
		EnableTouch = true;
		EnableTouchPersists = true;
		RenderColor = RenderColor.WithAlpha( Math.Min( RenderColor.a, 0.95f ) );
	}

	public override void Spawn()
	{
		base.Spawn();

		WaterController.WaterEntity = this;
		Transmit = TransmitType.Always;
		Tags.Clear();
		Tags.Add( "water" );
		CreatePhysics();
		RenderColor = RenderColor.WithAlpha( Math.Min( RenderColor.a, 0.95f ) );
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();

		WaterController.WaterEntity = null;
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		WaterController.Touch( other );
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		WaterController.EndTouch( other );
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		WaterController.StartTouch( other );
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
