[Library("math_counter")]
[HammerEntity]
[Title("math_counter"), Category("Legacy")]
public partial class MathCounter : Entity
{
	// stub

	[Property]
	public float min { get; set; } = 0;

	[Property]
	public float max { get; set; } = 1;

	[Property]
	public float startvalue { get; set; } = 0;

	protected Output OnHitMax { get; set; }

	[Input]
	public void Add()
	{

	}
}