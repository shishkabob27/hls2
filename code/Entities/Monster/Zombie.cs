[Library("monster_zombie"), HammerEntity]
[EditorModel("models/hl1/monster/zombie.vmdl")]
[Title("Zombie"), Category("Monsters")]
internal class Zombie : NPC
{
    // Stub NPC, this does nothing yet
    public Zombie()
    {
        NPCAnimGraph = "animgraphs/zombie.vanmgrph";
        SetAnimGraph(NPCAnimGraph);
        Health = 20;
        WalkSpeed = 30;
        Speed = 30;
    }
    public override void Spawn()
    { 

        Health = 20;
        base.Spawn();
        
        SetModel("models/hl1/monster/zombie.vmdl");
        //SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;
        Tags.Add("npc", "playerclip");
    
    }
    public override void Think()
    {
        if (LifeState != LifeState.Alive) return;
        base.Think();
        // todo, trace a cone maybe...
        var a = Trace.Ray(Position, Position + Rotation.Forward * 2000)
            .Radius(30f)
            .EntitiesOnly()
            .Ignore(this)
            .Run();
        if (a.Entity == null) return;
        var b = Trace.Ray(Position, a.Entity.Position)
            .Ignore(this)
            .Run();
        if (b.Entity != a.Entity) return;
        if (a.Entity is HLPlayer)
        {
            Steer.Target = a.Entity.Position;
        }
    }
}
