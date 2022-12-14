public partial class HLPlayer
{
	void AddRoll()
	{
		if ( Game.LocalPawn is not HLPlayer pawn ) return;
		Camera.Rotation = Rotation.Angles().WithRoll( Camera.Rotation.Angles().roll + CalculateRoll( Camera.Rotation, pawn.Velocity, cl_rollangle, cl_rollspeed ) ).ToRotation();
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
