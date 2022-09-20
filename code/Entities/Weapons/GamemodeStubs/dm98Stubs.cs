class BaseDM98Stub : BaseGamemodeStub
{
}


// ---------------- WEAPONS ----------------
[Library( "dm_tripmine" )]
class dm_tripmine : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new TripmineWeapon();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "dm_smg" )]
class dm_smg : BaseDM98Stub
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

[Library( "dm_357" )]
class dm_357 : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Python();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "dm_pistol" )]
class dm_pistol : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Pistol();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "dm_crossbow" )]
class dm_crossbow : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Crossbow();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "dm_crowbar" )]
class dm_crowbar : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Crowbar();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "dm_shotgun" )]
class dm_shotgun : BaseDM98Stub
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
[Library( "dm_grenade" )]
class dm_grenade : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new GrenadeWeapon();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
// ---------------- ITEMS ----------------
[Library( "dm_healthkit" )]
class dm_healthkit : BaseDM98Stub
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

[Library( "dm_battery" )]
class dm_battery : BaseDM98Stub
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

[Library( "dm_chargerstation" )]
class dm_chargerstation : func_healthcharger
{
}


// ---------------- AMMO ----------------
[Library( "dm_ammo9mmclip" )]
class dm_ammo9mmclip : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Ammo9mmClip();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "dm_ammo9mmbox" )]
class dm_ammo9mmbox : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Ammo9mmAR();//new Ammo9mmBox();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "dm_ammobuckshot" )]
class dm_ammobuckshot : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new AmmoBuckshot();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "dm_ammo357" )]
class dm_ammo357 : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Ammo357();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "dm_ammocrossbow" )]
class dm_ammocrossbow : BaseDM98Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new AmmoCrossbow();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}