[Library( "hl_hornet" )]
[HideInEditor]
partial class Hornet : NPC, ICombat
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/hornet.vmdl" );

	bool Stuck;
	public bool alienShot = false;
	public bool Dart = false;

	const int HORNET_RED = 0;
	const int HORNET_ORANGE = 1;

	float FlySpeed = 800;
	float FlySpeedOrange = 800;
	float FlySpeedRed = 600;
	float StopAttack = 200;
	float StartAttack = 200;
	[Net]
	int Type { get; set; } = 1;
	Vector3 OrangeColour = new Vector3( 1f, 0.5f, 0f );
	Vector3 RedColour = new Vector3( 0.7f, 0.15f, 0.05f );

	Particles Trail;
	public override int Classify()
	{
		//if ( alienShot ) return (int)HLCombat.Class.CLASS_ALIEN_BIOWEAPON;
		return (int)HLCombat.Class.CLASS_PLAYER_BIOWEAPON;
	}
	public override void TakeDamage( DamageInfo info )
	{
		// Hornets shouldn't gib.
		info.Damage = 2;
		base.TakeDamage( info );
	}
	public override void Spawn()
	{
		EyeHeight = 0;
		NoNav = true;
		entFOV = 0.9f;
		Health = 1;
		if ( Rand.Int( 1, 5 ) <= 2 )
		{
			Type = HORNET_RED;
			FlySpeed = FlySpeedRed;
		}
		else
		{
			Type = HORNET_ORANGE;
			FlySpeed = FlySpeedOrange;
		}

		base.Spawn();
		if ( HLGame.hl_gamemode == "deathmatch" )
		{
			// hornets don't live as long in multiplayer
			StopAttack = Time.Now + 3.5f;
		}
		else
		{
			StopAttack = Time.Now + 5.0f;
		}
		StartAttack = Time.Now + 0.2f;

		PlaySound( "ag_fire" );
		Tags.Clear();
		Tags.Add( "debris" );
		TrailEffect();
		Model = WorldModel;
	}
	[ClientRpc]
	public void TrailEffect()
	{
		Trail = Particles.Create( "particles/hornet_trail.vpcf", Position );
		if ( Type == HORNET_ORANGE )
		{
			Trail.SetPosition( 1, OrangeColour );
		}
		else
		{
			Trail.SetPosition( 1, RedColour );
		}
		Trail.SetEntity( 0, this );
	}
	[ClientRpc]
	public void DeleteTrailEffect()
	{
		if ( Trail != null ) Trail.Destroy();
	}

	public override void ProcessEntity( Entity ent, int rel )
	{
		if ( Time.Now < StartAttack || Dart ) return;
		if ( rel > 0 && ent.Position.Distance( Position ) < 512 )
		{
			var EnemyLKP = ent.Position;
			if ( ( ent as ModelEntity ) != null )
			{
				EnemyLKP = ( ( ent as ModelEntity ).CollisionWorldSpaceCenter - Position );
			}
			else
			{
				EnemyLKP = EnemyLKP + Velocity * FlySpeed * 0.1f;
			}

			Vector3 vecFlightDir;
			var vecDirToEnemy = EnemyLKP.Normal;

			if ( Velocity.Length < 0.1 )
				vecFlightDir = vecDirToEnemy;
			else
				vecFlightDir = Velocity.Normal;

			var flDelta = Vector3.Dot( vecFlightDir, vecDirToEnemy );
			if ( flDelta < 0.5 )
			{// hafta turn wide again. play sound

				PlaySound( "ag_buzz" );
			}
			if ( flDelta <= 0 && Type == HORNET_RED )
			{// no flying backwards, but we don't want to invert this, cause we'd go fast when we have to turn REAL far.
				flDelta = 0.25f;
			}

			Velocity = ( vecFlightDir + vecDirToEnemy ).Normal;
			if ( alienShot )
			{
				// random pattern only applies to hornets fired by monsters, not players. 
				var a = Velocity;
				a.x += Rand.Float( -0.10f, 0.10f );// scramble the flight dir a bit.
				a.y += Rand.Float( -0.10f, 0.10f );
				a.z += Rand.Float( -0.10f, 0.10f );
				Velocity = a;
			}

			switch ( Type )
			{
				case HORNET_RED:
					Velocity = Velocity * ( FlySpeed * flDelta );// scale the dir by the ( speed * width of turn )
					StartAttack = Time.Now + Rand.Float( 0.1f, 0.3f );
					break;
				case HORNET_ORANGE:
					Velocity = Velocity * FlySpeed;// do not have to slow down to turn.
					StartAttack = Time.Now + 0.1f;// fixed think time
					break;
			}

			if ( HLGame.hl_gamemode != "deathmatch" )
			{
				if ( flDelta >= 0.4 && ( Position - EnemyLKP ).Length <= 300 )
				{

					PlaySound( "ag_buzz" );

					Velocity = Velocity * 2;
					StartAttack = Time.Now + 1.0f;
				}
			}

		}
	}

	[Event.Tick.Server]
	public virtual void Tick2()
	{
		if ( !IsServer )
			return;

		if ( Stuck )
			return;
		if ( Time.Now > StopAttack )
		{
			PlaySound( "ag_hornethit" );

			DeleteTrailEffect();
			Delete();
			return;
		}


		Rotation = Rotation.LookAt( Velocity, new Vector3( 0, 0, 1 ) );

		var start = Position;
		var end = start + ( Velocity * Time.Delta );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				//.HitLayer( CollisionLayer.Water, !InWater )
				.Ignore( this )
				.Size( 1.0f )
				.Run();


		if ( tr.Hit )
		{
			if ( tr.Entity == Owner )
			{
				Position = end;
				return;
			}
			if ( tr.Entity.IsValid() && GetRelationship( tr.Entity ) == HLCombat.R_NO && !Dart )
			{
				//Rotation =

				var a = Velocity.Normal;
				a.x *= -1;
				a.y *= -1;
				Position = Position + a * 4;
				Velocity = a * FlySpeed;

				return;
			}
			// TODO: SPARKY PARTICLES (unless flesh)

			Stuck = true;
			Position = tr.EndPosition + Rotation.Forward * -1;

			if ( tr.Entity.IsValid() )
			{
				PlaySound( "ag_hornethit" );
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, tr.Direction * 200, 8 )
													.UsingTraceResult( tr )
													.WithAttacker( Owner )
													.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}


			// delete self
			DeleteTrailEffect();
			Delete();
		}
		else
		{
			Position = end;
		}
	}
}
