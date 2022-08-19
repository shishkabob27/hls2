﻿[Library("scripted_sequence")]
[HammerEntity]
[Title("scripted_sequence"), Category("Choreo"), Icon("volume_up")]
public partial class scripted_sequence : Entity
{
    [Flags]
    public enum Flags
    {
        Repeatable = 1,
        LeaveCorpse = 2,
        StartonSpawn = 4,
        NoInterruptions = 8,
        OverrideAI = 16,
        DontTeleportNPCatEnd = 128,
        LoopinPostIdle = 256,
        PriorityScript = 512,
        SearchCyclically = 1024,
        NoComplaints = 2048,
        AllowNPCDeath = 4096,
        //StartUnbreakable = 524288,
    }


    /// <summary>
    /// The target entity. legacy goldsrc stuff.
    /// </summary>
    [Property("target"), FGDType("target_destination")]
    public string TargetLegacy { get; set; } = "";
    public Entity TargetLegacyEnt;

    [Property("delay")]
    public float DelayLegacy { get; set; } = 0;

    [Property("killtarget"), FGDType("target_destination")]
    public string KillTargetLegacy { get; set; } = "";
    public Entity KillTargetLegacyEnt;

    [Property("m_iszEntity"), FGDType("target_destination")]
    public string TargetEntity { get; set; }

    [Property("m_iszPlay")]
    public string ActionAnimation { get; set; } = "null";

    [Property("m_bLoopActionSequence")]
    public int LoopActionAnimation { get; set; } = 0;

    [Property("m_fMoveTo")]
    public int MoveToMode { get; set; }

    public NPC TargetNPC;
    public scripted_sequence()
    {
    }

    
    protected Output OnEndSequence { get; set; }
    protected Output OnBeginSequence { get; set; }

    private async Task TriggerTask()
    {

        Log.Info("begin delay");
        await GameTask.DelaySeconds(DelayLegacy);
        Log.Info("end delay");
        await OnEndSequence.Fire(this);
    }

    [Input]
    public void BeginSequence()
    {

        ticker = false;
        ticker2 = false;
        timesince = -1;
        timetick = 0;
        timeduration = 0;
        // if (TargetLegacy != "" && DelayLegacy == 0)
        // GameTask.RunInThreadAsync(TriggerTask);

        if (FindByName(TargetEntity) is NPC)
        {
            TargetNPC = FindByName(TargetEntity) as NPC;
            Log.Info(TargetNPC.CurrentSequence.Name);
            switch (MoveToMode)
            {
                case 1:
                    TargetNPC.Steer.Target = this.Position;
                    if (TargetNPC.NPCAnimGraph != "")
                    {
                        if (!(TargetNPC.Position.AlmostEqual(this.Position, 16) || TargetNPC.Position == this.Position))
                        {
                            TargetNPC.SetAnimGraph(TargetNPC.NPCAnimGraph);
                            TargetNPC.UseAnimGraph = true;
                        } 
                        else if (ActionAnimation != "null")
                        {
                            TargetNPC.SetAnimGraph("");
                            TargetNPC.UseAnimGraph = false;
                        }
                    }
                    break;
                case 4:
                    TargetNPC.Position = this.Position;
                    if (ActionAnimation != "null")
                    {
                        TargetNPC.SetAnimGraph("");
                        TargetNPC.UseAnimGraph = false;
                    }
                    break;
                default:
                    TargetNPC.Steer.Target = this.Position;
                    if (TargetNPC.NPCAnimGraph != "")
                    { 
                        if (!(TargetNPC.Position.AlmostEqual(this.Position, 16) || TargetNPC.Position == this.Position))
                        {
                            TargetNPC.SetAnimGraph(TargetNPC.NPCAnimGraph);
                            TargetNPC.UseAnimGraph = true;
                            TargetNPC.CurrentSequence.Name = ActionAnimation;
                        }
                        else if (ActionAnimation != "null")
                        {
                            TargetNPC.SetAnimGraph("");
                            TargetNPC.UseAnimGraph = false;
                            TargetNPC.CurrentSequence.Name = ActionAnimation;
                            timeduration = TargetNPC.CurrentSequence.Duration;
                            timetick = 0;
                        }
                    }
                    break;
            }

            Log.Info("script sequence target set to " + TargetEntity);
            //TargetEntity.Position = this.Position; //TODO make this do something lol
        }
    }
    
    [Input]
    void MoveToPosition()
    {
        if (FindByName(TargetEntity) is NPC && FindByName(TargetEntity).IsValid())
        {
            TargetNPC = FindByName(TargetEntity) as NPC;
            TargetNPC.Steer.Target = this.Position;
            Log.Info("script sequence target set to " + TargetEntity);
            //TargetEntity.Position = this.Position; //TODO make this do something lol
        }
    }
    public override void Spawn()
    {

        if (TargetLegacy != "")
        {
            TargetLegacyEnt = FindByName(TargetLegacy);
        }
        if (KillTargetLegacy != "")
        {
            KillTargetLegacyEnt = FindByName(KillTargetLegacy);
        }

    }

