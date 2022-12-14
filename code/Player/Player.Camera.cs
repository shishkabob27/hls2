public partial class HLPlayer
{
	private void UpdateCamera()
	{
		
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		if ( LifeState == LifeState.Dead )
		{
			DeadCameraSimulate();
			return;
		}

		if ( ThirdPerson )
		{
			ThirdPersonCameraSimulate();
		}
		else
		{
			FirstPersonCameraSimulate();
		}
	}


	void FirstPersonCameraSimulate()
	{
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.Position = EyePosition;
		Camera.FirstPersonViewer = this;
		// View Bob
		AddViewBob();
		// View Roll 
		//AddRoll();
		// Punch Angles
		//AddPunch();
		// env_shake and other shaky things
		V_CalcShake();

		if ( ActiveChild is Weapon weapon )
		{
			weapon.UpdateViewmodelCamera();
			//weapon.UpdateCamera();
		}

		Camera.Main.SetViewModelCamera( Camera.FieldOfView );
	}

	void ThirdPersonCameraSimulate()
	{
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.FirstPersonViewer = null;

		Vector3 targetPos;
		var center = this.Position + Vector3.Up * 64;

		var pos = center;
		var rot = Rotation.FromAxis( Vector3.Up, -16 ) * Camera.Rotation;

		float distance = 130.0f * Scale;
		targetPos = pos + rot.Right * ((CollisionBounds.Mins.x + 32) * Scale);
		targetPos += rot.Forward * -distance;

		var tr = Trace.Ray( pos, targetPos )
			.WithAnyTags( "solid" )
			.Ignore( this )
			.Radius( 8 )
			.Run();

		Camera.Position = tr.EndPosition;
	}

	void DeadCameraSimulate()
	{
		Camera.FirstPersonViewer = this;
		Camera.Position = Position;//+ new Vector3( 0f, 0f, 24f );
		Camera.Rotation = (ViewAngles + new Angles( 0, 0, 80 )).ToRotation();
		Log.Info( "DEADCAMERA" );
	}

}
