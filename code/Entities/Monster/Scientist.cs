[Library("monster_scientist"), HammerEntity]
[EditorModel("models/hl1/monster/scientist/scientist_01.vmdl")]
[Title("Scientist"), Category("Monsters")]
internal class Scientist : AnimatedEntity
{
    List<string> ScientistMDLList = new List<string>{
        "models/hl1/monster/scientist/scientist_01.vmdl",
        "models/hl1/monster/scientist/scientist_02.vmdl",
        "models/hl1/monster/scientist/scientist_03.vmdl",
        "models/hl1/monster/scientist/scientist_04.vmdl",
    };

    public override void Spawn()
    {
        base.Spawn();
        SetAnimGraph("animgraphs/scientist.vanmgrph");
        Health = 20;
        SetModel(Rand.FromList<string>(ScientistMDLList));
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true; 
        PhysicsEnabled = true;
        UsePhysicsCollision = true;

        Tags.Add("player"); // add player for now until a monster tag is added (can't do that now cause editing addon cfg is a pain for me (xenthio btw)
    
    }
    // Stub, this does nothing yet
    DamageInfo LastDamage;
    
    public override void TakeDamage(DamageInfo info)
    {
        LastDamage = info;        
        if (LifeState == LifeState.Dead)
            return;

        base.TakeDamage(info);

        this.ProceduralHitReaction(info);

        //
        // Add a score to the killer
        //
        if (LifeState == LifeState.Dead && info.Attacker != null)
        {
            if (info.Attacker.Client != null && info.Attacker != this)
            {
                info.Attacker.Client.AddInt("kills");
            }
        }
    }

    public override void OnKilled()
    {
        base.OnKilled();

        if (LastDamage.Flags.HasFlag(DamageFlags.Blast))
        {
            using (Prediction.Off())
            {
                HLCombat.CreateGibs(this.CollisionWorldSpaceCenter, LastDamage);

            }
        }
        else
        {
            //BecomeRagdollOnClient(Velocity, LastDamage.Flags, LastDamage.Position, LastDamage.Force, GetHitboxBone(LastDamage.HitboxIndex));
        }
    } 
    
}
