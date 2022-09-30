[Library("monster_barney"), HammerEntity]
[EditorModel("models/hl1/monster/barney.vmdl")]
[Title("Barney"), Category("Monsters"), Icon("person")]
internal class Barney : NPC
{
    // Stub NPC, this does nothing yet
    public Barney()
    {

        NPCAnimGraph = "animgraphs/hl1/monster/scientist.vanmgrph";
    }
    public override int Classify()
    {
        return (int)HLCombat.Class.CLASS_PLAYER_ALLY;
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
    public override void ProcessEntity(Entity ent, int rel)
    {
        if (rel > 0)
        {
            targetRotation = Rotation.From(((Position - ent.Position) * -360).EulerAngles);
        }
    }

}
