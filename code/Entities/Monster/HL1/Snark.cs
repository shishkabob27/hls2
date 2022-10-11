[Library( "monster_snark" ), HammerEntity]
[EditorModel( "models/hl1/monster/snark.vmdl" )]
[Title( "Snark" ), Category( "Monsters" ), Icon( "person" )]
public class Snark : NPC
{
	const int SQUEEK_DETONATE_DELAY = 15;
	float Die;
	float flpitch;
	float StartAttack = 0;
	float StartAttack2 = 0;
	float NextHunt;
	float NextHunt2;
	float NextSound;
	float NextSound2;
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/squeak_npc.vmdl" );
	Angles RotAngles;
	Vector3 posPrev;
	Entity PrevGroundEntity;
	public override void Spawn()
	{
		base.Spawn();
		EyeHeight = 5;
		SetupPhysicsFromOBB( PhysicsMotionType.Keyframed, new Vector3( -4f, -4f, 0f ), new Vector3( 4f, 4f, 8f ) );
		Model = WorldModel;
		NPCAnimGraph = "animgraphs/hl1/monster/squeak_npc.vanmgrph";
		SetAnimGraph( NPCAnimGraph );
		Die = Time.Now + SQUEEK_DETONATE_DELAY;
		GroundBounce = 1;
		WallBounce = 1;
		HasFriction = false;
		Unstick = true;
		SticktoFloor = false;
		PlaySound( "sqk_throw" );
		entFOV = 0; // 180 degrees
		Tags.Clear();
		Tags.Add( "solid" );

	}
	public override void ProcessEntity( Entity ent, int rel )
	{

		if ( ent.Position.Distance( Position ) < 512 && ent is not Snark && ent != Owner )
		{
			if ( ent is Snark ) return;
			if ( ent == Owner ) return;
			if ( Time.Now < StartAttack2 ) return;
			StartAttack2 = Time.Now + 0.1f;
			if ( Time.Now < NextHunt2 ) return;

			GroundEntity = null;
			Position += new Vector3( 0, 0, 1 );

			NextHunt2 = Time.Now + 2.0f;
			var vecDir = ent.Position - Position;
			if ( (ent as ModelEntity) != null )
			{
				vecDir = ((ent as ModelEntity).CollisionWorldSpaceCenter - Position);
			}

			var vecTarget = vecDir.Normal;

			float flVel = Velocity.Length;
			float flAdj = 50.0f / (flVel + 10.0f);

			if ( flAdj > 1.2 )
				flAdj = 1.2f;

			// ALERT( at_console, "think : enemy\n");

			// ALERT( at_console, "%.0f %.2f %.2f %.2f\n", flVel, m_vecTarget.x, m_vecTarget.y, m_vecTarget.z );

			GroundEntity = null;
			Position += new Vector3( 0, 0, 1 );
			Velocity = Velocity * flAdj + vecTarget * 300;

		}
	}
	public override void Think()
	{

		if ( Time.Now < StartAttack ) return;
		StartAttack = Time.Now + 0.1f;
		if ( Time.Now >= Die )
		{
			Velocity = Velocity.Normal;
			Sound.FromWorld( "sqk_blast", Position );
			Delete();
			return;
		}
		if ( (Die - Time.Now <= 0.5) && (Die - Time.Now >= 0.3) )
		{
			Sound.FromEntity( "sqk_die", this ).SetPitch( (float)Math.Sqrt( (100 + Rand.Float( 0, 64 )) / 100 ) );
		}

		if ( Time.Now < NextHunt ) return;
		NextHunt = Time.Now + 2.0f;
		if ( GroundEntity != null )
		{
			AngularVelocity = new Vector3( 0, 0, 0 ).EulerAngles;
		}
		else
		{
			if ( AngularVelocity == new Vector3( 0, 0, 0 ).EulerAngles )
			{
				var a = AngularVelocity;
				a.pitch = Rand.Float( -100, 100 );
				a.roll = Rand.Float( -100, 100 );
				AngularVelocity = a;
			}
		}
		if ( (Position - posPrev).Length < 1.0 )
		{
			var a = Velocity;
			a.x = Rand.Float( -100, 100 );
			a.y = Rand.Float( -100, 100 );
			//Velocity = a;
		}

		RotAngles.yaw = Rotation.LookAt( Velocity, new Vector3( 0, 0, 1 ) ).Yaw();
		posPrev = Position;
	}
	[Event.Tick.Server]
	public void tick2()
	{
		RotAngles += AngularVelocity * Time.Delta;
		Rotation = RotAngles.ToRotation();
		if ( GroundEntity != null )
		{
			if ( GroundEntity != PrevGroundEntity )
			{
				NextHunt = Time.Now - 1;
				NextHunt2 = Time.Now - 1;
				StartAttack = Time.Now - 1;
				StartAttack2 = Time.Now - 1;
				Bounce( GroundEntity );
				//Position = Position.WithZ( Position.z + 53 );
			}
			//PrevGroundEntity = GroundEntity;
		}
		if ( WallEntity != null )
		{
			NextHunt = Time.Now - 1;
			NextHunt2 = Time.Now - 1;
			StartAttack = Time.Now - 1;
			StartAttack2 = Time.Now - 1;
			Bounce( WallEntity );
		}
	}
	public override void Touch( Entity other )
	{
		Bounce( other );
		base.StartTouch( other );
	}
	void Bounce( Entity other )
	{
		if ( IsClient ) return;
		Owner = null;
		NextHunt = Time.Now - 1;
		NextHunt2 = Time.Now - 1;
		RotAngles.pitch = 0;
		RotAngles.roll = 0;
		AngularVelocity = new Vector3( 0, 0, 0 ).EulerAngles;
		flpitch = (float)Math.Sqrt( (155.0f - 60.0f * ((Die - Time.Now) / SQUEEK_DETONATE_DELAY)) / 100 );
		if ( other is ICombat && other is not Snark && Time.Now > NextSound2 )
		{

			NextSound2 = Time.Now + 0.4f;
			Sound.FromEntity( "sqk_deploy", this ).SetPitch( flpitch );
			var damageInfo = DamageInfo.FromBullet( Position, other.Position * 200, 10 )
													.WithAttacker( Owner )
													.WithWeapon( this );

			other.TakeDamage( damageInfo );
		}

		if ( Time.Now < NextSound ) return;
		NextSound = Time.Now + 0.2f;
		Sound.FromEntity( "sqk_hunt", this ).SetPitch( flpitch );
	}
}
