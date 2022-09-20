class BaseSQuakeStub : BaseGamemodeStub
{
}

// ---------------- WEAPONS ----------------
[Library( "dm_supershotgun" )]
class dm_supershotgun : BaseSQuakeStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Shotgun();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "sq_rocket" )]
class sq_rocket : BaseSQuakeStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new RPG();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "nailgun" )]
class nailgun : BaseSQuakeStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new SMG();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
// ---------------- ITEMS ----------------
[Library( "armor1" )]
class armor1 : BaseSQuakeStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Battery();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}