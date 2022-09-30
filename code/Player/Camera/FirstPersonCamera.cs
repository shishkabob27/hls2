﻿public class FirstPersonCamera : CameraMode
{
    Vector3 lastPos;

    [ConVar.Client] public static float cl_rollspeed { get; set; } = 200.0f;
    [ConVar.Client] public static float cl_rollangle { get; set; } = 2.0f;
    [ConVar.Client] public static float cl_bob { get; set; } = 0.01f;
    [ConVar.Client] public static float cl_bobcycle { get; set; } = 0.8f;
    [ConVar.Client] public static float cl_bobup { get; set; } = 0.5f;
    [ConVar.Client] public static bool hl_won_viewbob { get; set; } = false;

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


        if ( pawn.ActiveChild is HLWeapon )
        {

            var wep = pawn.ActiveChild as HLWeapon;
            // Weapon position
            if ( wep.ViewModelEntity is Entity )
            {

                wep.ViewModelEntity.Position = Position;
                wep.ViewModelEntity.Rotation = Rotation;
            }
            // Weapon bobbing
            if ( wep.ViewModelEntity is HLViewModel )
            {
                var viewmodel = wep.ViewModelEntity as HLViewModel;
                var b = viewmodel.Position;
                for ( var i = 0; i < 3; i++ )
                {
                    b[i] += bob * 0.4f * Rotation.Forward[i];
                }

                //b.z += bob; // I don't understand, this is in the code but hl1 doesnt have this? is it broken in the original? i'll just comment it out...

                // pushing the view origin down off of the same X/Z plane as the ent's origin will give the
                // gun a very nice 'shifting' effect when the player looks up/down. If there is a problem
                // with view model distortion, this may be a cause. (SJB). 
                b.z -= 1;

                viewmodel.Position = b;

                var WepRot = viewmodel.Rotation;
                WepRot = WepRot.Angles().WithYaw( WepRot.Angles().yaw - bob * 0.5f ).ToRotation();
                WepRot = WepRot.Angles().WithRoll( WepRot.Angles().roll - bob * 1f ).ToRotation();
                WepRot = WepRot.Angles().WithPitch( WepRot.Angles().pitch - bob * 0.3f ).ToRotation();
                if ( hl_won_viewbob )
                    viewmodel.Rotation = WepRot;
                //wep.angle[YAW] -= bob * 0.5;
                //wep.angle[ROLL] -= bob * 1;
                //wep.angle[PITCH] -= bob * 0.3;

            }
        }


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
