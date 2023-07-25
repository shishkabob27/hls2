using Sandbox;

[Library( "func_trackautochange" )]
[HammerEntity, Solid]
[Title( "func_trackautochange" ), Category("Brush Entities")]
public partial class func_trackautochange : BrushEntity
{

	// stub

	[Property( "height" )]
	public int height { get; set; }

	[Property( "rotation" )]
	public int rotation { get; set; }

	[Property( "train" ), FGDType( "target_destination" )]
	public EntityTarget train { get; set; }

	[Property( "speed" )]
	public int speed { get; set; }

	[Property( "toptrack" ), FGDType( "target_destination" )]
	public EntityTarget toptrack { get; set; }

	[Property( "bottomtrack" ), FGDType( "target_destination" )]
	public EntityTarget bottomtrack { get; set; }

	bool ShouldMove = false;

	public float ogRotation = 0;
	public Vector3 ogPos;

	float ogTrainRotation = 0;
	public Vector3 ogTrainPos;

	public Vector3 TrainVelocity;

	[GameEvent.Tick.Server]
	public void tick()
	{
		if ( !ShouldMove )
			return;

		var newpos = new Vector3(ogPos.x, ogPos.y, ogPos.z + height);
		TrainVelocity = (((Position - (newpos)).Normal * speed)) * -1;
		Position += TrainVelocity * Time.Delta;
		Position = new Vector3( ogPos.x, ogPos.y, Position.z );
	
		var progress = (Position.z - ogPos.z) / height;
		var newrot = ogRotation + (rotation * progress);
		Rotation = Rotation.FromYaw(newrot);

		train.GetTarget().Position = ogTrainPos + (new Vector3(0, 0, Position.z - ogPos.z));
		train.GetTarget().Rotation = Rotation.FromYaw(ogTrainRotation + (rotation * progress));



		if (Position.z.AlmostEqual( newpos.z , 2  ))
		{
			ShouldMove = false;

			Position = newpos;
			Rotation = Rotation.FromYaw(ogRotation + rotation);

			train.GetTarget().Position = ogTrainPos + (new Vector3(0, 0, height));
			train.GetTarget().Rotation = Rotation.FromYaw(ogTrainRotation + rotation);

			(train.GetTarget() as func_tracktrain).Target = bottomtrack;
			(train.GetTarget() as func_tracktrain).StartForward();
		}

	}

	[Input]
	public void Trigger()
	{
		ogRotation = Rotation.Yaw();
		ogPos = Position;

		ogTrainRotation = train.GetTarget().Rotation.Yaw();
		ogTrainPos = train.GetTarget().Position;

		ShouldMove = true;
	}
}   
