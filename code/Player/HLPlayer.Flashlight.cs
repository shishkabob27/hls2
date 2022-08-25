using System.Diagnostics;

public partial class HLPlayer
{
    [Net, Predicted]
    public bool FlashlightEnabled { get; private set; } = false;

    private SpotLightEntity Light;

    public void SimulateFlashlight()
    {

        if (Light.IsValid())
        {
            var transform = new Transform(EyePosition + EyeRotation.Forward * 15, EyeRotation);
            transform.Rotation *= Rotation.From(new Angles(0, 0, 0));
            Light.Transform = transform;
        }

        if (Input.Pressed(InputButton.Flashlight))
        {
            FlashlightEnabled = !FlashlightEnabled;
            if (Light.IsValid())
                Light.Enabled = FlashlightEnabled;
            PlaySound(FlashlightEnabled ? "flashlight.on" : "flashlight.off");

        }
    }

    protected void NewFlashlight()
    {
        Light = new SpotLightEntity();
        Light.LightCookie = Texture.Load("materials/effects/lightcookie.vtex");

        Light.Parent = this;

        Light.DynamicShadows = true;
        Light.Range = 512;
        Light.Falloff = 1.0f;
        Light.LinearAttenuation = 0.0f;
        Light.QuadraticAttenuation = 1.0f;
        Light.Brightness = 2;
        Light.Color = Color.White;
        Light.InnerConeAngle = 20;
        Light.OuterConeAngle = 40;
        Light.FogStrength = 1.0f;


        Light.Enabled = false;
        FlashlightEnabled = false;
    }

    protected void RemoveFlashlight()
    {
        Light.Delete();
        Light = null;
    }

}