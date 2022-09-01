using System.Diagnostics;

public partial class HLPlayer
{
    [ConVar.Replicated] public static float r_flashlightfarz { get; set; } = 1024;
    [Net, Predicted]
    public bool FlashlightEnabled { get; private set; } = false;

    [Net, Predicted]
    public SpotLightEntity Light { get; private set; }

    public void SimulateFlashlight()
    {

        if (Light.IsValid() && IsServer)
        {
            Light.Position = EyePosition + EyeRotation.Forward * 15;
            Light.Rotation = EyeRotation;
        }
        if (Input.Pressed(InputButton.Flashlight))
        {
            FlashlightEnabled = !FlashlightEnabled;

            PlaySound("flashlight1");

            if (Light.IsValid() == false && FlashlightEnabled && IsServer)
            {
                NewFlashlight();
            }

            if (Light.IsValid() == true && !FlashlightEnabled && IsServer)
            {
                RemoveFlashlight();
            }
        }


    }

    

    protected void NewFlashlight()
    {
        Light = new SpotLightEntity();
        Light.Predictable = true;
        Light.LightCookie = Texture.Load("materials/effects/lightcookie.vtex");
        
        Light.DynamicShadows = true;
        Light.Range = r_flashlightfarz;
        Light.Falloff = 1.0f;
        Light.LinearAttenuation = 0.0f;
        Light.QuadraticAttenuation = 1.0f;
        Light.Brightness = 2;
        Light.Color = Color.White;
        Light.InnerConeAngle = 20;
        Light.OuterConeAngle = 40;
        Light.FogStrength = 1.0f;
        

        Light.Enabled = true;
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

    [Event.Entity.PostCleanup]
    public void OnMapCleanupEvent()
    {
        RemoveFlashlight();
    }

}