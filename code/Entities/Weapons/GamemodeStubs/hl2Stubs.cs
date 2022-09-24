class BaseHL2Stub : BaseGamemodeStub
{
}

[Library( "hl2_357" )]
class hl2_357 : BaseHL2Stub
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
class hl2_ar2 : BaseHL2Stub
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
class hl2_bugbait : BaseHL2Stub
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
class hl2_crossbow : BaseHL2Stub
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
class hl2_crowbar : BaseHL2Stub
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
class hl2_egon : BaseHL2Stub
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
class hl2_gauss : BaseHL2Stub
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
class hl2_gravgun : BaseHL2Stub
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
class hl2_grenade : BaseHL2Stub
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
class hl2_rpg : BaseHL2Stub
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
class hl2_slam : BaseHL2Stub
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
class hl2_smg1 : BaseHL2Stub
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
class hl2_spas12 : BaseHL2Stub
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
class hl2_stunstick : BaseHL2Stub
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
