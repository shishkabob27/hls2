[Library( "path_corner" )]
[HammerEntity]
[Title( "path_corner" ), Category( "Legacy" ), Icon( "conversion_path" )]
public partial class path_corner : path_track
{
	[Property( "target" ), FGDType( "target_destination" )]
	new public string Target { get; set; } = "";

	// stub
	[Input]
	void Activate()
	{

	}
}
