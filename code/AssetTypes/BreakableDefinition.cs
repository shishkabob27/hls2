[GameResource("Breakable Definition", "break", "Defines the .")]
public partial class BreakableDefinition : GameResource
{
	public string Title { get; set; }

	[ResourceType("vmdl"), Title("Gib Models")]
	public string Model { get; set; }


	[ResourceType("sound"), Title("Break Sound")]
	public string BreakSound { get; set; }

	[ResourceType("sound"), Title("Gib Bounce Sound")]
	public string BounceSound { get; set; }

	[HideInEditor]
	public int Amount { get; set; }
    
	// ...
}