[Library( "func_tracktrain" )]
[HammerEntity]
[Title( "func_tracktrain" ), Category( "Brush Entities" ), Icon( "directions_railway" )]
public partial class func_tracktrain : BrushEntity
{
	Vector3 PrevPos;
	[Property( "target" ), FGDType( "target_destination" )]
	public EntityTarget Target { get; set; } //= "";
	public float speed = 0;

	path_track prevcheck;
	bool prevcheckfailed = false;
	path_track TargetEnt;

	[Property( "orientationtype" )]
	public float OrientationType { get; set; } = 1;
	public override void Spawn()
	{
		Tags.Clear();
		base.Spawn();
	}
	path_track a;
	[Event.Tick.Server]
	public void tick()
	{
		if ( speed == 0 ) return;
		try
		{
			// this was so we don't cnstantly check unless we know anything as changed.
			if (a == null)// && !(a == prevcheck && prevcheckfailed))
			{ 
				a = Target.GetTarget() as path_track;

				/*prevcheck = Target;
				var b = Entity.FindAllByName( Target );
				if (b.Count() > 0) TargetEnt = (b.First() as path_track);
				
				if ( a == null)
				{
					prevcheckfailed = true;
				}
				else
				{
					prevcheckfailed = false;
				}*/
			}
			if ( speed != 0 )
			{

				Velocity = (((Position - a.Position).Normal * speed)) * -1;
				Position += Velocity * Time.Delta;
				if ( OrientationType != 0 ) Rotation = Rotation.Lerp( Rotation, Rotation.LookAt( Position.WithZ( 0 ) - a.Position.WithZ( 0 ), Vector3.Up ), Time.Delta * 2.4f );
				foreach ( var child in Children )
				{
					child.Velocity = Velocity;
					//child.Position += child.Velocity;
					//if ( OrientationType != 0 ) child.Rotation = Rotation.Lerp( child.Rotation, Rotation.LookAt( child.Position.WithZ( 0 ) - a.Position.WithZ( 0 ), Vector3.Up ), Time.Delta * 5 );
				}
			}
			if ( Position.AlmostEqual( a.Position, 16 ) )
			{

				a.OnPass.Fire( this );
				Velocity = Vector3.Zero;
				foreach ( var child in Children )
				{
					child.Velocity = Vector3.Zero;
					//child.Position += child.Velocity;
					//if ( OrientationType != 0 ) child.Rotation = Rotation.Lerp( child.Rotation, Rotation.LookAt( child.Position.WithZ( 0 ) - a.Position.WithZ( 0 ), Vector3.Up ), Time.Delta * 5 );
				}
				Target = a.Target;
				var b = (Target.GetTarget() as path_track);
				a = b;
				TargetEnt = b;
				if ( b.Speed != 0 )
				{
					speed = b.Speed;
				}
			}
		}
		catch { }
	}
	/// <summary>
	/// Enables the entity.
	/// </summary>
	[Input]
	public void StartForward()
	{
		speed = 20;
		try
		{
			var a = (Target.GetTarget() as path_track);
			if ( a.Speed != 0 )
			{
				speed = a.Speed;
			}
		}
		catch
		{

		}
	}
	/// <summary>
	/// Enables the entity.
	/// </summary>
	[Input]
	new public void Enable()
	{
		Enabled = true;
	}

	/// <summary>
	/// Enables the entity.
	/// </summary>
	[Input]
	public void TeleportToPathTrack( string PathTrack )
	{
		var a = (Entity.FindAllByName( PathTrack ).First() as path_track);
		Position = a.Position;
	}

	/// <summary>
	/// Disables the entity, so that it would not fire any outputs.
	/// </summary>
	[Input]
	new public void Disable()
	{
		Enabled = false;
	}

	/// <summary>
	/// Toggles the enabled state of the entity.
	/// </summary>
	[Input]
	new public void Toggle()
	{
		Enabled = !Enabled;
	}

	[Input]
	public void SetSpeed( float spd )
	{
		speed = spd;
	}
	// stub
}
