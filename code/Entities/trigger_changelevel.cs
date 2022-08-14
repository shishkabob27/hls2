[Library( "trigger_changelevel" )]
[HammerEntity, Solid]
[Title( "Trigger Level Change" ), Category( "Triggers" ), Icon( "done" )]
public partial class ChangeLevelTrigger : BaseTrigger
{

    [Property]
	public String Map { get; set; } = "";
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

		ConsoleSystem.Run("changelevel "+ Map);
	}
}
