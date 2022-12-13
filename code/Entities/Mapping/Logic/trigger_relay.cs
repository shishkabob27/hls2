/// <summary>
/// A logic entity that allows to do a multitude of logic operations with Map I/O.<br/>
/// <br/>
/// TODO: This is a stop-gap solution and may be removed in the future in favor of "map blueprints" or node based Map I/O.
/// </summary>
[Library( "logic_relay" )]
[HammerEntity]
[VisGroup( VisGroup.Logic )]
[EditorSprite( "editor/logic_relay.vmat" )]
[Title( "Logic Relay" ), Category( "Logic" ), Icon( "calculate" )]
public partial class LogicRelay : Entity
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


	/*
		* logic_relay
		*
		*/

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

        //if (activator is not LogicRelay) //prevent dumb crash
		//{
        //Log.Info("Activating logic relay by " + activator);
		OnTrigger.Fire( this );
		//}
	}

		
}
