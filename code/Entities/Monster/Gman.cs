[Library("monster_gman"), HammerEntity]
[EditorModel("models/hl1/monster/gman.vmdl")]
[Title("Gman"), Category("Monsters")]
public class Gman : NPC
{
    // Stub NPC, this does nothing yet
    
    public Gman()
    {
        NPCAnimGraph = "animgraphs/gman.vanmgrph";
        SetAnimGraph(NPCAnimGraph);
    }
    public override void Spawn()
    {
        base.Spawn();
        //SetAnimGraph("animgraphs/scientist.vanmgrph");
        Health = 20;

        SetAnimGraph(NPCAnimGraph);
        SetModel("models/hl1/monster/gman.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;

        Tags.Add("npc", "playerclip");
    }
    
}
