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

        Tags.Add("npc");
    
    }
    
}
