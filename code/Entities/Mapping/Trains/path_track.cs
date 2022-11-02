[Library( "path_track" )]
[HammerEntity]
[Title( "path_track" ), Category( "Paths" ), Icon( "conversion_path" )]
public partial class path_track : Entity
{

	public Output OnPass { get; set; }

	[Property( "target" ), FGDType( "target_destination" )]
	public string Target { get; set; } = "";


	[Property( "speed" )]
	public float Speed { get; set; } = 200;

	// stub
	[Input]
	void Activate()
	{

	}


}
