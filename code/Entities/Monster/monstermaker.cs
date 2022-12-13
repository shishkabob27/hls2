[Library("monstermaker"), HammerEntity]
[EditorSprite( "editor/npc_maker.vmat" )]
[Title("monstermaker"), Category("Monsters"), Icon("person")]
public class MonsterMaker : Entity
{
    //stub

    [Property]
	public string monstertype { get; set; }
    

    [Input]
	new public void Spawn()
	{
        var entityType = TypeLibrary.GetType<Entity>( monstertype ).GetType();
        if ( entityType == null )

            if ( !TypeLibrary.HasAttribute<SpawnableAttribute>( entityType ) )
                return;

        var ent = TypeLibrary.Create<Entity>( entityType );

        ent.Position = Position;
	}
}
