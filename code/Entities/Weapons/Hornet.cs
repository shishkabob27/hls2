[Library( "hl_hornet" )]
[HideInEditor]
partial class Hornet : NPC, ICombat
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/crossbow_bolt.vmdl" );

	bool Stuck;
	public bool alienShot = false;
	public bool Dart = false;

	float FlySpeed = 800;
	float FlySpeedAlt = 600;
	float StopAttack = 200;
	float StartAttack = 200;

	Particles Trail;
	public override int Classify()
	{
		//if ( alienShot ) return (int)HLCombat.Class.CLASS_ALIEN_BIOWEAPON;
		return (int)HLCombat.Class.CLASS_PLAYER_BIOWEAPON;
	}
	public override void Spawn()
	{
		NoNav = true;
		entFOV = 0.9f;
		Health = 1;
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

		Model = WorldModel;
	}
	[ClientRpc]
	public void TrailEffect()
	{
		Trail = Particles.Create( "particles/hornet_trail.vpcf", Position );
		Trail.SetPosition( 1, new Vector3( 1f, 0.5f, 0f ) );
		Trail.SetEntity( 0, this );
	}
	public override void ProcessEntity( Entity ent, int rel )
	{
		if ( Time.Now < StartAttack ) return;
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
			Velocity *= FlySpeed;
			Rotation = Rotation.LookAt( Velocity );

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
			if ( Trail != null ) Trail.Destroy();
			Delete();
			return;
		}


		var start = Position;
		var end = start + ( Velocity * Time.Delta );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				//.HitLayer( CollisionLayer.Water, !InWater )
				.Ignore( Owner )
				.Ignore( this )
				.Size( 1.0f )
				.Run();


		if ( tr.Hit )
		{

			if ( tr.Entity.IsValid() && GetRelationship( tr.Entity ) == HLCombat.R_NO )
			{
				//Rotation =
				if ( !Dart )
				{
					var a = Velocity.Normal;
					a.x *= -1;
					a.y *= -1;
					Velocity = a;
					Position = Position + Velocity * 4;
					Velocity = Velocity * FlySpeed;
				}
				else
				{

					// delete self
					if ( Trail != null ) Trail.Destroy();
					Delete();
				}
				return;
			}
			// TODO: SPARKY PARTICLES (unless flesh)

			Stuck = true;
			Position = tr.EndPosition + Rotation.Forward * -1;

			if ( tr.Entity.IsValid() )
			{
				PlaySound( "ag_hornethit" );
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, tr.Direction * 200, 20 )
													.UsingTraceResult( tr )
													.WithAttacker( Owner )
													.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}


			// delete self
			if ( Trail != null ) Trail.Destroy();
			Delete();
		}
		else
		{
			Position = end;
		}
	}
}
