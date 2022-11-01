namespace XeNPC;

using Sandbox;
using System;
using XeNPC.Debug;

public partial class NPC  
{

	public Queue<NPCTask> NPCTaskQueue = new Queue<NPCTask>();
	 
	async Task ProcessQueue()
	{
		if ( NPCTaskQueue.Count != 0 )
		{

			NPCTask CurrentTask = NPCTaskQueue.Dequeue();
			await CurrentTask.HandleTask(this);
			Log.Info( "Task Finished!" );
		}
		await GameTask.Delay( 1 );
		ProcessQueue();
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
	public MoveToTask(Vector3 pos)
	{
		Position = pos;
	}
	public MoveToTask(Vector3 pos, scripted_sequence seq )
	{
		Position = pos;
		Sequence = seq;
	}
	public override async Task HandleTask( NPC owner )
	{
		owner.Steer.Output.Finished = false;
		owner.Steer.Target = Position;
		while ( !owner.Steer.Output.Finished )
		{

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

		} 
		owner.DirectPlayback.Cancel();
		OnEnd();
		return;
	}
}
