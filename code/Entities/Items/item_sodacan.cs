[Library( "item_sodacan" ), HammerEntity]
[EditorModel( "models/hl1/items/can.vmdl" )]
[Title(  "item_sodacan" ), Category("Items")]
partial class item_sodacan : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/items/can.vmdl" );

	public int health { get; set; } = 0;

	/// <summary>
	/// 0: Coca-Cola
    /// 1: Sprite
    /// 2: Diet Coke
    /// 3: Orange
    /// 4: Surge
    /// 5: Moxie
    /// 6: Random
	/// </summary>
	public int type { get; set; } = 0;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		Tags.Add("weapon");
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not HLPlayer player ) return;
		if ( player.Health >= 100 ) return;

		var newhealth = player.Health + health;

		newhealth = newhealth.Clamp( 0, 100 );

		player.Health = newhealth;

		if (IsServer)
			Delete();
	}
}
