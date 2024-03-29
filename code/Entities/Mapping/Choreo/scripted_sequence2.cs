﻿[Library( "scripted_sequence2" )]
[HammerEntity]
[EditorModel( "models/editor/scripted_sequence.vmdl" )]
[Title( "scripted_sequence2" ), Category( "Choreo" ), Icon( "theater_comedy" )]
public partial class scripted_sequence2 : Entity
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

	public enum moveToMode
	{
		No,
		Walk,
		Run,
		Custom_movement,
		Instantaneous,
		No_turn_to_face
	}



	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags SpawnSettings { get; set; } = Flags.Repeatable;

	/// <summary>
	/// The target entity. legacy goldsrc stuff.
	/// </summary>
	[Property( "target" ), FGDType( "target_destination" )]
	public string TargetLegacy { get; set; } = "";
	public Entity TargetLegacyEnt;

	[Property( "delay" )]
	public float DelayLegacy { get; set; } = 0;

	[Property( "killtarget" ), FGDType( "target_destination" )]
	public string KillTargetLegacy { get; set; } = "";
	public Entity KillTargetLegacyEnt;

	[Property( "m_iszEntity" ), FGDType( "target_destination" )]
	public string TargetEntity { get; set; }

	[Property( "m_iszPlay" )]
	public string ActionAnimation { get; set; } = "null";

	[Property( "m_iszPostIdle" )]
	public string PostActionAnimation { get; set; } = "null";

	[Property( "m_iszIdle" )]
	public string PreActionAnimation { get; set; } = "null";

	[Property( "m_bLoopActionSequence" )]
	public bool LoopActionAnimation { get; set; } = false;




	[Property( "m_fMoveTo" )]
	public moveToMode MoveToMode { get; set; }

	public NPC TargetNPC;
	public scripted_sequence2()
	{
		TargetNPC = FindByName( TargetEntity ) as NPC;
	}


	protected Output OnEndSequence { get; set; }
	public void EndSequence()
	{

	}
	protected Output OnBeginSequence { get; set; }
	[Input]
	public void BeginSequence()
	{
		if ( TargetNPC is not NPC )
		{
			TargetNPC = FindByName( TargetEntity ) as NPC;
		}
		//TargetNPC.targetRotation = this.Rotation;
		if ( SpawnSettings.HasFlag( Flags.PriorityScript ) )
		{
			TargetNPC.InPriorityScriptedSequence = true;
		}
		if ( SpawnSettings.HasFlag( Flags.OverrideAI ) )
		{
			TargetNPC.ScriptedSequenceOverrideAi = true;
		}
		else if ( TargetNPC != null && TargetNPC.InPriorityScriptedSequence )
		{
			hasStarted = false;
			return;
		}

		if ( Game.IsClient ) return;
		if ( TargetNPC != null ) TargetNPC.InScriptedSequence = true;
		hasStarted = true;
		ticker = false;
		ticker2 = false;
		timesince = -1;
		timetick = 0;
		timeout = 0;
		timeduration = 0;
		// if (TargetLegacy != "" && DelayLegacy == 0)
		// GameTask.RunInThreadAsync(TriggerTask);




		Move();
		readyToPlay = true;
	}
	private async Task TriggerTask()
	{

		Log.Info( "begin delay" );
		await GameTask.DelaySeconds( DelayLegacy );
		Log.Info( "end delay" );
		await OnEndSequence.Fire( this );
	}
	void Move()
	{
		if ( TargetNPC is not NPC )
		{
			TargetNPC = FindByName( TargetEntity ) as NPC;
		}
		TargetNPC.InScriptedSequence = true;
		TargetNPC.targetRotationOVERRIDE = null;
		//TargetNPC.targetRotation = this.Rotation;
		if ( TargetNPC is NPC )
		{

			switch ( MoveToMode )
			{
				case moveToMode.Walk: // Walk to target
					TargetNPC.Steer.Target = this.Position;
					if ( TargetNPC.NPCAnimGraph != "" )
					{
						if ( !(TargetNPC.Position.AlmostEqual( this.Position, 16 ) || TargetNPC.Position == this.Position) )
						{
							TargetNPC.SetAnimGraph( TargetNPC.NPCAnimGraph );
							TargetNPC.UseAnimGraph = true;
						}
						else if ( ActionAnimation != "null" )
						{
						}
					}
					break;
				case moveToMode.Run: // Run to Target
					TargetNPC.Steer.Target = this.Position;
					if ( TargetNPC.NPCAnimGraph != "" )
					{
						if ( !(TargetNPC.Position.AlmostEqual( this.Position, 16 ) || TargetNPC.Position == this.Position) )
						{
							TargetNPC.SetAnimGraph( TargetNPC.NPCAnimGraph );
							TargetNPC.UseAnimGraph = true;
							TargetNPC.Speed = TargetNPC.RunSpeed;
						}
						else if ( ActionAnimation != "null" )
						{
						}
					}
					break;
				case moveToMode.Instantaneous: // Instantanious teleport to Target
					TargetNPC.Position = this.Position;
					if ( ActionAnimation != "null" )
					{

						//TargetNPC.targetRotation = this.Rotation;
					}
					break;
				default:
					TargetNPC.Steer.Target = this.Position;
					if ( TargetNPC.NPCAnimGraph != "" )
					{
						if ( !(TargetNPC.Position.AlmostEqual( this.Position, 16 ) || TargetNPC.Position == this.Position) )
						{
							TargetNPC.SetAnimGraph( TargetNPC.NPCAnimGraph );
							TargetNPC.UseAnimGraph = true;
							TargetNPC.CurrentSequence.Name = ActionAnimation;

							//TargetNPC.targetRotation = this.Rotation;
						}
						else if ( ActionAnimation != "null" )
						{
							TargetNPC.CurrentSequence.Name = ActionAnimation;

							//TargetNPC.targetRotation = this.Rotation;
							timeduration = TargetNPC.CurrentSequence.Duration;
							timetick = 0;
						}
					}
					break;
			}

			//TargetNPC.targetRotation = this.Rotation;
			//Log.Info("script sequence target set to " + TargetEntity);
			//TargetEntity.Position = this.Position; //TODO make this do something lol
		}
	}
	bool hasStarted = false;
	bool readyToPlay = false;

	[Input]
	void CancelSequence()
	{

		if ( TargetNPC is not NPC )
		{
			TargetNPC = FindByName( TargetEntity ) as NPC;
		}
		//hasStarted = false;
		ticker = false;
		ticker2 = false;
		timesince = -1;
		timetick = 0;
		timeout = 0;
		timeduration = 0;
		TargetNPC.Speed = TargetNPC.WalkSpeed;
		if ( SpawnSettings.HasFlag( Flags.PriorityScript ) )
		{
			TargetNPC.InPriorityScriptedSequence = false;
		}

		if ( SpawnSettings.HasFlag( Flags.OverrideAI ) )
		{
			TargetNPC.ScriptedSequenceOverrideAi = false;
		}
		TargetNPC.InScriptedSequence = false;
		//hasStarted = false;
	}
	[Input]
	void MoveToPosition()
	{

		TargetNPC.InScriptedSequence = true;
		Move();
	}
	public override void Spawn()
	{

		if ( TargetLegacy != "" )
		{
			TargetLegacyEnt = FindByName( TargetLegacy );
		}
		if ( KillTargetLegacy != "" )
		{
			KillTargetLegacyEnt = FindByName( KillTargetLegacy );
		}

	}
	float timetick = 0;
	float timeout = 0;
	float timeduration = 0;
	bool ticker = false;
	bool ticker2 = false;
	bool preticker = false;
	bool preticker2 = false;
	bool preactionplayed = false;
	bool startonspawncheckdone = false;
	float timesince = -1;
	[GameEvent.Tick.Server]
	public void Tick()
	{
		if ( TargetNPC is NPC && TargetNPC.IsValid() && TargetNPC.Velocity.AlmostEqual( this.Position ) )
		{
			timeout += 1;
		}
		if ( SpawnSettings.HasFlag( Flags.StartonSpawn ) && startonspawncheckdone == false )////this is a check that if the entity is meant to start on spawn
		{
			if ( TargetNPC is not NPC )
			{
				TargetNPC = FindByName( TargetEntity ) as NPC;
			}
			if ( PreActionAnimation != null )////if we have a preaction we dont want to begain the sequence 
			{
				MoveToPosition();
				hasStarted = true;
			}
			else
				BeginSequence();//if we dont just play the sequence 

			startonspawncheckdone = true;
		}

		if ( hasStarted == false )
		{


			return;
		}
		if ( TargetNPC is NPC && TargetNPC != null && TargetNPC.IsValid && ((TargetNPC.Position.AlmostEqual( this.Position, 3.0f + (timeout / 1000) ) && TargetNPC.Steer.Output.Finished) || TargetNPC.Position == this.Position || (TargetNPC.Steer.Output.Finished && TargetNPC.Position.AlmostEqual( this.Position, 8.0f + (timeout / 1000) ))) ) //&& TargetNPC.CurrentSequence.IsFinished == true 
		{
			if ( readyToPlay )
			{
				//TargetNPC.Position = this.Position;
				timetick += 0.01f;

				if ( ticker == true && ticker2 == false ) // next tick has happened, play the animation
				{
					TargetNPC.Position = this.Position;
					TargetNPC.InScriptedSequence = true;
					ticker2 = true;
					timetick = 0;
					//TargetNPC.targetRotation = this.Rotation;
				}

				if ( ticker == false ) // we've reached our goal, run this once, wait for next tick over to play the animation
				{
					ticker = true;
					//Log.Info("script sequence target reached");
					OnBeginSequence.Fire( this );

					// this is ass, when is direct playback in animgraph coming in?
					//TargetNPC.SetAnimGraph("");
					//TargetNPC.UseAnimGraph = false; // use animgraph = false does nothing... why?
					TargetNPC.CurrentSequence.Name = ActionAnimation;
					timeduration = TargetNPC.CurrentSequence.Duration;
					TargetNPC.DirectPlayback.Play( ActionAnimation );
					TargetNPC.PlaybackRate = 0.5f;
					TargetNPC.targetRotationOVERRIDE = this.Rotation;

					TargetNPC.targetRotation = this.Rotation;
				}






			}
			else if ( PreActionAnimation != null )
			{
				if ( preticker == true && preticker2 == false ) // next tick has happened, play the animation
				{
					TargetNPC.Position = this.Position;
					TargetNPC.InScriptedSequence = true;
					preticker2 = true;
					//TargetNPC.targetRotation = this.Rotation;
				}

				if ( ticker == false ) // we've reached our goal, run this once, wait for next tick over to play the animation
				{
					preticker = true;

					// this is ass, when is direct playback in animgraph coming in?
					//TargetNPC.SetAnimGraph("");
					//TargetNPC.UseAnimGraph = false; // use animgraph = false does nothing... why?
					TargetNPC.CurrentSequence.Name = ActionAnimation;
					timeduration = TargetNPC.CurrentSequence.Duration;
					TargetNPC.DirectPlayback.Play( ActionAnimation );
					TargetNPC.PlaybackRate = 0.5f;
					TargetNPC.targetRotationOVERRIDE = this.Rotation;

					TargetNPC.targetRotation = this.Rotation;
				}
			}
		}
		if ( timetick > timeduration ) // the animation has finished playing
		{
			timetick = 0;
			if ( LoopActionAnimation == true && ticker && ticker2 )
			{
				ticker = false;
				ticker2 = false;
			}
			else
			if ( PostActionAnimation != "null" && ticker && ticker2 )
			{
				TargetNPC.CurrentSequence.Name = PostActionAnimation;
				TargetNPC.DirectPlayback.Play( PostActionAnimation );
				ticker = false;
				ticker2 = false;
			}
			else
			{
				timesince = Time.Now + DelayLegacy;
				if ( TargetNPC.NPCAnimGraph != "" )
				{
				}
			}
			TargetNPC.Speed = TargetNPC.WalkSpeed;
			OnEndSequence.Fire( this );
			//TargetNPC.targetRotation = this.Rotation;
			hasStarted = false;
			if ( SpawnSettings.HasFlag( Flags.PriorityScript ) )
			{
				TargetNPC.InPriorityScriptedSequence = false; // does this even fucking work?????
			}
			if ( SpawnSettings.HasFlag( Flags.OverrideAI ) )
			{
				TargetNPC.ScriptedSequenceOverrideAi = false;
			}
			TargetNPC.InScriptedSequence = false;
			//if (TargetLegacy != "")
			//GameTask.RunInThreadAsync(TriggerTask);



		}
		/*
        
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
        */
	}

}
