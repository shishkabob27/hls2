
[Library( "hl_tripmine_planted", Title = "Planted Tripmine" )]
partial class Tripmine : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/tripmine.vmdl" );

	Particles LaserParticle;
	LaserTrigger LaserTrigger;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

	}

	public async Task Arm( float seconds )
	{
		// todo: PlaySound doesn't play any sound unless there's a little delay here?
		await Task.DelaySeconds( .01f );

		PlaySound( "tripmine_deploy" );

		await Task.DelaySeconds( seconds );

		if ( !IsValid ) return;

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		LaserParticle = Particles.Create( "particles/tripmine_laser.vpcf", this, "laser", true );
		LaserParticle.SetPosition( 0, Position );

		var tr = Trace.Ray( Position, Position + Rotation.Forward * 4000.0f )
					.Ignore(this)
					.Run();

		LaserParticle.SetPosition( 1, tr.EndPosition );

		LaserTrigger = new LaserTrigger();
		LaserTrigger.SetParent( this, "laser", Transform.Zero );
		LaserTrigger.CreateTrigger( tr.Distance );
		LaserTrigger.OnTriggered = ( e ) => _ = Explode( 0.2f );

		PlaySound( "tripmine_armed" );

		if ( tr.Entity != null && tr.Entity is not WorldEntity )
		{
			await Explode( 0.5f );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		if ( info.Attacker.IsValid() )
		{
			Owner = info.Attacker;
		}

		_ = Explode( 0.3f );
	}

	bool exploding = false;

	async Task Explode( float delay )
	{
		if ( exploding ) return;

		PlaySound( "dm.tripmine_activated" );

		LaserParticle?.Destroy( true );
		LaserParticle = null;

		exploding = true;
		await Task.DelaySeconds( delay );

		HLGame.Explosion( this, Owner, Position, 400, 150, 1.0f );
		Delete();
	}
}

public class LaserTrigger : ModelEntity
{
	public Action<Entity> OnTriggered { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		// Client doesn't need to know about this ;)
		Transmit = TransmitType.Never;
	}

	public void CreateTrigger( float length )
	{
		SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, new Capsule( Vector3.Zero, Rotation.Forward * length, 0.2f ) );
		CollisionGroup = CollisionGroup.Trigger;
        Tags.Add("trigger");
    }

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is WorldEntity ) return;
		if ( other is BaseTrigger ) return;
		if (other is Tripmine) return;

		OnTriggered?.Invoke( other );
	}
}
