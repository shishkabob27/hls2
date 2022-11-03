[Library("info_node")]
[HammerEntity]
[EditorModel( "models/editor/ground_node.vmdl" )]
[Title("info_node"), Category("Legacy"), Icon("volume_up")]
public class info_node : Entity
{
	// don't remove this yet, i have some ideas for this
	public override void Spawn()
	{
		base.Spawn();
		Delete();
	}
}
