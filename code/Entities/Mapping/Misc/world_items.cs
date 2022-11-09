[Library( "world_items" ), HammerEntity]
[Title( "world_items" ), Icon( "place" )]
public class world_items : Entity
{
	[Property]
	public int Type { get; set; } = 0;
	public override void Spawn()
	{
		base.Spawn();
		var a = "ent";
		switch ( Type )
		{
			case 42: a = "item_antidote"; break;
			case 43: a = "item_security"; break;
			case 44: a = "item_battery"; break;
			case 45: a = "item_suit"; break;
			default: break;
		}
		var entityType = TypeLibrary.GetDescription<Entity>( a );
		if ( entityType == null ) return;
		var b = entityType.Create<Entity>();
		b.Position = this.Position;
		b.Rotation = this.Rotation;
		Delete();
	}
}
