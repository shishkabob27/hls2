public partial class scripted_sequence : Entity
{
	public void MoveTo(MoveToMode moveMode)
	{
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
				break;
			case MoveToMode.No_turn_to_face:
				break;
		}
	}
	void WalkTo(bool running = false)
	{
		TargetNPC.Steer.Target = Position;
		TargetNPC.Speed = running ? TargetNPC.WalkSpeed : TargetNPC.RunSpeed;
	}
}
