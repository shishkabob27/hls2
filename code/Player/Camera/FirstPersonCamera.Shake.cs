partial class FirstPersonCamera
{
	public float Shake_ENDTIME { get; set; } = 0;
	public float Shake_DURATION { get; set; } = 0;
	public float Shake_NEXTSHAKE { get; set; } = 0;
	public float Shake_AMPLITUDE { get; set; } = 0;
	public float Shake_FREQUENCY { get; set; } = 0;
	public Vector3 Shake_OFFSET { get; set; }
	public float Shake_ANGLE { get; set; }
	void V_CalcShake()
	{
		float shakeFraction;
		float shakeFrequency;

		if ( (Time.Now > Shake_ENDTIME) || Shake_AMPLITUDE <= 0 || Shake_FREQUENCY <= 0 || Shake_ENDTIME <= 0 )
			return;

		if ( Time.Now > Shake_NEXTSHAKE )
		{
			Shake_NEXTSHAKE = Time.Now + (1.0f / Shake_FREQUENCY);
			var a = Shake_OFFSET;
			for ( int i = 0; i < 3; i++ )
			{
				a[i] = Rand.Float( Shake_AMPLITUDE * -1, Shake_AMPLITUDE );
			}
			Shake_OFFSET = a;
			Shake_ANGLE = Rand.Float( Shake_AMPLITUDE * -0.25f, Shake_AMPLITUDE * 0.25f );
		}

		shakeFraction = (Shake_ENDTIME - Time.Now) / Shake_DURATION;

		// Ramp up
		if ( shakeFraction != 0 )
		{
			shakeFrequency = (Shake_FREQUENCY / shakeFraction);
		}
		else
		{
			shakeFrequency = 0;
		}

		// Settle
		shakeFraction *= shakeFraction;
		float angle = Time.Now * shakeFrequency;
		if ( angle > 1e8 )
		{
			angle = 1e8F;
		}
		shakeFraction *= MathF.Sin( angle );//Time.Now * shakeFrequency

		// Apply
		Position += Shake_OFFSET * shakeFraction;
		Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + Shake_ANGLE * shakeFraction ).ToRotation();

		// Lower the shake amplitute over time
		Shake_AMPLITUDE -= Shake_AMPLITUDE * (Time.Delta / (Shake_DURATION * Shake_FREQUENCY));

		if ( Global.IsRunningInVR && shakeFraction != 0 )
		{
			Input.VR.RightHand.TriggerHapticVibration( Shake_DURATION, shakeFrequency, shakeFraction );
			Input.VR.LeftHand.TriggerHapticVibration( Shake_DURATION, shakeFrequency, shakeFraction );
		}
		//Input.TriggerHapticVibration( Shake_DURATION, shakeFrequency, shakeFraction ); cant rumble controllers what the?

	}
}
