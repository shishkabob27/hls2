﻿public partial class HLPlayer : Player, ICombat
{
	TimeSince timeSinceDropped = 0;

	[Net] public float SurfaceFriction { get; set; } = 1;

	public bool InWater => WaterLevelType >= WaterLevelType.Feet;
	public bool IsGrounded => GroundEntity != null;
	public bool IsUnderwater => WaterLevelType >= WaterLevelType.Eyes;
	public bool IsDucked => Tags.Has( PlayerTags.Ducked );
	public bool IsAlive => LifeState == LifeState.Alive;

	public bool IsOnLadder = false;
	public WaterLevelType WaterLevelType { get; internal set; }


	public bool IsObserver = false;
	public bool IsNoclipping = false;


	[Net, Local] public VRHandLeft LeftHand { get; set; }
	[Net, Local] public VRHandRight RightHand { get; set; }
	[Net]
	public float Armour { get; set; } = 0;

	[Net]
	public bool HasHEV { get; set; } = false;

	[Net]
	public bool GodMode { get; set; } = false;

	[Net]
	public float MaxHealth { get; set; } = 100;

	[Net]
	public float MaxArmour { get; set; } = 100;

	//[Net]
	public bool IN_FORWARD { get; set; } = false;
	//[Net]
	public bool IN_LEFT { get; set; } = false;
	//[Net]
	public bool IN_RIGHT { get; set; } = false;
	//[Net]
	public bool IN_BACKWARD { get; set; } = false;

	public float Forward { get; set; }
	public float Left { get; set; }
	public float Up { get; set; }

	[Net]
	public bool IN_USE { get; set; } = false;

	public int button { get; set; } = 0;

	public bool SupressPickupNotices { get; private set; }
	public Rotation BaseRotation;
	public int ComboKillCount { get; set; } = 0;
	public TimeSince TimeSinceLastKill { get; set; }

	[Net]
	public Vector3 WishVelocity { get; set; }

	[ConVar.Replicated] public static bool hl_sfmmode { get; set; } = false;

	[ConVar.ClientData] public static string hl_pm { get; set; } = "player";

	[Net]
	public int team { get; set; } = 1;

	[Net]
	public bool IsCarryingFlag { get; set; } = false;

	[Net, Predicted]
	public Vector3 punchangle { get; set; } = Vector3.Zero;

	public Vector3 punchanglecl = Vector3.Zero;

	public HLPlayer()
	{


		Inventory = new HLInventory( this );
	}

	public bool CanMove()
	{
		return true;
	}
	public void DoHLPlayerNoclip( Client player )
	{
		//if (!player.HasPermission("noclip"))
		//return;

		if ( player.Pawn is HLPlayer basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}
	public override void Respawn()
	{


		//SetModel("models/citizen/citizen.vmdl");

		SetPlayerModel();


		SetAnimGraph( "animgraphs/hl1/player.vanmgrph" );

		Controller = new HL1GameMovement();

		Animator = new HLPlayerAnimator();

		CameraMode = new FirstPersonCamera();


		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		ClearAmmo();

		SupressPickupNotices = true;

		Inventory.DeleteContents();

		SupressPickupNotices = false;
		Health = 100;
		Armour = 0;

		if ( HLGame.GameIsMultiplayer() )
		{
			HasHEV = true;

			Inventory.Add( new Crowbar() );
			Inventory.Add( new Pistol() );

			GiveAmmo( AmmoType.Pistol, 68 );
		}

		Tags.Add( "player" );
		if ( Client.IsUsingVr )
		{
			CreateHands();
		}

		Host.AssertServer();

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;
		WaterLevel = 0;

		CreateHull();

		switch ( HLGame.hl_gamemode )
		{
			case "campagin": Game.Current?.MoveToSpawnpoint( this ); break;
			case "deathmatch": HLGame.MoveToDMSpawnpoint( this ); break;
			case "ctf": Game.Current?.MoveToSpawnpoint( this ); break;
			default: Game.Current?.MoveToSpawnpoint( this ); break;
		}

		ResetInterpolation();

		updtasync();


	}

	[ConCmd.Client]
	public static void ChangeTeam()
	{
		var _ = new TeamSelector();
	}
	
	private void CreateHands()
	{
		DeleteHands();

		LeftHand = new() { Owner = this };
		RightHand = new() { Owner = this };
		LeftHand.Spawn();
		RightHand.Spawn();
	}

	private void DeleteHands()
	{
		LeftHand?.Delete();
		RightHand?.Delete();
	}

	[ConCmd.Server]
	public static void GiveEverything()
	{
		//give all weapons, should auto update no matter what.
		var ply = ConsoleSystem.Caller.Pawn as HLPlayer;
		var weptype = typeof( HLWeapon );
		var weptypes = TypeLibrary.GetDescriptions( weptype );
		foreach ( var weapontype in weptypes )
		{
			var ent = weapontype.Create<Entity>();
			ent.Position = ConsoleSystem.Caller.Pawn.Position;
			(ent as HLWeapon).DeleteIfNotCarriedAfter( 0.1f );
		}
		var ammtype = typeof( BaseAmmo );
		var ammtypes = TypeLibrary.GetDescriptions( ammtype );
		foreach ( var ammotype in ammtypes )
		{
			var ent = ammotype.Create<Entity>();
			ent.Position = ConsoleSystem.Caller.Pawn.Position;
			ent.DeleteAsync( 0.1f );
		}

		var suit = new Suit();
		suit.Position = ConsoleSystem.Caller.Pawn.Position;
		suit.Spawn();
		suit.DeleteAsync( 0.1f );
	}

	[ConCmd.Server]
	public static void GiveAll()
	{
		var ply = ConsoleSystem.Caller.Pawn as HLPlayer;



		Givewep( new Crowbar() );
		Givewep( new Pistol() );
		Givewep( new Python() );
		Givewep( new Shotgun() );
		Givewep( new SMG() );
		Givewep( new RPG() );
		Givewep( new Crossbow() );
		Givewep( new GrenadeWeapon() );
		Givewep( new TripmineWeapon() );
		Givewep( new Gauss() );
		Givewep( new Egon() );
		Givewep( new HornetGun() );
		Givewep( new SnarkWeapon() );
		Givewep( new SatchelWeapon() );

		ply.GiveAmmo( AmmoType.Pistol, 17 );
		ply.GiveAmmo( AmmoType.Python, 6 );
		ply.GiveAmmo( AmmoType.Buckshot, 12 );
		ply.GiveAmmo( AmmoType.Crossbow, 5 );
		//ply.GiveAmmo( AmmoType.Grenade, 1000 );
		ply.GiveAmmo( AmmoType.Pistol, 20 );
		ply.GiveAmmo( AmmoType.SMGGrenade, 3 );
		ply.GiveAmmo( AmmoType.RPG, 1 );
		//ply.GiveAmmo( AmmoType.Tripmine, 1000 );
		//ply.GiveAmmo( AmmoType.Satchel, 1000 );
		ply.GiveAmmo( AmmoType.Uranium, 5 );
		//ply.GiveAmmo( AmmoType.Snark, 1000 );

		var battery = new Battery();
		battery.Position = ConsoleSystem.Caller.Pawn.Position;
		battery.Spawn();
		battery.DeleteAsync( 0.1f );

		var suit = new Suit();
		suit.Position = ConsoleSystem.Caller.Pawn.Position;
		suit.Spawn();
		suit.DeleteAsync( 0.1f );
	}
	static void Givewep( HLWeapon wep )
	{
		wep.Position = ConsoleSystem.Caller.Pawn.Position;
		wep.DeleteIfNotCarriedAfter( 0.1f );
	}
	public override void OnKilled()
	{
		OnKilled( true );
	}

	public void OnKilled( bool corpse = true )
	{
		base.OnKilled();
		DeleteHands();
		RemoveFlashlight();
		HasHEV = false;
		if ( HLGame.GameIsMultiplayer() )
		{
			var coffin = new Coffin();
			coffin.Position = Position + Vector3.Up * 30;
			coffin.Rotation = Rotation;
			coffin.Velocity = Velocity + Rotation.Forward * 100;
			coffin.Populate( this );
		}

		Inventory.DeleteContents();



		if ( corpse )
		{
			CreateCorpse( Velocity, LastDamage.Flags, LastDamage.Position, LastDamage.Force, GetHitboxBone( LastDamage.HitboxIndex ), this );
		}

		Controller = null;

		CameraMode = new DeadCamera();

		EnableAllCollisions = false;
		EnableDrawing = false;

		foreach ( var child in Children.OfType<ModelEntity>() )
		{
			child.EnableDrawing = false;
		}
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( HLGame.CurrentState == HLGame.GameStates.GameEnd )
		{
			input.ViewAngles = input.OriginalViewAngles;
			return;
		};
		base.BuildInput( input );
	}

	public override void FrameSimulate( Client cl )
	{

		if ( Client.IsUsingVr )
		{
			rotationvr();

			if ( Health > 0 )
			{
				LeftHand.FrameSimulate( cl );
				RightHand.FrameSimulate( cl );
			}
		}
		else
		{

			base.FrameSimulate( cl );

		}
	}

	public Rotation vrrotate { get; set; }
	public void rotationvr()
	{
		vrrotate = Rotation.FromYaw( vrrotate.Yaw() - (float)Math.Round( Input.VR.RightHand.Joystick.Value.x, 1 ) * 4 );


		var a = Transform;
		//a.Position = Rotation.FromAxis(Vector3.Up, -(Input.VR.RightHand.Joystick.Value.x * 4)) * (Transform.Position - Input.VR.Head.Position.WithZ(Position.z)) + Input.VR.Head.Position.WithZ(Position.z);

		a.Rotation = vrrotate;// Rotation.FromAxis(Vector3.Up, -(Input.VR.RightHand.Joystick.Value.x * 4)) * Transform.Rotation;

		EyeRotation = a.Rotation;
		Transform = a;
	}
	public override void Simulate( Client cl )
	{
		if ( HLGame.CurrentState == HLGame.GameStates.GameEnd )
			return;


		punchangle = punchangle.Approach( 0, Time.Delta * 14.3f ); // was Delta * 10, 14.3 matches hl1 the most

		Forward = Input.Forward;
		Left = Input.Left;
		Up = Input.Up;
		if ( Client.IsUsingVr )
		{
			EyeRotation = Input.VR.Head.Rotation;

			//offsetsomehowidk = (Input.VR.Head.Position.WithZ(Position.z) - Position.WithZ(Position.z));  
			IN_USE = Input.Down( InputButton.Use );
			IN_FORWARD = Input.VR.RightHand.Joystick.Delta.x > 0;
			IN_LEFT = Input.Down( InputButton.Left );
			IN_RIGHT = Input.Down( InputButton.Right );
			IN_BACKWARD = Input.Down( InputButton.Back );

		}
		else
		{
			IN_USE = Input.Down( InputButton.Use );
			IN_FORWARD = Input.Down( InputButton.Forward );
			IN_LEFT = Input.Down( InputButton.Left );
			IN_RIGHT = Input.Down( InputButton.Right );
			IN_BACKWARD = Input.Down( InputButton.Back );
		}
		base.Simulate( cl );
		SimulateFlashlight( cl );
		//
		// Input requested a weapon switch
		//

		FallDamageThink();

		if ( LeftHand != null && RightHand != null )
		{
			LeftHand.Simulate( cl );
			RightHand.Simulate( cl );
		}
		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		TickPlayerUse();

		if ( Input.Pressed( InputButton.View ) && !Client.IsUsingVr )
		{
			if ( CameraMode is ThirdPersonCamera )
			{
				CameraMode = new FirstPersonCamera();
			}
			else
			{
				CameraMode = new ThirdPersonCamera();
			}
		}

		SimulateActiveChild( cl, ActiveChild );

		//
		// If the current weapon is out of ammo and we last fired it over half a second ago
		// lets try to switch to a better wepaon
		//
		if ( ActiveChild is HLWeapon weapon && !weapon.IsUsable() && weapon.TimeSincePrimaryAttack > 0.5f && weapon.TimeSinceSecondaryAttack > 0.5f )
		{
			SwitchToBestWeapon();
		}
		FootstepSounds();
	}

	TimeSince lastFootStep;
	bool StepLeft;
	void FootstepSounds()
	{
		if ( IsClient ) return;

		float velrun;
		float velwalk;
		float flduck;
		float fvol;

		float speed = Velocity.Length;

		if ( (IsDucked) || IsOnLadder )
		{
			velwalk = 60;       // These constants should be based on cl_movespeedkey * cl_forwardspeed somehow
			velrun = 80;        // UNDONE: Move walking to server
			flduck = 100;
		}
		else
		{
			velwalk = 120;
			velrun = 210;
			flduck = 0;
		}
		bool fWalking = speed < velrun;

		if ( (IsOnLadder || (IsGrounded)) &&
			(Velocity.Length > 0.0) &&
			(speed >= velwalk) )
		{


			if ( lastFootStep > 0.35 && IsOnLadder )
			{
				lastFootStep = 0;
				PlaySound( "pl_ladder" ).SetVolume( 0.5f );
			}

			if ( !HLGame.hl_legacyfootsteps ) return;
			if ( IsOnLadder ) return;
			if ( lastFootStep > (fWalking ? 0.4 : 0.3) )
			{
				StepLeft = !StepLeft;
				lastFootStep = 0;
				fvol = fWalking ? 0.2f : 0.5f;
				if ( IsDucked )
				{
					if ( HLGame.hl_fix_ducking_footsteps ) fvol *= 0.35f; else return;
					if ( HLGame.hl_fix_ducking_footsteps ) lastFootStep = -0.1f;
				}
				plfootstep( Position, fvol, (StepLeft ? 0 : 1) );
			}
		}
	}
	Vector3 prevVel = Vector3.Zero;
	const int PLAYER_FATAL_FALL_SPEED = 1024;// approx 20 metres
	const int PLAYER_MAX_SAFE_FALL_SPEED = 580;// approx 5 metres
	const float DAMAGE_FOR_FALL_SPEED = (float)100 / (PLAYER_FATAL_FALL_SPEED - PLAYER_MAX_SAFE_FALL_SPEED);// damage per unit per second.
	const int PLAYER_MIN_BOUNCE_SPEED = 200;
	const float PLAYER_FALL_PUNCH_THRESHHOLD = (float)350; // won't punch player's screen/make scrape noise unless player falling at least this fast.

	[ConVar.ClientData] public static bool hl_won_fall_damage_sound { get; set; } = false;
	[ConVar.Replicated] public static int mp_falldamage { get; set; } = 0;

	void FallDamageThink()
	{
		if ( IsClient ) return;
		var FallSpeed = -prevVel.z;
		if ( GroundEntity != null && FallSpeed >= PLAYER_FALL_PUNCH_THRESHHOLD )
		{
			float fvol = 0;
			var b = punchangle;

			if ( WaterEntity != null || WaterLevel > 0 )
			{

			}
			else if ( FallSpeed > PLAYER_MAX_SAFE_FALL_SPEED )
			{
				float flFallDamage = (FallSpeed - PLAYER_MAX_SAFE_FALL_SPEED) * DAMAGE_FOR_FALL_SPEED;

				if ( HLGame.GameIsMultiplayer() && mp_falldamage == 0 )
				{
					flFallDamage = 10;
				}

				if ( flFallDamage > Health )
				{
					Sound.FromWorld( "bodysplat", Position );
				}

				if ( flFallDamage > 0 )
				{
					// original hl1 dll had a bug that played these two sounds and the same time so i guess we can have it here if above won cvar is on
					if ( Client.GetClientData( "hl_won_fall_damage_sound" ).ToBool() ) Sound.FromWorld( "pl_fallpain2", Position );
					Sound.FromWorld( "pl_fallpain", Position );
					var a = new DamageInfo
					{
						Damage = flFallDamage,
						Flags = DamageFlags.Fall,
					};
					TakeDamage( a );
					fvol = 1;
					b.x = 0;
				}
			}
			b.z = FallSpeed * 0.013f;   // punch z axis

			if ( b[0] > 8 )
			{
				b[0] = 8;
			}
			punchangle = b;
		}
		prevVel = Velocity;
	}
	void ViewPunchThink()
	{
		//float len;
		//len = punchangle.Length;
		//Log.Info( "1: " + len );
		//len -= ( 10.0f + len * 0.5f ) * 0f;
		//Log.Info( "2: " + len );
		//len = Math.Max( len, 0.0f );
		//Log.Info( "3: " + len );
		//punchangle = punchangle.LerpTo( Vector3.Zero, Time.Delta );

	}
	/*
	float VectorNormalize( Vector3 v )
	{
		float length, ilength;

		length = v.x * v.x + v.y * v.y + v.z * v.z;
		length = (float)Math.Sqrt( length );        // FIXME

		if ( length > 0 )
		{
			ilength = 1 / length;
			v[0] *= ilength;
			v[1] *= ilength;
			v[2] *= ilength;
		}

		return length;

	}
	*/
	new public void Deafen( float strength )
	{
		//Audio.SetEffect("flashbang", strength, velocity: 20.0f, fadeOut: 4.0f * strength);
	}

	public void SwitchToBestWeapon()
	{
		var best = Children.Select( x => x as HLWeapon )
			.Where( x => x.IsValid() && x.IsUsable() )
			.OrderByDescending( x => x.BucketWeight )
			.FirstOrDefault();

		if ( best == null ) return;

		ActiveChild = best;
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		if ( IsClient ) return;

		if ( other is TouchTrigger )
		{
			StartTouch( other.Parent );
			return;
		}

		if ( other is PickupTrigger )
		{
			StartTouch( other.Parent );
			return;
		}

		Inventory?.Add( other, Inventory.Active == null );
	}

	public override void PostCameraSetup( ref CameraSetup setup )
	{
		if ( HLGame.CurrentState == HLGame.GameStates.GameEnd )
			return;

		base.PostCameraSetup( ref setup );

		setup.ZNear = 1;
		setup.ZFar = 25000;

		if ( setup.Viewer != null && !Client.IsUsingVr )
		{
			AddCameraEffects( ref setup );
		}
	}

	float walkBob = 0;
	float fov = 0;

	private void AddCameraEffects( ref CameraSetup setup )
	{
		if ( Client.IsUsingVr ) return;
		if ( Health <= 0 ) return;
		var speed = Velocity.WithZ( 0 ).Length.LerpInverse( 0, 2 );
		var up = setup.Rotation.Up;
		//
		//if ( GroundEntity != null )
		//{
		//walkBob += Time.Delta * 12f * speed;
		//}
		//setup.Position += up * MathF.Sin( walkBob ) * speed * 2;

		setup.FieldOfView += fov;


	}

	const float ARMOUR_RATIO = 0.2f;
	const float ARMOUR_BONUS = 0.5f;



	DamageInfo LastDamage;

	const int HITGROUP_GENERIC = 0;
	const int HITGROUP_HEAD = 1;
	const int HITGROUP_CHEST = 2;
	const int HITGROUP_STOMACH = 3;
	const int HITGROUP_LEFTARM = 4;
	const int HITGROUP_RIGHTARM = 5;
	const int HITGROUP_LEFTLEG = 6;
	const int HITGROUP_RIGHTLEG = 7;
	const int HITGROUP_GEAR = 10;
	const int HITGROUP_SPECIAL = 11;

	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Dead )
			return;

		LastDamage = info;

		switch ( GetHitboxGroup( info.HitboxIndex ) )
		{
			case HITGROUP_GENERIC:
				break;
			case HITGROUP_HEAD:
				info.Damage *= 3;
				break;
			case HITGROUP_CHEST:
				info.Damage *= 1;
				break;
			case HITGROUP_STOMACH:
				info.Damage *= 1;
				break;
			case HITGROUP_LEFTARM:
			case HITGROUP_RIGHTARM:
				info.Damage *= 1;
				break;
			case HITGROUP_LEFTLEG:
			case HITGROUP_RIGHTLEG:
				info.Damage *= 1;
				break;
			default:
				break;
		}

		this.ProceduralHitReaction( info );

		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;

		var flBonus = ARMOUR_BONUS;
		var flRatio = ARMOUR_RATIO;

		if ( info.Flags.HasFlag( DamageFlags.Blast ) && HLGame.GameIsMultiplayer() )
		{
			// blasts damage armour more.
			flBonus *= 2;
		}


		// Armour. 
		if ( !info.Flags.HasFlag( DamageFlags.Fall ) && !info.Flags.HasFlag( DamageFlags.Drown ) ) // armour doesn't protect against fall or drown damage!
		{
			float flNew = info.Damage * flRatio;

			float flArmor;

			flArmor = (info.Damage - flNew) * flBonus;

			// Does this use more armour than we have?
			if ( flArmor > Armour )
			{
				flArmor = Armour;
				flArmor *= (1 / flBonus);
				flNew = info.Damage - flArmor;
				Armour = 0;
			}
			else
				Armour -= flArmor;

			info.Damage = flNew;
		}

		if ( info.Flags.HasFlag( DamageFlags.Blast ) )
		{
			Deafen( To.Single( Client ), info.Damage.LerpInverse( 0, 60 ) );
		}
		bool docorpse = true;

		if ( Health > 0 && info.Damage > 0 && !GodMode )
		{
			Health -= info.Damage;
			if ( Health <= 0 )
			{

				if ( Health < -20 && !info.Flags.HasFlag( DamageFlags.DoNotGib ) )
				{
					HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, Position, Health, this.CollisionBounds, 0 );
					docorpse = false;
				}

				if ( info.Flags.HasFlag( DamageFlags.AlwaysGib ) && docorpse )
				{
					HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, info.Position, Health, this.CollisionBounds, 0 );
					docorpse = false;
				}
				//Health = 0;
				OnKilled( docorpse );


			}
		}


		var b = punchangle;
		b.x = -2;
		punchangle = b;

		if ( info.Attacker is HLPlayer attacker )
		{
			if ( attacker != this )
			{
				attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Health.LerpInverse( 100, 0 ) );
			}

			TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
		}

		//
		// Add a score to the killer
		//
		if ( LifeState == LifeState.Dead && info.Attacker != null )
		{
			if ( info.Attacker.Client != null && info.Attacker != this )
			{
				info.Attacker.Client.AddInt( "kills" );
			}
		}
	}

	[ClientRpc]
	public void DidDamage( Vector3 pos, float amount, float healthinv )
	{
		Sound.FromScreen( "dm.ui_attacker" )
			.SetPitch( 1 + healthinv * 1 );

		HitIndicator.Current?.OnHit( pos, amount );
	}

	public TimeSince TimeSinceDamage = 1.0f;

	[ClientRpc]
	public void TookDamage( Vector3 pos )
	{
		TimeSinceDamage = 0;
		DamageIndicator.Current?.OnHit( pos );
	}

	[ClientRpc]
	public void PlaySoundFromScreen( string sound )
	{
		Sound.FromScreen( sound );
	}

	[ConCmd.Client]
	public static void InflictDamage()
	{
		if ( Local.Pawn is HLPlayer ply )
		{
			ply.TookDamage( ply.Position + ply.EyeRotation.Forward * 100.0f );
		}
	}

	public void Explode()
	{
		var target = (ConsoleSystem.Caller.Pawn as HLPlayer);
		if ( target == null ) return;
		target.TakeDamage( DamageInfo.Generic( (target.Health + (target.Armour) * 2) + 30 ) );
	}

	TimeSince timeSinceLastFootstep = 0;

	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( HLGame.hl_legacyfootsteps ) return;
		if ( LifeState != LifeState.Alive )
			return;

		if ( !IsServer )
			return;

		if ( timeSinceLastFootstep < 0.20f )
			return;

		volume *= FootstepVolume();

		timeSinceLastFootstep = 0;

		//DebugOverlay.Box( 1, pos, -1, 1, Color.Red );
		//DebugOverlay.Text( pos, $"{volume}", Color.White, 5 );
		plfootstep( pos, volume, foot );

	}
	void plfootstep( Vector3 pos, float volume, int foot = 0 )
	{
		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) return;
		var txtnm = "concrete";
		if ( tr.Entity is WorldEntity mdl )
		{
			//get texture name
		}
		tr.Surface.DoHLFootstep( this, tr, foot, volume, txtnm );
	}
	public void OnJump( Vector3 pos )
	{
		if ( LifeState != LifeState.Alive )
			return;

		if ( !IsServer )
			return;


		//var volume *= FootstepVolume();


		//DebugOverlay.Box( 1, pos, -1, 1, Color.Red );
		//DebugOverlay.Text( pos, $"{volume}", Color.White, 5 );

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 10 ) // if we were able to jump we must've been on something, fixes jump sound not being played jumping off of thin / small surfaces
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) return;

		tr.Surface.DoHLJump( this, tr, 2 );
	}


	public void RenderHud( Vector2 screenSize )
	{
		if ( LifeState != LifeState.Alive )
			return;

		// RenderOverlayTest( screenSize );
	}

	public int Classify()
	{
		return (int)HLCombat.Class.CLASS_PLAYER;
	}
}
public static class PlayerTags
{
	/// <summary>
	/// Is currently ducking.
	/// </summary>
	public const string Ducked = "ducked";
	/// <summary>
	/// Is currently performing a water jump.
	/// </summary>
	public const string WaterJump = "waterjump";
	/// <summary>
	/// Is currently in cheat activated noclip mode.
	/// </summary>
	public const string Noclipped = "noclipped";
	/// <summary>
	/// Does not accept any damage.
	/// </summary>
	public const string GodMode = "god";
	/// <summary>
	/// Take all the damage, but don't die.
	/// </summary>
	public const string Buddha = "buddha";
}
partial class HLPlayer
{
	public virtual Vector3 GetPlayerMins( bool ducked )
	{
		if ( IsObserver )
			return ViewVectors.ObserverHullMin;
		else
			return ducked ? ViewVectors.DuckHullMin : ViewVectors.HullMin;
	}

