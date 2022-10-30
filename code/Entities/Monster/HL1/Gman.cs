[Library("monster_gman"), HammerEntity]
[EditorModel("models/hl1/monster/gman.vmdl")]
[Title("Gman"), Category("Monsters"), Icon("person"), MenuCategory( "Black Mesa" )]
public class Gman : NPC
{
    // Stub NPC, this does nothing yet
    
    public Gman()
    {
        NPCAnimGraph = "animgraphs/hl1/monster/gman.vanmgrph";
    }
    public override void Spawn()
    {
        base.Spawn();

        Health = 20;
        SetModel("models/hl1/monster/gman.vmdl");
        SetAnimGraph(NPCAnimGraph);
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;

        Tags.Add("npc", "playerclip");
    }
    
}
