[Library( "speaker" )]
[HammerEntity]
[Title( "speaker" ), Category("Legacy"), Icon("speaker")]
public class speaker : Entity
{
	// stub

	[Property] public int preset { get; set; }
	[Property] public int health { get; set; }
	[Property] public float radius { get; set; }

	[Property] public int delaymin { get; set; }
	[Property] public int delaymaxs { get; set; }

	[Flags]
	public enum Flags
	{
		StartSilent = 1
	}


	[Property( Title = "Spawn Settings" )]
	public Flags spawnflags { get; set; }

	/// <summary>
	/// The (initial) enabled state of the logic entity.
	/// </summary>
	[Property]
	public bool Enabled { get; set; } = true;

	/// <summary>
	/// Enables the entity.
	/// </summary>
	[Input]
	public void Enable()
	{
		Enabled = true;
	}

	/// <summary>
	/// Disables the entity, so that it would not fire any outputs.
	/// </summary>
	[Input]
	public void Disable()
	{
		Enabled = false;
	}

	/// <summary>
	/// Toggles the enabled state of the entity.
	/// </summary>
	[Input]
	public void Toggle()
	{
		Enabled = !Enabled;
	}

}
