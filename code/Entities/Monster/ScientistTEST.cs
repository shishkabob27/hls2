[Library("monster_scientistTEST"), HammerEntity]
[EditorModel("models/hl1/monster/scientist/scientist_01.vmdl")]
[Title("ScientistTEST"), Category("Monsters")]
internal class ScientistTEST : NPC
{
    // Stub NPC, this does nothing yet

    
    List<string> ScientistMDLList = new List<string>{
        "models/hl1/monster/scientist/scientist_01.vmdl",
        "models/hl1/monster/scientist/scientist_02.vmdl",
        "models/hl1/monster/scientist/scientist_03.vmdl",
        "models/hl1/monster/scientist/scientist_04.vmdl",
    };

    [Property]
	public float Body { get; set; } = -1;

    public override void Spawn()
    {
        base.Spawn();
        SetAnimGraph("animgraphs/scientist.vanmgrph");
        Health = 20;
        Speed = 80;
        SetModel(SetScientistModel());
        //SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true; 
        //PhysicsEnabled = true;
        UsePhysicsCollision = true;

        Tags.Add("player"); // add player for now until a monster tag is added (can't do that now cause editing addon cfg is a pain for me (xenthio btw)
    
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
    string MODE = "MODE_IDLE";
    bool wasInBound = false;
    public override void Think()
    {
        var ply = HLUtils.FindPlayerInBox(Position, 8096);
        if (MODE == "MODE_FOLLOW") 
        {
            if (ply != null && ply.IsValid && HLUtils.IsPlayerInBox(Position, 80) == false)
            {
                Steer.Target = ply?.Position ?? Vector3.Zero;
                wasInBound = true;
            }
            else if (ply != null && ply.IsValid && wasInBound == true)
            {
                Steer.Target = Position;
                wasInBound = false;
            }

            if (ply != null && ply.IsValid && HLUtils.IsPlayerInBox(Position, 190))
            {
                Speed = 80;
            }
            else if (ply != null && ply.IsValid && HLUtils.IsPlayerInBox(Position, 200) == false)
            {
                Speed = 200;
            }
            else
            {
                Speed = 80;
            }
        }   
       
       
        
        // we've been pushed!
        if (ply != null && ply.IsValid && HLUtils.IsPlayerInBox(Position, 10))
        {
            Steer.Target = Position + (ply.Position - Position).Normal * -78;
            Speed = 80;
        }

    }
    int ticker = 1;

    static SoundEvent soundevent = new SoundEvent("sounds/hl1/scientist/alright.vsnd");
    
    public override bool OnUse(Entity user)
    {
        if (ticker == 1)
        {
            ticker = 0;

            //Steer.Target = Position + (user.Position - Position).Normal * 10; // Turn to face the user.
            targetRotation = Rotation.LookAt(user.Position.WithZ(0) - Position.WithZ(0), Vector3.Up);
            //targetRotation.w = Rotation.w;
            //this.LookDir = 
            //lookAt = user.Position.WithZ(Position.z);

            CurrentSound.Stop();

            
            //if ( ResourceLibrary.TryGet<SoundEvent>("sounds/hl1/scientist/sci_follow.sound", out var soundevent) ) //PlaySound("sci_follow");
            //{
            CurrentSound = PlaySound("sounds/hl1/scientist/sci_follow.sound");

            //}


            if (MODE == "MODE_IDLE")
            {
                
                MODE = "MODE_FOLLOW";
            } else if (MODE == "MODE_FOLLOW")
            {
                MODE = "MODE_IDLE";
                
            }
            return true;
        }
        else
        {
            ticker = 1;
            return false;
        }


        return base.OnUse(user);
    }


}
