/// <summary>
/// Gives 25 Armour
/// </summary>
[Library( "item_battery" ), HammerEntity]
[EditorModel( "models/hl1/gameplay/battery.vmdl" )]
[Title(  "Battery" ), Category("Items")]
partial class Battery : ModelEntity, IRespawnableEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/gameplay/battery.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		var c = Components.GetOrCreate<Movement>();

		Tags.Add("weapon");
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not HLPlayer player ) return;
		if ( player.Armour >= 100 ) return;
		if ( !player.HasHEV ) return;

		var newhealth = player.Armour + 15;

		newhealth = newhealth.Clamp( 0, 100 );

		player.Armour = newhealth;

		Sound.FromWorld( "dm_item_battery", Position );
		PickupFeed.OnPickup( To.Single( player ), $"+15 Armour" );

		ItemRespawn.Taken( this );
		if (IsServer)
			Delete();
	}
}
