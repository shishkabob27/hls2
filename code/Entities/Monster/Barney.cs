[Library("monster_barney"), HammerEntity]
[EditorModel("models/hl1/monster/barney.vmdl")]
[Title("Barney"), Category("Monsters")]
internal class Barney : NPC
{
    // Stub NPC, this does nothing yet
    public Barney()
    {

        NPCAnimGraph = "animgraphs/scientist.vanmgrph";
    }
    public override void Spawn()
    {
        base.Spawn();
        Health = 20;

        SetAnimGraph(NPCAnimGraph);
        SetModel("models/hl1/monster/barney.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-9, -9, 0), new Vector3(9, 9, 72));
        EnableHitboxes = true; 

        Tags.Add("npc", "playerclip");
    
    }
    
}
