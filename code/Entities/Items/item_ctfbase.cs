[Library( "item_ctfbase" ), HammerEntity]
[EditorModel( "models/op4/ctf/civ_stand.vmdl" )]
[Title(  "item_ctfbase" ), Category("Capture The Flag"), MenuCategory( "Items" )]
partial class item_ctfbase : ModelEntity
{
	[Property, Title("Team")]
	public int goal_no {get; set;}

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op4/ctf/civ_stand.vmdl" );

		UsePhysicsCollision = true;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );
		if ( other is not HLPlayer player ) return;

		if(player.IsCarryingFlag &&player.team == goal_no){
			switch (player.team)
			{
				case 1: (HLGame.Current as HLGame).ScoreTeamBM += 1; break;
				case 2: (HLGame.Current as HLGame).ScoreTeamOF += 1; break;
			}
			Log.Info(player.Parent.Name+" has scored!");
		}

	}
}
