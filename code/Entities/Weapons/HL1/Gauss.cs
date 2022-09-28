[Library( "weapon_gauss" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/gauss.vmdl" )]
[Title( "Gauss" ), Category( "Weapons" )]
partial class Gauss : HLWeapon
{
    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/gauss.vmdl" );
    public override string ViewModelPath => "models/hl1/weapons/view/v_gauss.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 2;
    public override AmmoType AmmoType => AmmoType.Uranium;
    public override string AmmoIcon => "ui/ammo7.png";
    public override string InventoryIcon => "/ui/weapons/weapon_gauss.png";
    public override string InventoryIconSelected => "/ui/weapons/weapon_gauss_selected.png";
    public override float ReloadTime => 0.1f;
    public override int ClipSize => -1;
    static Vector3 orangeCOLOUR = new Vector3( 255, 128, 0 );
    static Vector3 whiteCOLOUR = new Vector3( 255, 255, 255 );
    Nullable<Sound> spinSound;
    bool spinning = false;
    float spintime = 0.0f;
    float spintime2 = 0.0f;
    float startspin = 0.0f;
    float PlayAftershock = 0;

    float GetFullChargeTime()
    {

        if ( HLGame.GameIsMultiplayer() )
            return 1.5f;
        return 4;
    }
    float GetAmmoTickTime()
    {

        if ( HLGame.GameIsMultiplayer() )
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
        if ( PlayAftershock != 0 && PlayAftershock < Time.Now )
        {
            PlaySound( "gauss_electro" );
            PlayAftershock = 0;
        }
        base.Simulate( owner );
        if ( Owner is not HLPlayer player ) return;

        var owner2 = Owner as HLPlayer;

        if ( ( !( Input.Down( InputButton.SecondaryAttack ) ) && spinning ) || ( ( player.AmmoCount( AmmoType.Uranium ) <= 0 ) && spinning ) || ( !EnableDrawing && spinning ) )
        {
            var dmg = 200.0f;
            if ( Time.Now - startspin > GetFullChargeTime() )
            {
                dmg = 200;
            }
            else
            {
                dmg = 200 * ( ( Time.Now - startspin ) / GetFullChargeTime() );
            }
            if ( dmg < 10 ) return; // wait until we have a bit of charge
            ViewModelEntity?.SetAnimParameter( "spinning", false );
            var x = 85 + Rand.Float( 0, 31 );
            PlaySound( "gauss" ).SetPitch( HLUtils.CorrectPitch( x ) );

            GaussLaser( whiteCOLOUR, dmg, player.EyeRotation.Forward, GetFiringPos(), true );
            Rand.SetSeed( Time.Tick ); // same random seed across client and server
            PlayAftershock = Time.Now + Rand.Float( 0.3f, 0.8f );
            var ZVel = player.Velocity.z;
            var a = player.Velocity;

            a = player.Velocity - player.EyeRotation.Forward * dmg * 5;
            if ( !HLGame.GameIsMultiplayer() )
            {
                // In singleplayer we do not get launched upwards
                a.z = ZVel;
            }
            player.Velocity = a;

            ViewModelEntity?.SetAnimParameter( "fire", true );
            spinning = false;
        }
    }
    public override void ActiveEnd( Entity ent, bool dropped )
    {
        spinning = false;
        spinSound?.SetVolume( spinning ? 1 : 0 );
        base.ActiveEnd( ent, dropped );
    }

    public override void AttackPrimary()
    {
        if ( spinning ) return;

        TimeSincePrimaryAttack = 0;

        if ( Owner is not HLPlayer player ) return;

        var owner = Owner as HLPlayer;
        if ( owner.TakeAmmo( AmmoType, 2 ) == 0 )
        {
            return;
        }
        var x = 85 + Rand.Float( 0, 31 );
        PlaySound( "gauss" ).SetPitch( HLUtils.CorrectPitch( x ) );
        GaussLaser( orangeCOLOUR, 20, player.EyeRotation.Forward, GetFiringPos() );

        Rand.SetSeed( Time.Tick ); // same random seed across client and server
        PlayAftershock = Time.Now + Rand.Float( 0.3f, 0.8f );
        player.punchanglecl.x = -2;

    }


    [ConVar.Client] public static bool hl_debug_gauss { get; set; } = false;
    [ConVar.Replicated] public static bool hl_gauss_experimental_punch { get; set; } = false;

    public void GaussLaser( Vector3 Colour, float Damage, Vector3 vecOrigDir, Vector3 vecOrigSrc, bool doPunch = false )
    {
        Vector3 vecSrc = vecOrigSrc;
        Vector3 vecDir = vecOrigDir;
        Vector3 vecDest = vecSrc + vecDir * 8192;
        float DMG = Damage;
        float flMaxFrac = 1.0f;
        int nTotal = 0;
        int MaxHits = 10; // how many times do we trace and bounce?
        float force = 1;
        bool FirstBeam = true;
        while ( DMG > 10 && MaxHits > 0 )
        {
            MaxHits--;


            ShootEffects( Colour, vecSrc, vecDir, vecDest, FirstBeam );
            //ShootBullet( 0, 1, DMG, 2.0f );

            if ( FirstBeam )
            {
                // TODO: muzzleflash here 
                FirstBeam = false;
                nTotal += 26;
            }

            foreach ( var tr in TraceBullet( vecSrc, vecDest, 2.0f ) )
            {

                if ( IsServer )
                {
                    tr.Surface.DoHLBulletImpact( tr );

                    var damageInfo = DamageInfo.FromBullet( vecSrc, vecDest * force, DMG )
                        .UsingTraceResult( tr )
                        .WithAttacker( Owner )
                        .WithWeapon( this );

                    tr.Entity.TakeDamage( damageInfo );
                    if ( tr.Entity is NPC )
                    {
                        var trace = Trace.Ray( vecSrc, vecDest )
                        .WorldOnly()
                        .Ignore( this )
                        .Size( 1.0f )
                        .Run();
                        if ( ResourceLibrary.TryGet<DecalDefinition>( "decals/red_blood.decal", out var decal ) )
                        {
                            Decal.Place( decal, trace );
                        }
                    }
                }

                if ( hl_debug_gauss ) DebugOverlay.Line( tr.StartPosition, tr.EndPosition, 10 );

                if ( tr.Entity is WorldEntity )
                {
                    var n = -Vector3.Dot( tr.Normal, vecDir );
                    if ( n < 0.5 ) // 60 degrees
                    {
                        Vector3 r;

                        r = 2.0f * tr.Normal * n + vecDir;
                        flMaxFrac = flMaxFrac - tr.Fraction;
                        vecDir = r;
                        vecSrc = tr.EndPosition + vecDir * 8;
                        vecDest = vecSrc + vecDir * 8192;

                        // explode a bit
                        //m_pPlayer->RadiusDamage( tr.vecEndPos, pev, m_pPlayer->pev, flDamage * n, CLASS_NONE, DMG_BLAST );

                        nTotal += 34;

                        // lose energy
                        if ( n == 0 ) n = 0.1f;
                        DMG = DMG * ( 1 - n );
                    }
                    else
                    {
                        nTotal += 13;
                        if ( doPunch && hl_gauss_experimental_punch )
                        {

                            var trace = Trace.Ray( tr.EndPosition + vecDir * 8, vecDest ).Run();
                            if ( !trace.StartedSolid )
                            {
                                var trace2 = Trace.Ray( trace.EndPosition, tr.EndPosition ).Run();
                                float n2 = ( trace2.EndPosition - trace2.EndPosition ).Length;

                                if ( n2 < DMG )
                                {
                                    if ( n2 == 0 ) n2 = 1;
                                    DMG -= n2;

                                    // ALERT( at_console, "punch %f\n", n );
                                    nTotal += 21;

                                    // exit blast damage
                                    //m_pPlayer->RadiusDamage( beam_tr.vecEndPos + vecDir * 8, pev, m_pPlayer->pev, flDamage, CLASS_NONE, DMG_BLAST );
                                    float damage_radius;


                                    if ( HLGame.GameIsMultiplayer() )
                                    {
                                        damage_radius = DMG * 1.75f;  // Old code == 2.5
                                    }
                                    else
                                    {
                                        damage_radius = DMG * 2.5f;
                                    }
                                    //HLExplosion.Explosion( this, Owner, trace2.EndPosition + vecDir * 8, damage_radius, DMG, 1, "electro" );

                                    //:RadiusDamage( beam_tr.vecEndPos + vecDir * 8, pev, m_pPlayer->pev, flDamage, damage_radius, CLASS_NONE, DMG_BLAST );

                                    //CSoundEnt::InsertSound( bits_SOUND_COMBAT, pev->origin, NORMAL_EXPLOSION_VOLUME, 3.0 );

                                    nTotal += 53;

                                    vecSrc = trace2.EndPosition + vecDir;
                                }
                            }
                        }
                        else
                        {

                            DMG = 0;
                        }
                    }
                }
                else
                {
                    vecSrc = tr.EndPosition + vecDir;
                }
            }
        }
    }
    protected void ShootEffects( Vector3 beamcolour, Vector3 vecSrc, Vector3 vecDir, Vector3 vecDest, bool FirstBeam = true )
    {
        if ( Owner is not HLPlayer player ) return;

        var owner = Owner as HLPlayer;
        var startPos = vecSrc;//GetFiringPos();
        var dir = vecDir;//GetFiringRotation().Forward;

        var tr = Trace.Ray( startPos, vecDest )
        .UseHitboxes()
            .Ignore( owner, false )
            .WithAllTags( "solid" )
            .Run();

        if ( true )//Beam == null)
        {
            Beam = Particles.Create( "particles/generic_beam.vpcf", vecSrc );
        }

        ViewModelEntity?.SetAnimParameter( "fire", true );
        VRWeaponModel?.SetAnimParameter( "fire", true );
        ViewModelEntity?.SetAnimParameter( "holdtype_attack", false ? 2 : 1 );
        VRWeaponModel?.SetAnimParameter( "holdtype_attack", false ? 2 : 1 );


        if ( FirstBeam )
        {
            Beam.SetEntityAttachment( 0, EffectEntity, "muzzle", true );
        }
        else
        {
            Beam.SetPosition( 0, startPos );
        }
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