[Library("monster_gman"), HammerEntity]
[EditorModel("models/hl1/monster/gman.vmdl")]
[Title("Gman"), Category("Monsters")]
internal class Gman : NPC
{
    // Stub NPC, this does nothing yet
    
    public override void Spawn()
    {
        base.Spawn();
        //SetAnimGraph("animgraphs/scientist.vanmgrph");
        Health = 20;

        SetModel("models/hl1/monster/gman.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;
        PhysicsEnabled = true;
        UsePhysicsCollision = true;

        Tags.Add("player"); // add player for now until a monster tag is added (can't do that now cause editing addon cfg is a pain for me (xenthio btw)
    }
    
}
