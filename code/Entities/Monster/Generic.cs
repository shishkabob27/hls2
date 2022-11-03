[Library( "monster_generic" ), HammerEntity]
[Model]
[Title( "monster_generic" ), Category( "Monsters" ), Icon( "person" )]
public partial class GenericMonster : NPC
{
	// Stub NPC, this does nothing yet

	// [Net, Property, ResourceType( "vmdl" )]
	// public string model { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		// SetModel(model);
		Health = 100;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
		//SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
		EnableHitboxes = true;
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		animHelper = new HLAnimationHelper( this );
		Tags.Add( "npc", "playerclip" );
		
	}
	[Event.Tick.Server]
	public void Ticker()
	{

	}
}
