public struct QAngle
{
	public float Pitch, Yaw, Roll;

	public float x { get => Pitch; set { Pitch = value; } }
	public float y { get => Yaw; set { Yaw = value; } }
	public float z { get => Roll; set { Roll = value; } }

	public Vector3 Forward => ((Rotation)this).Forward;
	public Vector3 Right => ((Rotation)this).Right;
	public Vector3 Up => ((Rotation)this).Up;

	public QAngle() : this(0, 0, 0) { }
	public QAngle(float x, float y, float z)
	{
		Pitch = x;
		Yaw = y;
		Roll = z;
	}

	public void AngleVectors(out Vector3 forward, out Vector3 right, out Vector3 up)
	{
		float sr, sp, sy, cr, cp, cy;

		var pitchRad = Pitch.DegreeToRadian();
		var yawRad = Yaw.DegreeToRadian();
		var rollRad = Roll.DegreeToRadian();

		sp = MathF.Sin(pitchRad);
		cp = MathF.Cos(pitchRad);

		sy = MathF.Sin(yawRad);
		cy = MathF.Cos(yawRad);

		sr = MathF.Sin(rollRad);
		cr = MathF.Cos(rollRad);

		forward = new(cp * cy,
						cp * sy,
						-sp);

		right = new(-1 * sr * sp * cy + -1 * cr * -sy,
						-1 * sr * sp * sy + -1 * cr * cy,
						-1 * sr * cp);

		up = new(cr * sp * cy + -sr * -sy,
					cr * sp * sy + -sr * cy,
					cr * cp);
	}
	public static QAngle operator +(QAngle c1, QAngle c2)
	{
		return new QAngle(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
	}
	public static QAngle operator -(QAngle c1, QAngle c2)
	{
		return new QAngle(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
	}
	public static QAngle operator -(QAngle value)
	{
		return new QAngle(0f - value.x, 0f - value.y, 0f - value.z);
	}

	public static implicit operator QAngle(Vector3 value)
	{
		return new(value.x, value.y, 0f);
	}

	public static implicit operator QAngle(Rotation value)
	{
		return new(value.Pitch(), value.Yaw(), value.Roll());
	}

	public static implicit operator Rotation(QAngle value)
	{
		return Rotation.From(value.Pitch, value.Yaw, value.Roll);
	}
}