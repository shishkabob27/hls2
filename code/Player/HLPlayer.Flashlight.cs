using System.Diagnostics;

public partial class HLPlayer
{
    [Net, Predicted]
    public bool FlashlightEnabled { get; private set; } = false;

    [Predicted]
    public SpotLightEntity Light { get; private set; }

    public void SimulateFlashlight()
    {

        if (Light.IsValid())
        {
            Light.Position = EyePosition + EyeRotation.Forward * 15;
            Light.Rotation = EyeRotation;
        }

        if (Input.Pressed(InputButton.Flashlight))
        {
            FlashlightEnabled = !FlashlightEnabled;
            if (Light.IsValid() == false && FlashlightEnabled)
            {
                NewFlashlight();
            }
            if (Light.IsValid() == true && !FlashlightEnabled)
            {
                RemoveFlashlight();
            }
            PlaySound(FlashlightEnabled ? "flashlight.on" : "flashlight.off");

        }
    }

    protected void NewFlashlight()
    {
        Light = new SpotLightEntity();
        Light.Predictable = true;
        Light.LightCookie = Texture.Load("materials/effects/lightcookie.vtex");
        
        Light.DynamicShadows = true;
        Light.Range = 1024;
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
        Light.Delete();
        Light = null;
    }
}