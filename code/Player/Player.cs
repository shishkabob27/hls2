public partial class HLPlayer : Player, ICombat
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
	[Net]
	public bool IsNoclipping { get; set; } = false;

	public bool IsInVR = false;
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
	public void DoHLPlayerNoclip( IClient player )
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
			IsInVR = true;
		}

		Game.AssertServer();

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;
		//WaterLevel = 0;

		CreateHull();

		switch ( HLGame.sv_gamemode )
		{
			case "campagin": HLGame.MoveToSpawnpoint( this ); break;
			case "deathmatch": HLGame.MoveToDMSpawnpoint( this ); break;
			case "ctf": HLGame.MoveToCTFSpawnpoint( this ); break;
			default: HLGame.MoveToDMSpawnpoint( this ); break;
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
		var weptype = typeof( Weapon );
		var weptypes = TypeLibrary.GetTypes( weptype );
		foreach ( var weapontype in weptypes )
		{
			ply.GiveWeapon( weapontype.Create<Weapon>() );
		}
		var ammtype = typeof( BaseAmmo );
		var ammtypes = TypeLibrary.GetTypes( ammtype );
		foreach ( var ammotype in ammtypes )
		{
			var ent = ammotype.Create<ModelEntity>();
			if ( HLGame.sv_force_physics )
			{
				ent.PhysicsEnabled = false;
				ent.Position = ply.Position;
			}
			else
			{
				ent.Position = ply.CollisionWorldSpaceCenter;
			}
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



		ply.GiveWeapon( new Crowbar() );
		ply.GiveWeapon( new Pistol() );
		ply.GiveWeapon( new Python() );
		ply.GiveWeapon( new Shotgun() );
		ply.GiveWeapon( new SMG() );
		ply.GiveWeapon( new RPG() );
		ply.GiveWeapon( new Crossbow() );
		ply.GiveWeapon( new GrenadeWeapon() );
		ply.GiveWeapon( new TripmineWeapon() );
		ply.GiveWeapon( new Gauss() );
		ply.GiveWeapon( new Egon() );
		ply.GiveWeapon( new HornetGun() );
		ply.GiveWeapon( new SnarkWeapon() );
		ply.GiveWeapon( new SatchelWeapon() );

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
	public void GiveWeapon( Weapon wep )
	{
		if ( HLGame.sv_force_physics )
		{
			wep.PhysicsEnabled = false;
			wep.Position = Position;
		}
		else
		{
			wep.Position = CollisionWorldSpaceCenter;
		}
		wep.DeleteIfNotCarriedAfter( 0.1f );
	}
	public override void OnKilled()
	{
		OnKilled( true );
	}

	public void OnKilled( bool corpse = true )
	{
		HLGame.Current?.OnKilled( this );

		timeSinceDied = 0;
		LifeState = LifeState.Dead;
		StopUsing();

		Client?.AddInt( "deaths", 1 );

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
			CreateCorpse( Velocity, LastDamage.Tags, LastDamage.Position, LastDamage.Force, LastDamage.Hitbox.GetName(), this );
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

	public override void BuildInput()
	{
		if ( HLGame.CurrentState == HLGame.GameStates.GameEnd )
		{
			ViewAngles = OriginalViewAngles;
			return;
		};
		base.BuildInput();
	}

	public override void FrameSimulate( IClient cl )
	{

		if ( Client.IsUsingVr )
		{
			rotationvr();

			var postProcess = Camera.Main.FindOrCreateHook<Sandbox.Effects.ScreenEffects>();

			if ( Health > 0 )
			{
				LeftHand.FrameSimulate( cl );
				RightHand.FrameSimulate( cl );
				postProcess.Saturation = 1;
			}
			else
			{
				postProcess.Saturation = 0;
			}

			if ( LeftHand != null && RightHand != null )
				if ( HasHEV )
				{
					LeftHand.SetModel( "models/vr/v_hand_hevsuit/v_hand_hevsuit_left.vmdl" );
					RightHand.SetModel( "models/vr/v_hand_hevsuit/v_hand_hevsuit_right.vmdl" );
				}
				else
				{
					LeftHand.SetModel( "models/vr/v_hand_labcoat/v_hand_labcoat_left.vmdl" );
					RightHand.SetModel( "models/vr/v_hand_labcoat/v_hand_labcoat_right.vmdl" );
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

	TimeSince timeSinceDied;

	public override void Simulate( IClient cl )
	{
		if ( HLGame.CurrentState == HLGame.GameStates.GameEnd )
			return;

		punchangle = punchangle.Approach( 0, Time.Delta * 14.3f ); // was Delta * 10, 14.3 matches hl1 the most

		Forward = Input.AnalogMove.x;
		Left = Input.AnalogMove.y;
		Up = Input.AnalogMove.z;
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

		if ( LifeState == LifeState.Dead )
		{
			if ( timeSinceDied > HLGame.hl_respawn_time && Game.IsServer )
			{
				Respawn();
			}

			return;
		}

		//UpdatePhysicsHull();

		var controller = GetActiveController();
		controller?.Simulate( cl, this, GetActiveAnimator() );

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
		if ( ActiveChildInput.IsValid() && ActiveChildInput.Owner == this )
		{
			ActiveChild = ActiveChildInput;
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
		if ( ActiveChild is Weapon weapon && !weapon.IsUsable() && weapon.TimeSincePrimaryAttack > 0.5f && weapon.TimeSinceSecondaryAttack > 0.5f )
		{
			SwitchToBestWeapon();
		}
		FootstepSounds();
		SimulateSuit();
		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				if ( dropped.PhysicsGroup != null )
				{
					dropped.PhysicsGroup.Velocity = Velocity + (EyeRotation.Forward + EyeRotation.Up) * 300;
				}

				dropped.Velocity = Velocity + (EyeRotation.Forward + (EyeRotation.Up / 3)) * 255;

				timeSinceDropped = 0;
				SwitchToBestWeapon();
			}
		}
		SimulateAnimator();
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
		var best = Children.Select( x => x as Weapon )
			.Where( x => x.IsValid() && x.IsUsable() )
			.OrderByDescending( x => x.BucketWeight )
			.FirstOrDefault();

		if ( best == null ) return;

		ActiveChild = best;
		ActiveChildInput = best;
	}

	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		if ( Game.IsClient ) return;

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

	private void AddCameraEffects( ref CameraSetup setup )
	{
		if ( Client.IsUsingVr ) return;
		if ( Health <= 0 ) return;
		var speed = Velocity.WithZ( 0 ).Length.LerpInverse( 0, 2 );
		var up = setup.Rotation.Up;
	}

	const float ARMOUR_RATIO = 0.2f;
	const float ARMOUR_BONUS = 0.5f;


	float healthPrev = 100;

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
		healthPrev = Health;
		if ( LifeState == LifeState.Dead )
			return;

		LastDamage = info;
		if ( info.Hitbox.HasTag( "generic" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "head" ) )
			info.Damage *= 3;
		if ( info.Hitbox.HasTag( "chest" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "stomach" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "arm" ) )
			info.Damage *= 1;
		if ( info.Hitbox.HasTag( "leg" ) )
			info.Damage *= 1;

		/*switch ( GetHitboxGroup( info.HitboxIndex ) )
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
		}*/

		this.ProceduralHitReaction( info );

		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;

		var flBonus = ARMOUR_BONUS;
		var flRatio = ARMOUR_RATIO;

		if ( info.HasTag( DamageFlags.Blast ) && HLGame.GameIsMultiplayer() )
		{
			// blasts damage armour more.
			flBonus *= 2;
		}


		// Armour. 
		if ( !info.HasTag( DamageFlags.Fall ) && !info.HasTag( DamageFlags.Drown ) ) // armour doesn't protect against fall or drown damage!
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

		if ( info.HasTag( DamageFlags.Blast ) )
		{
			Deafen( To.Single( Client ), info.Damage.LerpInverse( 0, 60 ) );
		}
		bool docorpse = true;

		if ( Health > 0 && info.Damage > 0 && !GodMode )
		{
			Health -= info.Damage;
			if ( Health <= 0 )
			{

				if ( Health < -20 && !info.HasTag( DamageFlags.DoNotGib ) )
				{
					HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, Position, Health, this.CollisionBounds, 0 );
					docorpse = false;
				}

				if ( info.HasTag( DamageFlags.AlwaysGib ) && docorpse )
				{
					HLCombat.CreateGibs( this.CollisionWorldSpaceCenter, info.Position, Health, this.CollisionBounds, 0 );
					docorpse = false;
				}
				//Health = 0;
				OnKilled( docorpse );


			}
		}

		SuitTalkDamage( info );

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
		if ( Game.LocalPawn is HLPlayer ply )
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

		if ( !Game.IsServer )
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

		if ( !Game.IsServer )
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

