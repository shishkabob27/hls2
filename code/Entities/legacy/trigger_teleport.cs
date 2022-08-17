using SandboxEditor;

namespace Sandbox;

/// <summary>
/// A simple trigger volume that teleports entities that touch it.
/// </summary>
[Library( "trigger_teleport" )]
[HammerEntity, Solid]
[Title( "Trigger Teleport" ), Category( "Triggers" ), Icon( "auto_fix_normal" )]
public partial class TeleportVolumeEntity : BaseTrigger
{
	/// <summary>
	/// The entity specifying a location to which entities should be teleported to.
	/// </summary>
	[Property( "target" ), Title( "Remote Destination" )]
	public EntityTarget TargetEntity { get; set; }

	/// <summary>
	/// If set, teleports the entity with an offset depending on where the entity was in the trigger teleport. Think world portals. Place the target entity accordingly.
	/// </summary>
	[Property( "teleport_relative" ), Title( "Teleport Relatively" )]
	public bool TeleportRelative { get; set; }

	/// <summary>
	/// If set, the teleported entity will not have it's velocity reset to 0.
	/// </summary>
	[Property( "keep_velocity" ), Title( "Keep Velocity" )]
	public bool KeepVelocity { get; set; }
    
	/// <summary>
	/// Should the trigger be enabled or not when spawned?
	/// </summary>
	[Property("StartDisabled"), Title("Start Disabled")]
	public bool StartDisabled { get; set; } = false;
    
	/// <summary>
	/// Fired when the trigger teleports an entity
	/// </summary>
	protected Output OnTriggered { get; set; }
	public TeleportVolumeEntity()
    {
        Enabled = !StartDisabled;
    }
	public override void Spawn()
	{
		Enabled = !StartDisabled;
	}
	public override void OnTouchStart( Entity other )
	{
		if ( !Enabled ) return;

		var Targetent = TargetEntity.GetTargets( null ).FirstOrDefault();

		if ( Targetent != null )
		{
			Vector3 offset = Vector3.Zero;
			if ( TeleportRelative )
			{
				offset = other.Position - Position;
			}

			if ( !KeepVelocity ) other.Velocity = Vector3.Zero;

			// Fire the output, before actual teleportation so entity IO can do things like disable a trigger_teleport we are teleporting this entity into
			OnTriggered.Fire( other );

			other.Transform = Targetent.Transform;
			other.Position += offset;
		}
	}
}

[System.Obsolete( "Renamed to TeleportVolumeEntity" )]
class TriggerTeleport : TeleportVolumeEntity
{
}
