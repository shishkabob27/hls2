[Library( "weapon_gauss" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/gauss.vmdl" )]
[Title( "Gauss" ), Category( "Weapons" )]
partial class Gauss : HLWeapon
{
    //stub

    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/gauss.vmdl" );
    public override string ViewModelPath => "models/hl1/weapons/view/v_gauss.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 2;
    public override AmmoType AmmoType => AmmoType.Uranium;
    public override string AmmoIcon => "ui/ammo7.png";
    public override string InventoryIcon => "/ui/weapons/weapon_gauss.png";
    public override float ReloadTime => 0.1f;
    public override int ClipSize => -1;
    static Vector3 orangeCOLOUR = new Vector3( 255, 128, 0 );
    static Vector3 whiteCOLOUR = new Vector3( 255, 255, 255 );
    Nullable<Sound> spinSound;
    bool spinning = false;
    float spintime = 0.0f;
    float spintime2 = 0.0f;
    float startspin = 0.0f;

    float GetFullChargeTime()
    {

        if ( HLGame.hl_gamemode == "deathmatch" )
            return 1.5f;
        return 4;
    }
    float GetAmmoTickTime()
    {

        if ( HLGame.hl_gamemode == "deathmatch" )
            return 0.1f;
        return 0.3f;
    }
    public override void Spawn()
    {
        base.Spawn();
        Model = WorldModel;
        WeaponIsAmmoAmount = 15;
    }

    Particles Beam;
    public override void Simulate( Client owner )
    {
        if ( spinSound == null )
        {
            spinSound = Sound.FromEntity( "pulsemachine", this ).SetVolume( 0 );
        }
        else
        {
            float pitch = ( Time.Now - startspin ) * ( 150 / GetFullChargeTime() ) + 100;
            if ( pitch > 250 )
                pitch = 250;

            spinSound?.SetVolume( spinning ? 1 : 0 );
            spinSound?.SetPitch( pitch / 100 ); // ( spintime + 1.05f ) / 2 ).Clamp( 1.1f, 2.50f ) ); // old math, similar effect but doesnt play with multiplayers different timings as nice
        }
        base.Simulate( owner );
        if ( Owner is not HLPlayer player ) return;

        var owner2 = Owner as HLPlayer;

        if ( ( !( Input.Down( InputButton.SecondaryAttack ) ) && spinning ) || ( ( player.AmmoCount( AmmoType.Uranium ) <= 0 ) && spinning ) )
        {
            ViewModelEntity?.SetAnimParameter( "spinning", false );
            ShootEffects( whiteCOLOUR );
            var x = 85 + Rand.Float( 0, 31 );
            PlaySound( "gauss" ).SetPitch( HLUtils.CorrectPitch( x ) );
            var dmg = 200.0f;
            if ( Time.Now - startspin > GetFullChargeTime() )
            {
                dmg = 200;
            }
            else
            {
                dmg = 200 * ( ( Time.Now - startspin ) / GetFullChargeTime() );
            }
            ShootBullet( 0, 1, dmg, 2.0f );
            var ZVel = player.Velocity.z;
            var a = player.Velocity;

            a = player.Velocity - player.EyeRotation.Forward * dmg * 5;
            if ( HLGame.hl_gamemode != "deathmatch" )
            {
                // In singleplayer we do not get launched upwards
                a.z = ZVel;
            }
            player.Velocity = a;

            ViewModelEntity?.SetAnimParameter( "fire", true );
            spinning = false;
        }
    }
    public override void AttackPrimary()
    {
        TimeSincePrimaryAttack = 0;

        if ( Owner is not HLPlayer player ) return;

        var owner = Owner as HLPlayer;
        if ( owner.TakeAmmo( AmmoType, 2 ) == 0 )
        {
            return;
        }
        var x = 85 + Rand.Float( 0, 31 );
        PlaySound( "gauss" ).SetPitch( HLUtils.CorrectPitch( x ) );

        ShootEffects( orangeCOLOUR );
        ShootBullet( 0, 1, 20, 2.0f );

    }
    protected void ShootEffects( Vector3 beamcolour )
    {
        if ( Owner is not HLPlayer player ) return;

        var owner = Owner as HLPlayer;
        var startPos = GetFiringPos();
        var dir = GetFiringRotation().Forward;

        var tr = Trace.Ray( startPos, startPos + dir * 4096 )
        .UseHitboxes()
            .Ignore( owner, false )
            .WithAllTags( "solid" )
            .Run();

        if ( true )//Beam == null)
        {
            Beam = Particles.Create( "particles/generic_beam.vpcf", tr.EndPosition );
        }

        ViewModelEntity?.SetAnimParameter( "fire", true );
        VRWeaponModel?.SetAnimParameter( "fire", true );
        ViewModelEntity?.SetAnimParameter( "holdtype_attack", false ? 2 : 1 );
        VRWeaponModel?.SetAnimParameter( "holdtype_attack", false ? 2 : 1 );

        Beam.SetEntityAttachment( 0, EffectEntity, "muzzle", true );
        if ( Client.IsUsingVr ) Beam.SetEntityAttachment( 0, VRWeaponModel, "muzzle", true );
        Beam.SetPosition( 2, beamcolour );
        Beam.SetPosition( 3, new Vector3( 2, 1, 0 ) );

        //Beam.SetPosition(0, Position);
        //var pos = tr.StartPosition;
        //var a = GetAttachment("muzzle");
        //if (a != null)
        //pos = (a ?? default).Position;
        //Beam.SetPosition(0, pos);
        Beam.SetPosition( 1, tr.EndPosition );
        Beam.Destroy();

        Particles.Create( "particles/gauss_impact.vpcf", tr.EndPosition );
    }

    float tickammouse = 0;
    public override void AttackSecondary()
    {

        if ( !spinning )
        {
            tickammouse = Time.Now - 1;
            spintime = 0;
            spintime2 = Time.Now + GetFullChargeTime();
            startspin = Time.Now;
        }

        base.AttackSecondary();
        TimeSinceSecondaryAttack = 0;
        if ( Owner is not HLPlayer player ) return;

        var owner = Owner as HLPlayer;

        if ( Time.Now > tickammouse && Time.Now < spintime2 )
        {
            tickammouse = Time.Now + GetAmmoTickTime();
            if ( owner.TakeAmmo( AmmoType, 1 ) == 0 )
            {
                return;
            }
        }

        spinning = true;

        ViewModelEntity?.SetAnimParameter( "spinning", true );
        if ( Time.Now > spintime2 )
            return;

        spintime += 0.1f;
        //charge attack here!

    }
    public override void SimulateAnimator( PawnAnimator anim )
    {
        anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Gauss ); // TODO this is shit
        anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }
}