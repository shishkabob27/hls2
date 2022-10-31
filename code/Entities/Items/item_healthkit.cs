/// <summary>
/// Gives 25 health points.
/// </summary>
[Library("item_healthkit"), HammerEntity]
[EditorModel( "models/hl1/gameplay/medkit.vmdl" )]
[Title(  "Health Kit" ), Category("Items"), MenuCategory( "Items" )]
public partial class HealthKit : ModelEntity, IRespawnableEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/gameplay/medkit.vmdl" );

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

		if ( other is not HLPlayer pl ) return;
		if ( pl.Health >= pl.MaxHealth ) return;

		var newhealth = pl.Health + 25;

		newhealth = newhealth.Clamp( 0, pl.MaxHealth );

		pl.Health = newhealth;

		Sound.FromWorld( "dm.item_health", Position );
		ItemRespawn.Taken( this );
		if (IsServer)
			Delete();
	}
}
