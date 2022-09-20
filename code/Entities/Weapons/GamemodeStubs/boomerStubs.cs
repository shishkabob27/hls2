// these maps would be fun as fuck for flying around with a gauss in deathmatch mode so i just had to get them in here.
class BaseBoomerStub : BaseGamemodeStub
{
}
// ---------------- WEAPONS ----------------
[Library( "boomer_railgun" )]
class boomer_railgun : BaseBoomerStub
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

[Library( "boomer_nailgun" )]
class boomer_nailgun : BaseBoomerStub
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
[Library( "boomer_lightninggun" )]
class boomer_lightninggun : BaseBoomerStub
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

[Library( "boomer_crowbar" )]
class boomer_crowbar : BaseBoomerStub
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

[Library( "boomer_shotgun" )]
class boomer_shotgun : BaseBoomerStub
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
[Library( "boomer_grenadelauncher" )]
class boomer_grenadelauncher : BaseBoomerStub
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
[Library( "boomer_rocketlauncher" )]
class boomer_rocketlauncher : BaseBoomerStub
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
// ---------------- ITEMS ----------------
[Library( "boomer_healthkit" )]
class boomer_healthkit : BaseBoomerStub
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

[Library( "boomer_healthvial" )]
class boomer_healthvial : BaseBoomerStub
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

[Library( "boomer_megahealth" )]
class boomer_megahealth : BaseBoomerStub
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

[Library( "boomer_armour" )]
class boomer_armour : BaseBoomerStub
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

[Library( "boomer_armourshard" )]
class boomer_armourshard : BaseBoomerStub
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

[Library( "boomer_megaarmour" )]
class boomer_megaarmour : BaseBoomerStub
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

// ---------------- FUNC ----------------

[Library( "boomer_teleport" )]
class boomer_teleport : TeleportVolumeEntity
{
}


// ---------------- AMMO ----------------
[Library( "boomer_nails" )]
class boomer_nails : BaseBoomerStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        //var a = new Ammo9mmClip();
        var a = new SMG();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "boomer_ammobuckshot" )]
class boomer_ammobuckshot : BaseBoomerStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        //var a = new AmmoBuckshot();
        var a = new Shotgun();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "boomer_lightning" )]
class boomer_lightning : BaseBoomerStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        //var a = new AmmoUranium();
        var a = new Egon();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "boomer_rails" )]
class boomer_rails : BaseBoomerStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        //var a = new AmmoUranium();
        var a = new Gauss();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "boomer_rockets" )]
class boomer_rockets : BaseBoomerStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        //var a = new AmmoRPG();
        var a = new RPG();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "boomer_grenades" )]
class boomer_grenades : BaseBoomerStub
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