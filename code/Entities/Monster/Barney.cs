[Library("monster_barney"), HammerEntity]
[EditorModel("models/hl1/monster/barney.vmdl")]
[Title("Barney"), Category("Monsters")]
internal class Barney : NPC
{
    // Stub NPC, this does nothing yet

    public override void Spawn()
    {
        base.Spawn();
        Health = 20;
        
        SetModel("models/hl1/monster/barney.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true; 

        Tags.Add("player"); // add player for now until a monster tag is added (can't do that now cause editing addon cfg is a pain for me (xenthio btw)
    
    }
    
}
