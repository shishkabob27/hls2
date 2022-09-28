[Library( "item_ctfflag" ), HammerEntity]
[EditorModel( "models/op4/ctf/flag.vmdl" )]
[Title(  "item_ctfflag" ), Category("Capture The Flag")]
partial class item_ctfflag : ModelEntity
{
	[Property]
	public int skin {get; set;} = 1;

	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op4/ctf/flag.vmdl" );

		UsePhysicsCollision = true;

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
	}
}
