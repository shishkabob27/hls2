[Library("math_counter")]
[HammerEntity]
[EditorSprite("editor/math_counter.vmat")]
[Title("math_counter"), Category("Legacy")]
public partial class MathCounter : Entity
{
	// stub

	[Property]
	public bool Enabled { get; set; } = true;

	[Property(Title = "Initial Value")]
	public float startvalue { get; set; } = 0;

	[Property(Title = "Minimum Legal Value")]
	public float min { get; set; } = 0;

	[Property(Title = "Maximum Legal Value")]
	public float max { get; set; } = 1;

	public float currentValue;

	public bool allowOutput = true;

	protected Output OnHitMax { get; set; }
	public override void Spawn()
	{
		base.Spawn();
		currentValue = startvalue;
	}

	[Event.Tick.Server]
	public void Tick()
    {
		if (currentValue == max)
        {
			OnHitMax.Fire(this);
        }
    }
	
	[Input]
	public void Add(float value)
	{
		currentValue += value;
		Log.Info("ADD");
	}

	[Input]
	public void Divide(float value)
	{
		currentValue /= value;
	}

	[Input]
	public void Multiply(float value)
	{
		currentValue /= value;
	}

	[Input]
	public void SetValue(float value)
	{
		currentValue = value;
	}

}