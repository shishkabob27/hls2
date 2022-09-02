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
        /*
        if (TargetEntity is ICombat)
        {
            var trgt = (TargetEntity as ICombat);
            var rel = HLCombat.ClassMatrix[Classify(), trgt.Classify()];

            if (rel > 0 && TargetEntity.Position.Distance(Position) > 60)
            {
                Steer.Target = TargetEntity.Position - ((TargetEntity.Position - Position).Normal * 50); // don't get too close!
            }
            else if (rel > 0 && TargetEntity.Position.Distance(Position) < 60)
            {
                animHelper.Attack = true;
            }
        }
        */
    }
    public override void ProcessEntity(Entity ent, int rel)
    {
        if (ent.LifeState != LifeState.Alive) return;
        if (rel > 0 && ent.Position.Distance(Position) > 60)
        {
            Steer.Target = ent.Position - ((ent.Position - Position).Normal * 50); // don't get too close!
        }
        else if (rel > 0 && ent.Position.Distance(Position) < 60)
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
    public override int Classify()
    {
        return (int)HLCombat.Class.CLASS_ALIEN_MONSTER;
    }
    public override void OnAnimEventGeneric(string name, int intData, float floatData, Vector3 vectorData, string stringData)
    {
        if (stringData == "claw" && IsServer)
        {
            foreach (var tr in TraceBullet(EyePosition, EyePosition + EyeRotation.Forward * 70, 1))
            {
                var damageInfo = DamageInfo.FromBullet(tr.EndPosition, EyeRotation.Forward * 50, 5)
                    .UsingTraceResult(tr)
                    .WithAttacker(Owner)
                    .WithWeapon(this);
                Log.Info(damageInfo);
                tr.Entity.TakeDamage(damageInfo);
            }
        }
        //base.OnAnimEventGeneric(name, intData, floatData, vectorData, stringData);
    }
}
