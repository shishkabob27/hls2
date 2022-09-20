class BaseTF2Stub : BaseGamemodeStub
{
}

// ---------------- ITEMS ----------------
[Library( "tf_healthkit_full" )]
class tf_healthkit_full : BaseTF2Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new HealthKit();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "tf_healthkit_medium" )]
class tf_healthkit_medium : BaseTF2Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new HealthKit();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "tf_healthkit_small" )]
class tf_healthkit_small : BaseTF2Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new HealthKit();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
// ---------------- ITEMS ----------------
[Library( "tf_player_teamspawn" )]
class tf_player_teamspawn : BaseTF2Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new SpawnPoint();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}