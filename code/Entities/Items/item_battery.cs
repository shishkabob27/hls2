/// <summary>
/// Gives 25 Armour
/// </summary>
[Library( "item_battery" ), HammerEntity]
[EditorModel( "models/hl1/gameplay/battery.vmdl" )]
[Title(  "Battery" ), Category("Items"), MenuCategory( "Items" )]
public partial class Battery : ModelEntity, IRespawnableEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/gameplay/battery.vmdl" );

	PointLightEntity Light { get; set; }
	PointLightEntity Light2 { get; set; }
	PointLightEntity Light3 { get; set; }
	PointLightEntity Light4 { get; set; }
	
	bool isSetup = false;
	public override void Spawn()
	{
		base.Spawn();

		Light = new PointLightEntity();
		Light.Position = CollisionWorldSpaceCenter + new Vector3( 6.66f, 0, 12 );
		Light.Brightness = 0.05f;
		Light.Color = Color.FromRgb( 0x44CFFF );
		Light.Parent = this;
		Light.DynamicShadows = true;
		Light.Enabled = HLGame.hl_dynamic_light;


		Light2 = new PointLightEntity();
		Light2.Position = CollisionWorldSpaceCenter + new Vector3( -6.66f, 0, 12 );
		Light2.Brightness = 0.05f;
		Light2.Color = Color.FromRgb( 0x44CFFF );
		Light2.Parent = this;
		Light2.DynamicShadows = true;
		Light2.Enabled = HLGame.hl_dynamic_light;


		Light3 = new PointLightEntity();
		Light3.Position = CollisionWorldSpaceCenter + new Vector3( 0, 6.66f, 12 );
		Light3.Brightness = 0.05f;
		Light3.Color = Color.FromRgb( 0x44CFFF );
		Light3.Parent = this;
		Light3.DynamicShadows = true;
		Light3.Enabled = HLGame.hl_dynamic_light;


		Light4 = new PointLightEntity();
		Light4.Position = CollisionWorldSpaceCenter + new Vector3( 0, -6.66f, 12);
		Light4.Brightness = 0.05f;
		Light4.Color = Color.FromRgb( 0x44CFFF );
		Light4.Parent = this;
		Light4.DynamicShadows = true;
		Light4.Enabled = HLGame.hl_dynamic_light;

		isSetup = true;

		Model = WorldModel;
		var c = Components.GetOrCreate<Movement>();

		Tags.Add("weapon");
	}
	//[Event.Tick.Server] 
	/*void tick()
	{
		if (isSetup)
		{
			Light.Enabled = HLGame.hl_dynamic_light;
			Light2.Enabled = HLGame.hl_dynamic_light;
			Light3.Enabled = HLGame.hl_dynamic_light;
			Light4.Enabled = HLGame.hl_dynamic_light;
		}
	}*/
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
		PickupFeed.OnPickupItem( To.Single( player ), "ui/pickup/item_battery.png" );

		ItemRespawn.Taken( this );
		if (Game.IsServer)
			Delete();
	}
}
