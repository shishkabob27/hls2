[Library("monstermaker"), HammerEntity]
[EditorSprite( "editor/npc_maker.vmat" )]
[Title("monstermaker"), Category("Monsters"), Icon("person")]
public class monstermaker : Entity
{
    //stub

    [Property]
	public string monstertype { get; set; }
    

    [Input]
	new public void Spawn()
	{
        var entityType = TypeLibrary.GetTypeByName<Entity>( monstertype );
        if ( entityType == null )

            if ( !TypeLibrary.Has<SpawnableAttribute>( entityType ) )
                return;

        var ent = TypeLibrary.Create<Entity>( entityType );

        ent.Position = Position;
	}
}
