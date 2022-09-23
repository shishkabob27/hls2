[Library( "monster_snark" ), HammerEntity]
[EditorModel( "models/hl1/monster/snark.vmdl" )]
[Title( "Snark" ), Category( "Monsters" ), Icon( "person" )]
public class Snark : NPC
{

    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/squeak_npc.vmdl" );
    public override void Spawn()
    {
        SetupPhysicsFromOBB( PhysicsMotionType.Static, new Vector3( 0.1f, 0.1f, 0.1f ), new Vector3( -0.1f, -0.1f, -0.1f ) );
        base.Spawn();
        Model = WorldModel;
    }
}