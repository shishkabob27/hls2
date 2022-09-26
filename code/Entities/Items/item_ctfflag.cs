[Library( "item_ctfflag" ), HammerEntity]
[EditorModel( "models/op/ctf/flag.vmdl" )]
[Title(  "item_ctfflag" ), Category("Capture The Flag")]
partial class item_ctfflag : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op/ctf/flag.vmdl" );

		UsePhysicsCollision = true;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );
	}
}
