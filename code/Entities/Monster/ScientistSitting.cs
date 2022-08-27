﻿[Library("monster_sitting_scientist"), HammerEntity]
[EditorModel("models/hl1/monster/scientist/scientist_01.vmdl")]
[Title("Scientist"), Category("Monsters")]
internal class ScientistSitting : NPC
{
    // Stub NPC, this does nothing yet

    
    List<string> ScientistMDLList = new List<string>{
        "models/hl1/monster/scientist/scientist_01.vmdl",
        "models/hl1/monster/scientist/scientist_02.vmdl",
        "models/hl1/monster/scientist/scientist_03.vmdl",
        "models/hl1/monster/scientist/scientist_04.vmdl",
    };

    List<string> SittingAnims = new List<string>{
        "sitting2",
        "sitting3",
    };

    [Property]
	public float Body { get; set; } = 5;

    public override void Spawn()
    {
        NoNav = true;
        base.Spawn();
        //SetAnimGraph("animgraphs/scientist.vanmgrph");
        Health = 20;
        if (Body > 3)
        {
            Body = Rand.Int(0, 3);
        }
        SetModel(SetScientistModel());
        CurrentSequence.Name = Rand.FromList<string>(SittingAnims);
        //UseAnimGraph = false;
        SetupPhysicsFromAABB(PhysicsMotionType.Static, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;
        Position = Position - new Vector3(0, 0, 22);

        Tags.Add("npc", "playerclip");

    }

    // probably not the best way to do it but it works
    public string SetScientistModel(){
        switch (Body)
        {
            case 0:
                return "models/hl1/monster/scientist/scientist_01.vmdl";
            case 1:
                return "models/hl1/monster/scientist/scientist_02.vmdl";
            case 2:
                return "models/hl1/monster/scientist/scientist_03.vmdl";
            case 3:
                return "models/hl1/monster/scientist/scientist_04.vmdl";
            default:
                return Rand.FromList<string>(ScientistMDLList);
        }
    }
    DamageInfo LastDamage;
    
    float tick = 0;
    public override void Think()
    {
        tick += 0.01f;
        if (CurrentSequence.IsFinished == true || CurrentSequence.TimeNormalized == 1.0f || tick > CurrentSequence.Duration / 2)
        {
            tick = 0;
            CurrentSequence.Name = Rand.FromList<string>(SittingAnims);
        }
    }
    
}
