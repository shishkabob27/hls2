class BaseTTTStub : BaseGamemodeStub
{
}
// ---------------- WEAPONS ----------------
[Library( "ttt_weapon_mp5" )]
class ttt_weapon_mp5 : BaseTTTStub
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
[Library( "ttt_weapon_mp7" )]
class ttt_weapon_mp7 : BaseTTTStub
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
[Library( "ttt_weapon_ak47" )]
class ttt_weapon_ak47 : BaseTTTStub
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
[Library( "ttt_weapon_huge" )]
class ttt_weapon_huge : BaseTTTStub
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
[Library( "ttt_weapon_m4" )]
class ttt_weapon_m4 : BaseTTTStub
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
[Library( "ttt_weapon_p90" )]
class ttt_weapon_p90 : BaseTTTStub
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
[Library( "ttt_weapon_revolver" )]
class ttt_weapon_revolver : BaseTTTStub
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
[Library( "ttt_weapon_m9" )]
class ttt_weapon_m9 : BaseTTTStub
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
[Library( "ttt_weapon_silencedpistol" )]
class ttt_weapon_silencedpistol : BaseTTTStub
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
[Library( "ttt_weapon_p250" )]
class ttt_weapon_p250 : BaseTTTStub
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
[Library( "ttt_weapon_scout" )]
class ttt_weapon_scout : BaseTTTStub
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

[Library( "ttt_weapon_knife" )]
class ttt_weapon_knife : BaseTTTStub
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

[Library( "ttt_weapon_bekas" )]
class ttt_weapon_bekas : BaseTTTStub
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
[Library( "ttt_grenade_discombobulator" )]
class ttt_grenade_discombobulator : BaseTTTStub
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
[Library( "ttt_grenade_smoke" )]
class ttt_grenade_smoke : BaseTTTStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Tripmine();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "ttt_weapon_random" )]
class ttt_weapon_random : BaseTTTStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        SpawnRandomWeapon();
        this.Delete();
    }
}
[Library( "ttt_grenade_random" )]
class ttt_grenade_random : BaseTTTStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() )
        {
            Delete();
            return;
        }
        SpawnRandomGrenade();
        this.Delete();
    }
}
// ---------------- ITEMS ----------------
[Library( "ttt_equipment_healthstation" )]
class ttt_equipment_healthstation : func_healthcharger
{
}


// ---------------- AMMO ----------------
[Library( "ttt_ammo_smg" )]
class ttt_ammo_smg : BaseTTTStub
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
[Library( "ttt_ammo_rifle" )]
class ttt_ammo_rifle : BaseTTTStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        var a = new Ammo9mmAR(); //new Ammo9mmBox();
        a.Position = Position;
        a.Tags.Add( "stubmade" );
        this.Delete();
    }
}
[Library( "ttt_ammo_shotgun" )]
class ttt_ammo_shotgun : BaseTTTStub
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
[Library( "ttt_ammo_magnum" )]
class ttt_ammo_magnum : BaseTTTStub
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
[Library( "ttt_ammo_sniper" )]
class ttt_ammo_sniper : BaseTTTStub
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
[Library( "ttt_ammo_random" )]
class ttt_ammo_random : BaseTTTStub
{
    public override void Spawn()
    {
        if ( SpawnCheck() ) { Delete(); return; }
        SpawnRandomAmmo();
        this.Delete();
    }
}