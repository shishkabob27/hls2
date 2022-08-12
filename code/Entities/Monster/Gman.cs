[Library("monster_Gman"), HammerEntity]
[EditorModel("models/hl1/monster/gman.vmdl")]
[Title("Gman"), Category("Monsters")]
internal class Gman : AnimatedEntity
{
    // Stub NPC, this does nothing yet

    
    [Property]
	public float Body { get; set; } = -1;

    public override void Spawn()
    {
        base.Spawn();
        SetAnimGraph("animgraphs/scientist.vanmgrph");
        Health = 20;

        SetModel("models/hl1/monster/gman.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;
        PhysicsEnabled = true;
        UsePhysicsCollision = true;

        Tags.Add("player"); // add player for now until a monster tag is added (can't do that now cause editing addon cfg is a pain for me (xenthio btw)
    }
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
                HLCombat.CreateGibs(this.CollisionWorldSpaceCenter, LastDamage.Position, Health, this.CollisionBounds);

            }
        }
        else
        {
            //BecomeRagdollOnClient(Velocity, LastDamage.Flags, LastDamage.Position, LastDamage.Force, GetHitboxBone(LastDamage.HitboxIndex));
        }
    } 
    
}
