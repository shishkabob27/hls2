[Library( "item_ctfflag" ), HammerEntity]
[EditorModel( "models/op4/ctf/flag.vmdl" )]
[Title(  "item_ctfflag" ), Category("Capture The Flag"), MenuCategory( "Items" )]
partial class item_ctfflag : ModelEntity
{
	[Property]
	public int skin {get; set;} = 1;

	[Property, Title("Team")]
	public int goal_no {get; set;}

	public bool Carried = false;

	public bool Loose = false;

	public HLPlayer CurrentPlayer { get; set;}

	public TouchTrigger PickupTrigger { get; protected set; }

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op4/ctf/flag.vmdl" );

		UsePhysicsCollision = true;
		PhysicsEnabled = false;

		Tags.Add("weapon");

		switch (skin)
		{
			case 0: SetMaterialGroup(1); break;
			case 1: SetMaterialGroup(2); break;
			default: break;
		}
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );
		if ( other is not HLPlayer player ) return;
		if (player.team != goal_no) GiveFlagToPlayer(player);
		
	}

	public void GiveFlagToPlayer(HLPlayer player){
		player.IsCarryingFlag = true;
		CurrentPlayer = player;
		EnableDrawing = false;

		Log.Info(player.Parent.Name+" has taken the flag!");
	}
}
