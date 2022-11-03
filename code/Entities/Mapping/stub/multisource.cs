[Library( "multisource" )]
[HammerEntity]
[EditorSprite( "editor/multisource.vmat" )]
[Title( "multisource" ), Category("Legacy"), Icon("volume_up")]
public partial class multisource : Entity
{
	// stub
	public override void Spawn()
	{
		base.Spawn();
		// We don't do anything yet so just remove oursleves when we span to avoid clogging up the map with extra unneeded entities
		Delete();
	}
}
