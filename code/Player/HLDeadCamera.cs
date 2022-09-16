	public class HLDeadCamera : CameraMode
	{
		Vector3 FocusPoint;

		public override void Activated()
		{
			base.Activated();

			FocusPoint = CurrentView.Position;
		}

		public override void Update()
		{
			var player = Local.Client;
			if (player == null) return;

			// lerp the focus point
			FocusPoint = Vector3.Lerp(FocusPoint, GetSpectatePoint(), 1.0f);

			Position = FocusPoint + new Vector3(0f, 0f, 8f);
			Rotation = Input.Rotation;

			//Viewer = Local.Pawn;
		}

		public virtual Vector3 GetSpectatePoint()
		{
			if (Local.Pawn is Player player && player.Corpse.IsValid())
			{
				return player.Corpse.GetBoneTransform(player.Corpse.GetBoneIndex("bip_01_pelvis")).Position;
			}

			return Local.Pawn.Position;
		}
	}
