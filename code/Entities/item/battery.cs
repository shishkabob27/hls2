/// <summary>
/// Gives 25 Armour
/// </summary>
[Library( "item_battery" ), HammerEntity]
[EditorModel( "models/hl1/gameplay/battery.vmdl" )]
[Title(  "Battery" )]
partial class Battery : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/gameplay/battery.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not HLPlayer player ) return;
		if ( player.Armour >= 100 ) return;

		var newhealth = player.Armour + 15;

		newhealth = newhealth.Clamp( 0, 100 );

		player.Armour = newhealth;

		Sound.FromWorld( "dm_item_battery", Position );
		PickupFeed.OnPickup( To.Single( player ), $"+15 Armour" );

		Delete();
	}
}
