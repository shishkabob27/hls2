[Library( "math_counter" )]
[HammerEntity]
[EditorSprite( "editor/math_counter.vmat" )]
[Title( "math_counter" ), Category( "Logic" ), Icon( "calculate" )]
public partial class math_counter : Entity
{

	[Property]
	public bool Enabled { get; set; } = true;

	[Property( Title = "Initial Value" )]
	public float startvalue { get; set; } = 0;

	[Property( Title = "Minimum Legal Value" )]
	public float min { get; set; } = 0;

	[Property( Title = "Maximum Legal Value" )]
	public float max { get; set; } = 1;

	public float currentValue;

	public bool allowOutput = true;

	protected Output OnHitMax { get; set; }
	protected Output OnHitMin { get; set; }
	protected Output<float> OutValue { get; set; }
	protected Output<float> OnGetValue { get; set; }
	public override void Spawn()
	{
		base.Spawn();
		currentValue = startvalue;
	}

	[Input]
	public void Add( float value )
	{
		SetValue( currentValue + value );
	}

	[Input]
	public void Divide( float value )
	{
		SetValue( currentValue / value );
	}

	[Input]
	public void Multiply( float value )
	{
		SetValue( currentValue * value );
	}

	[Input]
	public void Subtract( float value )
	{
		SetValue( currentValue - value );
	}

	[Input]
	public void SetValue( float value )
	{
		if ( currentValue > max ) return;
		if ( currentValue < min ) return;
		currentValue = value;

		if ( currentValue >= max )
		{
			currentValue = max;
			OnHitMax.Fire( this ); return;
		}

		if ( currentValue <= min )
		{
			currentValue = min;
			OnHitMin.Fire( this ); return;
		}
		OutValue.Fire( this, currentValue );
	}

	[Input]
	public void GetValue()
	{
		OnGetValue.Fire( this, currentValue );
	}

}
