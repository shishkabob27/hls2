[Library( "trigger_autosave" )]
[HammerEntity, Solid]
[Title( "Trigger auto save" ), Category( "Triggers" ), Icon( "done" )]
public partial class AutoSaveTrigger : BaseTrigger
{

	public override void Spawn()
	{
		base.Spawn();

		EnableTouchPersists = true;
	}

	public override void OnTouchStart( Entity other )
	{
		base.OnTouchStart( other );

		if ( !Enabled ) return;

		OnTriggered( other );

		_ = DeleteAsync( Time.Delta );
	}

	/// <summary>
	/// Called once at least a single entity that passes filters is touching this trigger, just before this trigger getting deleted
	/// </summary>
	protected Output OnTrigger { get; set; }

	public virtual void OnTriggered( Entity other )
	{
		OnTrigger.Fire( other );
	}
}
