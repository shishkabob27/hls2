[Library("monster_zombie"), HammerEntity]
[EditorModel("models/hl1/monster/zombie.vmdl")]
[Title("Zombie"), Category("Monsters")]
internal class Zombie : NPC
{
    // Stub NPC, this does nothing yet

    public override void Spawn()
    {
        base.Spawn();
        Health = 20;
        
        SetModel("models/hl1/monster/zombie.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;
        SetAnimGraph("animgraphs/zombie.vanmgrph");
        Tags.Add("npc", "playerclip");
    
    }
    public override void Think()
    {
        base.Think();
    }
}
