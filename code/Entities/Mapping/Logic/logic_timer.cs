[Library( "logic_timer" )]
[HammerEntity]
[VisGroup( VisGroup.Logic )]
[EditorSprite("materials/editor/logic_timer.vmat")]
[Title( "logic_timer" ), Category( "Logic" ), Icon( "calculate" )]
public partial class logic_timer : Entity
{
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
	/// Enables the entity.
	/// </summary>
	[Input]
	public void TurnOn()
	{
		Enabled = true;
	}

	/// <summary>
	/// Disables the entity, so that it would not fire any outputs.
	/// </summary>
	[Input]
	public void TurnOff()
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

	[Input]
	public void Kill()
	{
		if(Game.IsServer) Delete();
	}


	/// <summary>
	/// Fired when the this entity receives the "Trigger" input.
	/// </summary>
	protected Output OnTrigger { get; set; }

	/// <summary>
	/// Trigger the "OnTrigger" output.
	/// </summary>
	[Input]
	public void Trigger()
	{
		if ( !Enabled ) return;

		OnTrigger.Fire( this );
	}

	
}
