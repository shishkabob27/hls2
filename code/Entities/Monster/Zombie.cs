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
    public TimeSince TimeSinceLastSound = new TimeSince();
    public override void Think()
    {

        int r = Rand.Int(50);
        if (LifeState != LifeState.Alive) return;
        if (r == 10 && TimeSinceLastSound > 5)
        {
            TimeSinceLastSound = 0;
            SpeakSound("sounds/hl1/zombie/zo_idle.sound");
            
        }
        base.Think();
        // todo, trace a cone maybe...
        var a = Trace.Ray(EyePosition, EyePosition + Rotation.Forward * 2000)
            .Radius(50f)
            .EntitiesOnly()
            .Ignore(this)
            .Run();

        if (a.Entity != TargetEntity && a.Entity is HLPlayer or null)
        {
            TargetEntity = a.Entity;
            if (TimeSinceLastSound > 2 && a.Entity is HLPlayer)
            {
                TimeSinceLastSound = 0;
                SpeakSound("sounds/hl1/zombie/zo_alert.sound");
            }
        }
        if (a.Entity == null) return;
        var b = Trace.Ray(EyePosition, a.Entity.Position)
            .Ignore(this)
            .Run();
        if (b.Entity != a.Entity) return;
        if (a.Entity is HLPlayer && a.Entity.Position.Distance(Position) > 60)
        {
            Steer.Target = a.Entity.Position - ((a.Entity.Position - Position).Normal * 50); // don't get too close!
        } 
        else if (a.Entity is HLPlayer && a.Entity.Position.Distance(Position) < 60)
        {
            animHelper.Attack = true;
        }
    }
    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);

        if (LifeState == LifeState.Alive)
        {
            SpeakSound("sounds/hl1/zombie/zo_pain.sound");
        }

    }
}
