[Library( "item_sodacan" ), HammerEntity]
[EditorModel( "models/hl1/items/can.vmdl" )]
[Title(  "Soda" ), Category("Items")]
partial class item_sodacan : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/items/can.vmdl" );

	public int health { get; set; } = 1;

	/// <summary>
	/// 0: Coca-Cola
    /// 1: Sprite
    /// 2: Diet Coke
    /// 3: Orange
    /// 4: Surge
    /// 5: Moxie
    /// 6: Random
	/// </summary>
	public int type { get; set; } = 6;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		Tags.Add("weapon");

		switch (type)
		{
			case 0: SetMaterialGroup(1); break;
			case 1: SetMaterialGroup(2); break;
			case 2: SetMaterialGroup(3); break;
			case 3: SetMaterialGroup(4); break;
			case 4: SetMaterialGroup(5); break;
			case 5: SetMaterialGroup(6); break;
			case 6: SetMaterialGroup(Rand.Int(1,6)); break;
			default: SetMaterialGroup(Rand.Int(1,6)); break;
		}
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
