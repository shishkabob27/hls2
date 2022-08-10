/// <summary>
/// Declares a sphere in which the specified soundscape will be active. If the ear is within two radiuses, we'll use the
/// closest. The entity needs to be within the PVS for it to be active.
/// </summary>
[HammerEntity]
[ClassName( "env_sound" ), Title( "Soundscape Radius" ), Category( "Legacy" ), Icon( "spatial_tracking" )]
[EditorSprite( "materials/editor/env_soundscape.vmat" )]
[Sphere( "radius" )]
public partial class SoundscapeRadiusEntity : Entity
{
	[Net, Property]
	public bool Enabled { get; set; } = true;

	[Net, Property]
	[ResourceType( "sndscape" )]
	public string Soundscape { get; set; }

	[Net, Property]
	public float Radius { get; set; } = 1000.0f;


	/// <summary>
	/// Become enabled
	/// </summary>
	[Input]
	protected void Enable()
	{
		Enabled = true;
	}

	/// <summary>
	/// Become disabled
	/// </summary>
	[Input]
	protected void Disable()
	{
		Enabled = false;
	}

	/// <summary>
	/// Toggle between enabled and disabled
	/// </summary>
	[Input]
	protected void Toggle()
	{
		Enabled = !Enabled;
	}

	/// <summary>
	/// Returns true if the soundscape is enabled and audible from this location
	/// </summary>
	public bool TestPosition( Vector3 position )
	{
		Host.AssertClient();

		if ( !Enabled )
			return false;

		var delta = position - Position;
		return delta.Length < Radius;
	}
}
