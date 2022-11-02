namespace XeNPC;

using Sandbox;
using System;
using XeNPC.Debug;

public partial class NPC  
{
	[SkipHotload]
	public Queue<NPCTask> NPCTaskQueue = new Queue<NPCTask>();
	[Event.Hotload]
	void HotloadFix()
	{
		NPCTaskQueue = new Queue<NPCTask>();
	}
	 
	async Task ProcessQueue()
	{
		while (true)
		{ 
			if ( NPCTaskQueue.Count != 0 )
			{

				NPCTask CurrentTask = NPCTaskQueue.Dequeue();
				await CurrentTask.HandleTask( this );
				//Log.Info( "Task Finished!" );
			}
			await GameTask.NextPhysicsFrame();
		}
	}
} 
public class NPCTask
{
	public scripted_sequence Sequence;
	public bool DidFinish = false;
	public virtual async Task HandleTask( NPC owner )
	{
		return;
	} 
	public virtual void OnEnd()
	{ 
		DidFinish = true;
		if ( Sequence != null )
		{
			Sequence.EndSequence();
		} 
	}
}
public class MoveToTask : NPCTask
{
	
	Vector3 Position;
	bool Running;
	public MoveToTask(Vector3 pos, bool run = false )
	{
		Position = pos;
		Running = run;
	}
	public MoveToTask(Vector3 pos, scripted_sequence seq, bool run = false )
	{
		Position = pos;
		Sequence = seq;
		Running = run;
	}
	public override async Task HandleTask( NPC owner )
	{
		owner.Speed = Running ? owner.RunSpeed : owner.WalkSpeed;
		owner.Steer.Output.Finished = false;
		owner.Steer.Target = Position;
		while ( !owner.Steer.Output.Finished )
		{
			await GameTask.NextPhysicsFrame();
		}
		OnEnd();
		return;
	}
}  
public class RotateToTask : NPCTask
{
	Rotation Rotation; 
	public RotateToTask( Rotation rot)
	{
		Rotation = rot;
	}
	public RotateToTask( Rotation rot, scripted_sequence seq )
	{
		Rotation = rot;
		Sequence = seq;
	}
	public override async Task HandleTask( NPC owner )
	{
		owner.Steer.Output.Finished = false;
		owner.targetRotation = Rotation;
		owner.targetRotationOVERRIDE = Rotation;
		OnEnd();
		return;
	}
}
public class PlayAnimTask : NPCTask
{
	string Animation; 
	public PlayAnimTask( string anim )
	{
		Animation = anim;
	}
	public PlayAnimTask( string anim, scripted_sequence seq )
	{
		Animation = anim;
		Sequence = seq;
	}
	public override async Task HandleTask( NPC owner )
	{
		owner.Steer.Output.Finished = false;
		owner.DirectPlayback.Play( Animation );
		while ( owner.DirectPlayback.Time < owner.DirectPlayback.Duration )
		{
			await GameTask.NextPhysicsFrame();
		} 
		owner.DirectPlayback.Cancel();
		OnEnd();
		return;
	}
}
