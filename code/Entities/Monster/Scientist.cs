[Library("monster_scientist"), HammerEntity]
[EditorModel("models/hl1/monster/scientist/scientist_01.vmdl")]
[Title("Scientist"), Category("Monsters")]
public partial class Scientist : NPC
{
    // Stub NPC, this does nothing yet

    Entity FollowTarget;
    
    List<string> ScientistMDLList = new List<string>{
        "models/hl1/monster/scientist/scientist_01.vmdl",
        "models/hl1/monster/scientist/scientist_02.vmdl",
        "models/hl1/monster/scientist/scientist_03.vmdl",
        "models/hl1/monster/scientist/scientist_04.vmdl",
    };

    [Property]
    public float Body { get; set; } = 5;
    public float VoicePitch = 100;
    
    
    public Scientist() {
        NPCAnimGraph = "animgraphs/scientist.vanmgrph";
        SetAnimGraph(NPCAnimGraph);
        Health = 20;
        Speed = 80;
        WalkSpeed = 80;
        RunSpeed = 200;
        VoicePitch = SetPitch();
    }

    public override void Spawn()
    {
        //NPCAnimGraph = "animgraphs/scientist.vanmgrph";
        base.Spawn();
        EnableTouch = true;
        if (Body > 3) {
            Body = Rand.Int(0, 3);
        }
        SetAnimGraph(NPCAnimGraph);
        Health = 20;
        Speed = 80;
        VoicePitch = SetPitch();
        SetModel(SetScientistModel());
        //SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        EnableHitboxes = true;

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

    public int SetPitch()
    {
        switch (Body)
        {
            case 0:
                return 105;  // glasses
            case 1:
                return 100;  // einstein
            case 2:
                return 95;   // luther
            case 3:
                return 100;  // slick
            default:
                return 100;
        }
    }
    
    string MODE = "MODE_IDLE";
    bool wasInBound = false;
    public override void Think()
    {
        if (InScriptedSequence)
            return;
        //VoicePitch = SetPitch();
        var ply = HLUtils.ClosestPlayerTo(Position);
        if (MODE == "MODE_FOLLOW") 
        {
            if (FollowTarget != null && FollowTarget.IsValid && FollowTarget.Position.Distance(Position) > 80)
            {
                Steer.Target = FollowTarget?.Position ?? Vector3.Zero;
                wasInBound = true;
            }
            else if (FollowTarget != null && FollowTarget.IsValid && wasInBound == true)
            {
                Steer.Target = Position;
                wasInBound = false;
            }

            if (FollowTarget != null && FollowTarget.IsValid && FollowTarget.Position.Distance(Position) < 230)
            {
                Speed = WalkSpeed;
            }
            else if (FollowTarget != null && FollowTarget.IsValid && FollowTarget.Position.Distance(Position) > 256)
            {
                Speed = RunSpeed;
            }
            else
            {
                Speed = WalkSpeed;
            }
        }

        // we've been pushed!
        if (ply != null && ply.IsValid && ply.Position.Distance(Position) < 32 && !InScriptedSequence)
        {
            Steer.Target = Position + (ply.Position - Position).Normal * -78;
            Speed = 80;
        }
        
    }
    public override void Touch(Entity other)
    {
        base.Touch(other);
        if (other is DoorEntity)
        {
            (other as DoorEntity).Open();
        }
    }
    
    int ticker = 1;

    static SoundEvent soundevent = new SoundEvent("sounds/hl1/scientist/alright.vsnd");
    public override void TakeDamage(DamageInfo info)
    {
        base.TakeDamage(info);

        if (LifeState == LifeState.Alive)
        {
            SpeakSound("sounds/hl1/scientist/sci_pain.sound", VoicePitch / 100);
        }
        animHelper.IsScared = true;
        //CurrentSound.Stop();
        //CurrentSound.Stop();
        //CurrentSound = PlaySound("sounds/hl1/scientist/sci_pain.sound").SetPitch(VoicePitch / 100);

    }
  
    public override bool OnUse(Entity user)
    {
        if (!(base.OnUse(user)))
        {
            return false;
        }
        if (ticker == 1)
        {
            ticker = 0;

            //Steer.Target = Position + (user.Position - Position).Normal * 10; // Turn to face the user.
            targetRotation = Rotation.LookAt(user.Position.WithZ(0) - Position.WithZ(0), Vector3.Up);
            //targetRotation.w = Rotation.w;
            //this.LookDir = 
            //lookAt = user.Position.WithZ(Position.z);

            

            
            //if ( ResourceLibrary.TryGet<SoundEvent>("sounds/hl1/scientist/sci_follow.sound", out var soundevent) ) //PlaySound("sci_follow");
            //{
            

            //}


            if (MODE == "MODE_IDLE")
            {
                //CurrentSound.Stop();
                //CurrentSound = PlaySound("sounds/hl1/scientist/sci_follow.sound").SetPitch(VoicePitch / 100);
                SpeakSound("sounds/hl1/scientist/sci_follow.sound", VoicePitch / 100);
                MODE = "MODE_FOLLOW";
                DontSleep = true;
                FollowTarget = HLUtils.ClosestPlayerTo(Position);
            } else if (MODE == "MODE_FOLLOW")
            {
                //CurrentSound.Stop();
                //CurrentSound = PlaySound("sounds/hl1/scientist/sci_stopfollow.sound").SetPitch(VoicePitch / 100);
                SpeakSound("sounds/hl1/scientist/sci_stopfollow.sound", VoicePitch / 100);
                MODE = "MODE_IDLE";
                DontSleep = false;
                FollowTarget = null;
            }
            return true;
        }
        else
        {
            ticker = 1;
            return false;
        }
    }


}
