public partial class HLPlayer
{
	private void UpdateCamera()
	{
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		if ( ThirdPerson )
		{
			Camera.FirstPersonViewer = null;

			Vector3 targetPos;
			var center = Position + Vector3.Up * 64;

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
		else
		{
			Camera.Position = EyePosition;
			Camera.FirstPersonViewer = this;

			if ( ActiveChild is Weapon weapon )
			{
				weapon.UpdateViewmodelCamera();
				//weapon.UpdateCamera();
			}

			Camera.Main.SetViewModelCamera( Camera.FieldOfView );

			AddCameraEffects();
		}
	}

	private void AddCameraEffects()
	{
		if ( Client.IsUsingVr ) return;
		if ( Health <= 0 ) return;
		var speed = Velocity.WithZ( 0 ).Length.LerpInverse( 0, 2 );
		var up = Rotation.Up;
	}
}
