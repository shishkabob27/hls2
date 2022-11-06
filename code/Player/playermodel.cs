[GameResource( "Playermodel", "pm", "" )]
public partial class Playermodel : GameResource
{

	[ResourceType( "vmdl" )]
	public string Model { get; set; }

	[ResourceType( "vanmgrph" )]
	public string Animgraph { get; set; }

	public string BodyGroupName { get; set; } = "";
	public int BodyGroupValue { get; set; } = 0;

}
