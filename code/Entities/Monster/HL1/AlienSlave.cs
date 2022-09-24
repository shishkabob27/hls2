[Library("monster_alien_slave"), HammerEntity]
[EditorModel("models/hl1/monster/vortigaunt.vmdl")]
[Title("Alien Slave"), Category("Monsters"), Icon("person")]
internal class AlienSlave : NPC
{
    // Stub NPC, this does nothing yet

    public override void Spawn()
    {
        base.Spawn();
        Health = 20;
        
        SetModel("models/hl1/monster/vortigaunt.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true; 

        Tags.Add("npc", "playerclip");
    
    }
    
}
