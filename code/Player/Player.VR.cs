partial class HLPlayer 
{
	public bool IsInVR = false;
	[Net, Local] public VRHandLeft LeftHand { get; set; }
	[Net, Local] public VRHandRight RightHand { get; set; }

	private void CreateHands()
	{
		DeleteHands();

		LeftHand = new() { Owner = this };
		RightHand = new() { Owner = this };
		LeftHand.Spawn();
		RightHand.Spawn();
	}

	private void DeleteHands()
	{
		LeftHand?.Delete();
		RightHand?.Delete();
	}

	public Rotation vrrotate { get; set; }
	public void rotationvr()
	{
		vrrotate = Rotation.FromYaw( vrrotate.Yaw() - (float)Math.Round( Input.VR.RightHand.Joystick.Value.x, 1 ) * 4 );


		var a = Transform;
		//a.Position = Rotation.FromAxis(Vector3.Up, -(Input.VR.RightHand.Joystick.Value.x * 4)) * (Transform.Position - Input.VR.Head.Position.WithZ(Position.z)) + Input.VR.Head.Position.WithZ(Position.z);

		a.Rotation = vrrotate;// Rotation.FromAxis(Vector3.Up, -(Input.VR.RightHand.Joystick.Value.x * 4)) * Transform.Rotation;

		EyeRotation = a.Rotation;
		Transform = a;
	}

	public void VRFrameSimulate( IClient cl )
	{
		rotationvr();

		var postProcess = Camera.Main.FindOrCreateHook<Sandbox.Effects.ScreenEffects>();

		if ( Health > 0 )
		{
			LeftHand.FrameSimulate( cl );
			RightHand.FrameSimulate( cl );
			postProcess.Saturation = 1;
		}
		else
		{
			postProcess.Saturation = 0;
		}

		if ( LeftHand != null && RightHand != null )
			if ( HasHEV )
			{
				LeftHand.SetModel( "models/vr/v_hand_hevsuit/v_hand_hevsuit_left.vmdl" );
				RightHand.SetModel( "models/vr/v_hand_hevsuit/v_hand_hevsuit_right.vmdl" );
			}
			else
			{
				LeftHand.SetModel( "models/vr/v_hand_labcoat/v_hand_labcoat_left.vmdl" );
				RightHand.SetModel( "models/vr/v_hand_labcoat/v_hand_labcoat_right.vmdl" );
			}
	}
}
