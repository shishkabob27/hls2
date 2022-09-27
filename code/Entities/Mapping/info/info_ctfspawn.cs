[Library( "info_ctfspawn" ), HammerEntity]
[EditorModel( "models/editor/playerstart.vmdl", FixedBounds = true )]
[Title( "info_ctfspawn" ), Category( "Capture The Flag" ), Icon( "place" )]
public class info_ctfspawn : Entity
{
    [Property]
    public int team_no { get; set; } = 0;
}
