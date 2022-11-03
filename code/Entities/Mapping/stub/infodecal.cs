[Library("infodecal")]
[HammerEntity]
[EditorModel( "models/editor/axis_helper_thick.vmdl" )] 
[Title("infodecal"), Category("Legacy")]
public partial class infodecal : Entity
{
	// stub
	
	[Property, ResourceType("vmat") ]
	public string texture { get; set; }
	public override void Spawn()
	{
		base.Spawn();
		// We don't do anything yet so just remove oursleves when we span to avoid clogging up the map with extra unneeded entities
		Delete();
	}
}
