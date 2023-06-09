
/// <summary>
/// A trigger volume that damages entities that touch it.
/// </summary>
[Library( "trigger_hurt" )]
[HammerEntity, Solid]
[Title( "Hurt Volume" ), Category( "Triggers" ), Icon( "personal_injury" )]
public partial class HurtVolumeEntity : BaseTrigger
{

	[Flags]
	public enum Flags
	{
		Clients = 1,
		NPCs = 2,
		Pushables = 4,
		PhysicsObjects = 8,
		OnlyPlayerAllyNPCs = 16,
		OnlyClientsInVehicles = 32,
		Everything = 64,
	}


	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags SpawnSettings { get; set; }

	/// <summary>
	/// Amount of damage to deal to touching entities per second.
	/// </summary>
	[Property( "damage", Title = "Damage" )]
	public float Damage { get; set; } = 10.0f;

	[Property( "StartDisabled", Title = "Start Disabled" )]
	public bool StartDisabled { get; set; } = false;

	public override void Spawn()
	{
		base.Spawn();
		if ( StartDisabled ) Enabled = false;
	}

	/// <summary>
	/// Fired when a player gets hurt by this trigger
	/// </summary>
	protected Output OnHurtPlayer { get; set; }

	/// <summary>
	/// Fired when anything BUT a player gets hurt by this trigger
	/// </summary>
	protected Output OnHurt { get; set; }

	/// <summary>
	/// Called every server tick to deal damage to touching entities.
	/// </summary>
	[Event.Tick.Server]
	protected virtual void DealDamagePerTick2()
	{
		ActivationTags = "";
		if ( !Enabled )
			return;

		foreach ( var entity in TouchingEntities )
		{
			if ( !entity.IsValid() )
				continue;

			entity.TakeDamage( DamageInfo.Generic( Damage * Time.Delta ).WithAttacker( this ) );

			if ( entity.Tags.Has( "player" ) )
			{
				OnHurtPlayer.Fire( entity );
			}
			else
			{
				OnHurt.Fire( entity );
			}
		}
	}

	/// <summary>
	/// Sets the damage per second for this trigger_hurt
	/// </summary>
	[Input]
	protected void SetDamage( float damage )
	{
		Damage = damage;
	}
}

