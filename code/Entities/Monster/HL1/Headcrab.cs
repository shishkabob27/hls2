[Library( "monster_headcrab" ), HammerEntity]
[EditorModel( "models/hl1/monster/headcrab.vmdl" )]
[Title( "Headcrab" ), Category( "Monsters" ), Icon( "person" )]
public class Headcrab : NPC
{
    float NextAttack;
    float NextIdleSound;
    float NextAlertSound;
    Entity Enemy;
    public override void Spawn()
    {
        base.Spawn();
        Health = 20;
        NPCAnimGraph = "animgraphs/hl1/monster/headcrab.vanmgrph";
        SetAnimGraph( NPCAnimGraph );
        SetModel( "models/hl1/monster/headcrab.vmdl" );
        SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -12, -12, 0 ), new Vector3( 12, 12, 24 ) );
        NPCSurface = "flesh_yellow";
        BloodColour = BLOOD_COLOUR_YELLOW;
        entFOV = 0.5f;
        EnableHitboxes = true;
        Tags.Add( "npc", "playerclip" );

    }
    public override int Classify()
    {
        return (int)HLCombat.Class.CLASS_ALIEN_PREY;
    }
    public override void ProcessEntity( Entity ent, int rel )
    {
        if ( rel > 0 )
        {
            if ( ent != Enemy && Time.Now > NextAlertSound )
            {
                NextAlertSound = Time.Now + Rand.Int( 2, 4 );
                PlaySound( "hc_alert" );
            }
            Enemy = ent;
            targetRotation = Rotation.LookAt( ent.Position.WithZ( 0 ) - Position.WithZ( 0 ), Vector3.Up );

            var e = ( ent.Position - Position );
            var vec2LOS = new Vector2( e.x, e.y );
            vec2LOS = vec2LOS.Normal;
            var flDist = ( ent.Position - Position ).Length;
            var flDot = (float)Vector2.Dot( vec2LOS, new Vector2( Rotation.Forward.x, Rotation.Forward.y ) );

            if ( CheckRangeAttack1( flDot, flDist ) && Time.Now > NextAttack )//&& Position.AlmostEqual( Steer.Target ) )
            {
                jumpAttack();
            }
            else
            {
                Steer.Target = ent.Position - ( ( ent.Position - Position ).Normal * 256 ); // don't get too close!
            }
        }
    }
    public override void Think()
    {
        if ( Time.Now > NextIdleSound )
        {
            NextIdleSound = Time.Now + Rand.Int( 2, 4 );
            PlaySound( "hc_idle" );
        }
        base.Think();
    }
    void jumpAttack()
    {
        animHelper.Attack = true;
        //ClearBits( pev->flags, FL_ONGROUND );

        //UTIL_SetOrigin( pev, pev->origin + Vector( 0, 0, 1 ) );// take him off ground so engine doesn't instantly reset onground 
        //UTIL_MakeVectors( pev->angles );

        Vector3 vecJumpDir;
        if ( Enemy != null )
        {
            float gravity = 800;
            if ( gravity <= 1 )
                gravity = 1;

            // How fast does the headcrab need to travel to reach that height given gravity?
            float height = ( Enemy.EyePosition.z - Position.z );
            if ( height < 16 )
                height = 16;
            float speed2 = (float)Math.Sqrt( 2 * gravity * height );
            float time = speed2 / gravity;

            // Scale the sideways velocity to get there at the right time
            vecJumpDir = ( Enemy.EyePosition - Position );
            vecJumpDir = vecJumpDir * ( 1.0f / time );

            // Speed to offset gravity at the desired height
            vecJumpDir.z = speed2;

            // Don't jump too far/fast
            float distance = vecJumpDir.Length;

            if ( distance > 650 )
            {
                vecJumpDir = vecJumpDir * ( 650.0f / distance );
            }
        }
        else
        {
            // jump hop, don't care where
            vecJumpDir = new Vector3( Rotation.Forward.x, Rotation.Forward.y, Rotation.Up.z ) * 350;
        }

        //int iSound = RANDOM_LONG( 0, 2 );
        //if ( iSound != 0 )
        //    EMIT_SOUND_DYN( edict(), CHAN_VOICE, pAttackSounds[iSound], GetSoundVolue(), ATTN_IDLE, 0, GetVoicePitch() );

        PlaySound( "hc_attack" );
        Velocity = vecJumpDir;
        NextAttack = Time.Now + 2;
    }
    bool CheckRangeAttack1( float flDot, float flDist )
    {
        if ( ( GroundEntity != null ) && flDist <= 256 && flDot >= 0.65 )
        {
            return true;
        }
        return false;
    }
    public override void TakeDamage( DamageInfo info )
    {

        PlaySound( "hc_pain" );
        TakeDamage( info );
    }
    public override void OnKilled()
    {

        PlaySound( "hc_death" );
        base.OnKilled();
    }
}