	public Vector3 GetPlayerMinsScaled( bool ducked )
	{
		return GetPlayerMins( ducked ) * Scale;
	}

	public virtual Vector3 GetPlayerMaxs( bool ducked )
	{
		if ( IsObserver )
			return ViewVectors.ObserverHullMax;
		else
			return ducked ? ViewVectors.DuckHullMax : ViewVectors.HullMax;
	}

	public Vector3 GetPlayerMaxsScaled( bool ducked )
	{
		return GetPlayerMaxs( ducked ) * Scale;
	}

	public virtual Vector3 GetPlayerExtents( bool ducked )
	{
		var mins = GetPlayerMins( ducked );
		var maxs = GetPlayerMaxs( ducked );

		return mins.Abs() + maxs.Abs();
	}

	public Vector3 GetPlayerExtentsScaled( bool ducked )
	{
		return GetPlayerExtents( ducked ) * Scale;
	}

	public virtual Vector3 GetPlayerViewOffset( bool ducked )
	{
		return ducked ? ViewVectors.DuckViewOffset : ViewVectors.ViewOffset;
	}

	public Vector3 GetPlayerViewOffsetScaled( bool ducked )
	{
		return GetPlayerViewOffset( ducked ) * Scale;
	}

	public virtual ViewVectors ViewVectors => new()
	{
		ViewOffset = new( 0, 0, 64 ),

		HullMin = new( -16, -16, 0 ),
		HullMax = new( 16, 16, 72 ),

		DuckHullMin = new( -16, -16, 0 ),
		DuckHullMax = new( 16, 16, 36 ),
		DuckViewOffset = new( 0, 0, 28 ),

		ObserverHullMin = new( -10, -10, -10 ),
		ObserverHullMax = new( 10, 10, 10 ),

		DeadViewOffset = new( 0, 0, 14 )
	};
}

public struct ViewVectors
{
	public Vector3 ViewOffset { get; set; }

	public Vector3 HullMin { get; set; }
	public Vector3 HullMax { get; set; }

	public Vector3 DuckHullMin { get; set; }
	public Vector3 DuckHullMax { get; set; }
	public Vector3 DuckViewOffset { get; set; }

	public Vector3 ObserverHullMax { get; set; }
	public Vector3 ObserverHullMin { get; set; }

	public Vector3 DeadViewOffset { get; set; }
}

