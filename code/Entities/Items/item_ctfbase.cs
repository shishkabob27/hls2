[Library( "item_ctfbase" ), HammerEntity]
[EditorModel( "models/op4/ctf/civ_stand.vmdl" )]
[Title(  "item_ctfbase" ), Category("Capture The Flag")]
partial class item_ctfbase : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();

		Model = Model.Load( "models/op4/ctf/civ_stand.vmdl" );

		UsePhysicsCollision = true;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );
	}
}
