public class FirstPersonCamera : CameraMode
{
    Vector3 lastPos;

    [ConVar.Client] public static float cl_rollspeed { get; set; } = 200.0f;
    [ConVar.Client] public static float cl_rollangle { get; set; } = 2.0f;

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

        Rotation = pawn.EyeRotation;

        Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + CalculateRoll( Rotation, pawn.Velocity, cl_rollangle, cl_rollspeed ) ).ToRotation();
        Rotation = Rotation.Angles().WithRoll( Rotation.Angles().roll + pawn.punchangle.z ).ToRotation();
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
}
