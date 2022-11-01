public partial class scripted_sequence : Entity
{
	public void MoveTo(MoveToMode moveMode)
	{
		DebugPrint( "Move to position started." );
		switch (moveMode)
		{
			case MoveToMode.No:
				break;
			case MoveToMode.Walk:
				WalkTo();
				break;
			case MoveToMode.Run:
				WalkTo(true);
				break;
			case MoveToMode.Custom_movement:
				break;
			case MoveToMode.Instantaneous:
				TargetNPC.Position = Position;
				TargetNPC.targetRotation = Rotation;
				TargetNPC.targetRotationOVERRIDE = Rotation;
				break;
			case MoveToMode.No_turn_to_face:
				TargetNPC.targetRotation = Rotation;
				TargetNPC.targetRotationOVERRIDE = Rotation;
				break;
		}
	}
	async Task WalkTo(bool running = false)
	{
		DebugPrint( "Walking to position." );
		TargetNPC.NPCTaskQueue.Enqueue( new MoveToTask(Position ) );
		TargetNPC.NPCTaskQueue.Enqueue( new RotateToTask(Rotation ) );
		//TargetNPC.Steer.Target = Position;
		//TargetNPC.Speed = running ? TargetNPC.RunSpeed : TargetNPC.WalkSpeed;
	}
}
