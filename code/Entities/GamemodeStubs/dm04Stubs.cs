class BaseDM04Stub : BaseGamemodeStub
{
}

[Library( "hl2_357" )]
class hl2_357 : BaseDM04Stub
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

[Library( "hl2_ar2" )]
class hl2_ar2 : BaseDM04Stub
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

[Library( "hl2_bugbait" )]
class hl2_bugbait : BaseDM04Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new SatchelWeapon();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "hl2_crossbow" )]
class hl2_crossbow : BaseDM04Stub
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

[Library( "hl2_crowbar" )]
class hl2_crowbar : BaseDM04Stub
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

[Library( "hl2_egon" )]
class hl2_egon : BaseDM04Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Egon();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "hl2_gauss" )]
class hl2_gauss : BaseDM04Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Gauss();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "hl2_gravgun" )]
class hl2_gravgun : BaseDM04Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Gauss();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "hl2_grenade" )]
class hl2_grenade : BaseDM04Stub
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

[Library( "hl2_rpg" )]
class hl2_rpg : BaseDM04Stub
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

[Library( "hl2_slam" )]
class hl2_slam : BaseDM04Stub
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

[Library( "hl2_smg1" )]
class hl2_smg1 : BaseDM04Stub
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

[Library( "hl2_spas12" )]
class hl2_spas12 : BaseDM04Stub
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

[Library( "hl2_stunstick" )]
class hl2_stunstick : BaseDM04Stub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new HornetGun();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}

[Library( "hl2_uspmatch" )]
class hl2_uspmatch : BaseDM04Stub
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
// ---------------- ITEMS ----------------
[Library( "dm08_healthkit" )]
class dm08_healthkit : BaseDM04Stub
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