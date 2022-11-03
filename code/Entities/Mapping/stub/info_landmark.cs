[Library( "info_landmark" )]
[HammerEntity]
[EditorSprite( "editor/info_landmark.vmat" )]
[Title( "info_landmark" ), Category("Legacy"), Icon("volume_up")]
public partial class info_landmark : Entity
{
	// stub
	public override void Spawn()
	{
		base.Spawn();
		// We don't do anything yet so just remove oursleves when we span to avoid clogging up the map with extra unneeded entities
		Delete();
	}
}