    float timetick = 0;
    float timeduration = 0;
    bool ticker = false;
    bool ticker2 = false;
    float timesince = -1;
    [Event.Tick.Server]
    public void Tick()
    {
        // this code fucking freezes my whole pc. what?
        if (TargetNPC != null && (TargetNPC.Position.AlmostEqual(this.Position, 16) || TargetNPC.Position == this.Position)) //&& TargetNPC.CurrentSequence.IsFinished == true 
        {
            timetick += 0.02f;
            if (ticker == false)
            {
                ticker = true;
                Log.Info("script sequence target reached");
                OnBeginSequence.Fire(this);

                // this is ass, when is direct playback in animgraph coming in?
                TargetNPC.SetAnimGraph(""); 
                TargetNPC.UseAnimGraph = false; // use animgraph = false does nothing... why?
                TargetNPC.CurrentSequence.Name = ActionAnimation;
                timeduration = TargetNPC.CurrentSequence.Duration;
            }

            if (ticker == false && ticker2 == false)
            {
                ticker2 = true;
                timetick = 0;
                TargetNPC.CurrentSequence.Name = ActionAnimation;
            }
            if (timetick > timeduration)
            {
                timetick = 0;
                if (LoopActionAnimation == 1 && ticker && ticker2)
                {
                    ticker = false;
                    ticker2 = false;
                }
                else
                {
                    timesince = Time.Now + DelayLegacy;
                    if (TargetNPC.NPCAnimGraph != "")
                    {
                        TargetNPC.SetAnimGraph(TargetNPC.NPCAnimGraph);
                        TargetNPC.UseAnimGraph = true;
                    }
                }
                OnEndSequence.Fire(this);
                //if (TargetLegacy != "")
                //GameTask.RunInThreadAsync(TriggerTask);



            }
        }
        /**
        
        if (TargetNPC != null && (TargetNPC.Position.AlmostEqual(this.Position, 16) || TargetNPC.Position == this.Position)) //&& TargetNPC.CurrentSequence.IsFinished == true 
        {
            timetick += 0.02f;

            //Log.Info("g");
            ;
            if (ActionAnimation != "null" && ActionAnimation != "" && ActionAnimation != null && ticker == false)
            {
                //Log.Info("Move to ended");
                ticker = true;

                TargetNPC.SetAnimGraph("");
                TargetNPC.UseAnimGraph = false;
                OnBeginSequence.Fire(this);
                //Log.Info(TargetNPC + " is now playing " + ActionAnimation);
                timetick = 0;
                TargetNPC.CurrentSequence.Name = ActionAnimation;
                timeduration = TargetNPC.CurrentSequence.Duration;
            }
            //Log.Info("ticked 2 " + timetick + " / " + TargetNPC.CurrentSequence.Duration);
            if ((ActionAnimation != "null" && ActionAnimation != "" && ActionAnimation != null && ticker == true && ticker2 == false))//TargetNPC.CurrentSequence.IsFinished == true))
            {
                ticker2 = true;
                TargetNPC.SetAnimGraph("");
                TargetNPC.UseAnimGraph = false;
                timetick = 0;
                Log.Info(TargetNPC + " is now playing " + ActionAnimation);
                TargetNPC.CurrentSequence.Name = ActionAnimation;
                timeduration = TargetNPC.CurrentSequence.Duration;

            }
            //Log.Info(TargetNPC + " time elapsed is " + TargetNPC.CurrentSequence.Duration);
            if (timetick > TargetNPC.CurrentSequence.Duration)
            {
                timetick = 0;
                if (LoopActionAnimation == 1 && ticker && ticker2)
                {
                    ticker = false;
                    ticker2 = false;
                }
                else 
                {
                    timesince = Time.Now + DelayLegacy;
                    if (TargetNPC.NPCAnimGraph != "")
                    {
                        TargetNPC.SetAnimGraph(TargetNPC.NPCAnimGraph);
                        TargetNPC.UseAnimGraph = true;
                    }
                }
                //if (TargetLegacy != "")
                //GameTask.RunInThreadAsync(TriggerTask);
                


            }
        }
        
        if (Time.Now >= timesince && timesince != -1)
        {
            timesince = -1;
            OnEndSequence.Fire(this);
            if (TargetLegacyEnt is scripted_sequence && TargetLegacy != "")
            {
                //(TargetLegacyEnt as scripted_sequence).BeginSequence();
            }
        }
        **/
    }

}
