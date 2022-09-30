public class FirstPersonCamera : CameraMode
{
    Vector3 lastPos;

    [ConVar.Client] public static float cl_rollspeed { get; set; } = 200.0f;
    [ConVar.Client] public static float cl_rollangle { get; set; } = 2.0f;
    [ConVar.Client] public static float cl_bob { get; set; } = 0.01f;
    [ConVar.Client] public static float cl_bobcycle { get; set; } = 0.8f;
    [ConVar.Client] public static float cl_bobup { get; set; } = 0.5f;

    public override void Activated()
    {
        var pawn = Local.Pawn;
        if ( pawn == null ) return;

        Position = pawn.EyePosition;
        Rotation = pawn.EyeRotation;

        lastPos = Position;
    }

    public override void Update()
    {
        var pawn = Local.Pawn as HLPlayer;
        if ( pawn == null ) return;
        Viewer = pawn;
        if ( pawn.Client.IsUsingVr ) return;

        var eyePos = pawn.EyePosition;

        Position = eyePos;
        var bob = V_CalcBob();
        var a = Position;
        a.z += bob;
        Position = a;

        Rotation = pawn.EyeRotation;

        Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + CalculateRoll( Rotation, pawn.Velocity, cl_rollangle, cl_rollspeed ) ).ToRotation();
        Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + pawn.punchangle.z ).ToRotation();
        Rotation = Rotation.Angles().WithPitch( Rotation.Angles().pitch + pawn.punchangle.x ).ToRotation();
        Rotation = Rotation.Angles().WithYaw( Rotation.Angles().yaw + pawn.punchangle.y ).ToRotation();


        Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + pawn.punchanglecl.z ).ToRotation();
        Rotation = Rotation.Angles().WithPitch( Rotation.Angles().pitch + pawn.punchanglecl.x ).ToRotation();
        Rotation = Rotation.Angles().WithYaw( Rotation.Angles().yaw + pawn.punchanglecl.y ).ToRotation();


        pawn.punchanglecl = pawn.punchanglecl.Approach( 0, Time.Delta * 14.3f ); // was Delta * 10, 14.3 matches hl1 the most
        //Log.Info( pawn.punchangle );

        lastPos = Position;
    }
    public virtual float CalculateRoll( Rotation angles, Vector3 velocity, float rollangle, float rollspeed )
    {
        if ( !HLGame.hl_viewroll ) return 0.0f;
        float sign;
        float side;
        float value;
        //QAngle a = angles; //.AngleVectors(out var forward, out var right, out var up);
        //a.AngleVectors(out var forward, out var right, out var up);
        var forward = angles.Forward;
        var right = angles.Right;
        var up = angles.Up;

        side = velocity.Dot( right );

        sign = side < 0 ? -1 : 1;

        side = Math.Abs( side );

        value = rollangle;

        if ( side < rollspeed )
        {
            side = side * value / rollspeed;
        }
        else
        {
            side = value;
        }

        return side * sign;
    }
    double bobtime;
    float bob;
    float cycle;
    float lasttime;
    float V_CalcBob()
    {
        Vector3 vel;
        if ( Local.Pawn is not HLPlayer player ) return 0;

        if ( player.GroundEntity == null || Time.Now == lasttime )
        {
            // just use old value
            return bob;
        }

        lasttime = Time.Now;

        bobtime += Time.Delta;
        cycle = (float)( bobtime - (int)( bobtime / cl_bobcycle ) * cl_bobcycle );
        cycle /= cl_bobcycle;

        if ( cycle < cl_bobup )
        {
            cycle = (float)Math.PI * cycle / cl_bobup;
        }
        else
        {
            cycle = (float)Math.PI + (float)Math.PI * ( cycle - cl_bobup ) / ( 1.0f - cl_bobup );
        }

        // bob is proportional to simulated velocity in the xy plane
        // (don't count Z, or jumping messes it up)
        //VectorCopy( pparams->simvel, vel );
        vel = player.Velocity.WithZ( 0 );
        //vel[2] = 0;

        bob = (float)Math.Sqrt( vel[0] * vel[0] + vel[1] * vel[1] ) * cl_bob;
        bob = bob * 0.3f + bob * 0.7f * (float)Math.Sin( cycle );
        bob = Math.Min( bob, 4 );
        bob = Math.Max( bob, -7 );
        return bob;

    }

}
