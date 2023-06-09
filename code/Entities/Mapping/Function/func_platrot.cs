[Library( "func_platrot" )]
[HammerEntity]
[Title("func_platrot"), Category("Brush Entities"), Icon("volume_up")]
public partial class func_platrot : KeyframeEntity
{
	[Flags]
	public enum Flags
	{
		Toggle = 1,
		XAxis = 64,
		YAxis = 128, 
	}

	bool IsUp = false;

	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags SpawnSettings { get; set; } = 0;

	[Property( "rotation" )]
	public float rotation { get; set; } = 1; 
	[Property( "speed" )]
	public float speed { get; set; } = 100; 
	[Property( "height" )]
	public float height { get; set; } = 100;
	// stub

	Angles prevAngles;
	Vector3 prevPos;
	[GameEvent.Tick.Server]
	void Tick()
	{
		AngularVelocity = prevAngles - Rotation.Angles();
		Velocity = prevPos - Position;
		prevAngles = Rotation.Angles();
		prevPos = Position;
	}

	[Input]
	public void GoUp()
	{
		var a = new Transform( Position + new Vector3( 0, 0, height ), Rotation * Rotation.From(0,rotation,0) );
		IsUp = true;
		KeyframeTo( a , 1 / (speed / rotation) );
	}
	[Input]
	public void GoDown()
	{
		var b = new Transform( Position - new Vector3( 0, 0, height ), Rotation * Rotation.From( 0, -rotation, 0 ) );
		IsUp = false;
		KeyframeTo( b, 1 / (speed / rotation) );
	}
	[Input]
	public void Toggle()
	{
		if ( IsUp == true )
		{
			GoDown();
		} 
		else
		{
			GoUp();
		} 
	}
	public override void Spawn()
	{
		Position = Position - new Vector3( 0, 0, height );
		IsUp = false;
		base.Spawn(); 
	}
}
