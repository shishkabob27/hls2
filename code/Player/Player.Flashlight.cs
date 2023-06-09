public partial class HLPlayer
{

	[ConVar.Replicated] public static float r_flashlightfarz { get; set; } = 1024;
	[Net, Predicted]
	public bool FlashlightEnabled { get; private set; } = false;

	[Net, Predicted]
	public Entity Light { get; private set; }
	[GameEvent.Client.Frame]
	void flashlightframe()
	{
		if ( Light.IsValid() )
		{
			if ( Client.GetClientData( "hl_classic_flashlight" ) == "1" )
			{
				var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 1500 )
					.Ignore( this )
					.Run();
				Light.Position = tr.EndPosition + tr.Normal * 2;
			}
			else
			{
				Light.Position = EyePosition + EyeRotation.Forward * 15;
			}

			Light.Rotation = EyeRotation;

			if ( Client.IsUsingVr )
			{
				if ( Client.GetClientData( "hl_classic_flashlight" ) == "1" )
				{
					var tr = Trace.Ray( EyeLocalPosition, EyeLocalPosition + Input.VR.Head.Rotation.Forward * 1500 )
						.Ignore( this )
						.Run();
					Light.Position = tr.EndPosition + tr.Normal * 2;
				}
				else
				{
					Light.Position = EyeLocalPosition;
				}
				Light.Rotation = Input.VR.Head.Rotation;
			}

			if ( Light is SpotLightEntity && Game.LocalPawn is HLPlayer Ply )
			{
				try
				{
					(Light as SpotLightEntity).EnableViewmodelRendering = (Ply.CameraMode is FirstPersonCamera && !Client.Components.Get<DevCamera>( true ).Enabled); // Do not viewmodel render in third person

				}
				catch { }
			}
		}
	}
	public void SimulateFlashlight( IClient cl )
	{
		if ( Light.IsValid() )
		{
			if ( Client.GetClientData( "hl_classic_flashlight" ) == "1" )
			{
				var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 1500 )
					.Ignore( this )
					.Run();
				Light.Position = tr.EndPosition + tr.Normal * 2;
			}
			else
			{
				Light.Position = EyePosition + EyeRotation.Forward * 15;
			}
			Light.Rotation = EyeRotation;

			if ( Client.IsUsingVr )
			{
				if ( Client.GetClientData( "hl_classic_flashlight" ) == "1" )
				{
					var tr = Trace.Ray( EyeLocalPosition, EyeLocalPosition + Input.VR.Head.Rotation.Forward * 1500 )
						.Ignore( this )
						.Run();
					Light.Position = tr.EndPosition + tr.Normal * 2;
				}
				else
				{
					Light.Position = EyeLocalPosition;
				}
				Light.Rotation = Input.VR.Head.Rotation;
			}
		}
		if ( Input.Pressed( "Flashlight" ) && HasHEV )
		{
			FlashlightEnabled = !FlashlightEnabled;

			PlaySound( "flashlight1" );

			if ( Light.IsValid() == false && FlashlightEnabled && Game.IsServer )
			{
				NewFlashlight();
			}

			if ( Light.IsValid() == true && !FlashlightEnabled && Game.IsServer )
			{
				RemoveFlashlight();
			}
		}


	}



	protected void NewFlashlight()
	{
		if ( Light.IsValid() )
		{
			Light.Delete();
		}
		if ( Client.GetClientData( "hl_classic_flashlight" ) == "1" )
		{
			Light = new PointLightEntity();
			(Light as PointLightEntity).Predictable = true;
			//(Light as PointLightEntity).LightCookie = Texture.Load("materials/effects/lightcookie.vtex");
			(Light as PointLightEntity).DynamicShadows = true;
			(Light as PointLightEntity).Range = r_flashlightfarz;
			(Light as PointLightEntity).Falloff = 1.0f;
			(Light as PointLightEntity).LinearAttenuation = 0.0f;
			(Light as PointLightEntity).QuadraticAttenuation = 1.0f;
			(Light as PointLightEntity).Brightness = 1;
			(Light as PointLightEntity).Color = Color.White;
			(Light as PointLightEntity).Range = 64;
			(Light as PointLightEntity).DynamicShadows = false;
			//(Light as PointLightEntity).InnerConeAngle = 20;
			//(Light as PointLightEntity).OuterConeAngle = 40;
			(Light as PointLightEntity).FogStrength = 1.0f;
			(Light as PointLightEntity).Owner = this;

			(Light as PointLightEntity).Enabled = true;
		}
		else
		{
			Light = new SpotLightEntity();
			(Light as SpotLightEntity).Predictable = true;
			(Light as SpotLightEntity).LightCookie = Texture.Load( "materials/effects/lightcookie.vtex" );
			(Light as SpotLightEntity).DynamicShadows = true;
			(Light as SpotLightEntity).Range = r_flashlightfarz;
			(Light as SpotLightEntity).Falloff = 1.0f;
			(Light as SpotLightEntity).Range = 0;
			(Light as SpotLightEntity).LinearAttenuation = 0.0f;
			(Light as SpotLightEntity).QuadraticAttenuation = 1.0f;
			(Light as SpotLightEntity).Brightness = 2;
			(Light as SpotLightEntity).Color = Color.White;
			(Light as SpotLightEntity).InnerConeAngle = 20;
			(Light as SpotLightEntity).OuterConeAngle = 40;
			SetupVMRender();
			(Light as SpotLightEntity).FogStrength = 1.0f;
			(Light as SpotLightEntity).Owner = this;

			(Light as SpotLightEntity).Enabled = true;
		}

	}
	[ClientRpc]
	void SetupVMRender()
	{

		(Light as SpotLightEntity).EnableViewmodelRendering = true;
	}


	protected void RemoveFlashlight()
	{
		try
		{

			Light.Delete();
			Light = null;
		}
		catch { }
	}

	[GameEvent.Entity.PostCleanup]
	public void OnMapCleanupEvent()
	{
		RemoveFlashlight();
	}

}
