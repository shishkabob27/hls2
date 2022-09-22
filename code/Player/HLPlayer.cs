public partial class HLPlayer : Player, ICombat
{
	TimeSince timeSinceDropped = 0;

	[Net, Local] public VRHandLeft LeftHand { get; set; }
	[Net, Local] public VRHandRight RightHand { get; set; }
	[Net]
	public float Armour { get; set; } = 0;

	[Net]
	public bool HasHEV { get; set; } = false;

	[Net]
	public float MaxHealth { get; set; } = 100;

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

	public HLPlayer()
	{
		Inventory = new HLInventory( this );
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

		var pm = "";
		switch ( Client.GetClientData( "hl_pm" ) )
		{
			case "player": pm = "models/hl1/player/player.vmdl"; break; // helmet is a different model, it has colour support and this doesn't. I like this better so add it here since yeah
			case "barney": pm = "models/hl1/player/barney.vmdl"; break;
			case "helmet": pm = "models/hl1/player/helmet.vmdl"; break;
			case "robo": pm = "models/hl1/player/robo.vmdl"; break;
			default: pm = "models/hl1/player/player.vmdl"; break;
		}

		SetModel( pm );

		SetAnimGraph( "animgraphs/player.vanmgrph" );

		Controller = new HLWalkController();

		Animator = new HLPlayerAnimator();

		CameraMode = new HLFirstPersonCamera();


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

		if ( HLGame.hl_gamemode == "deathmatch" )
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
		base.Respawn();
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
	public static void GiveAll()
	{
		var ply = ConsoleSystem.Caller.Pawn as HLPlayer;



		ply.Inventory.Add( new Crowbar() );
		ply.Inventory.Add( new Pistol() );
		ply.Inventory.Add( new Python() );
		ply.Inventory.Add( new Shotgun() );
		ply.Inventory.Add( new SMG() );
		ply.Inventory.Add( new RPG() );
		ply.Inventory.Add( new Crossbow() );
		ply.Inventory.Add( new GrenadeWeapon() );
		ply.Inventory.Add( new TripmineWeapon() );
		ply.Inventory.Add( new Gauss() );
		ply.Inventory.Add( new Egon() );
		ply.Inventory.Add( new HornetGun() );
		ply.Inventory.Add( new SnarkWeapon() );
		ply.Inventory.Add( new SatchelWeapon() );

		ply.GiveAmmo( AmmoType.Pistol, 1000 );
		ply.GiveAmmo( AmmoType.Python, 1000 );
		ply.GiveAmmo( AmmoType.Buckshot, 1000 );
		ply.GiveAmmo( AmmoType.Crossbow, 1000 );
		ply.GiveAmmo( AmmoType.Grenade, 1000 );
		ply.GiveAmmo( AmmoType.SMGGrenade, 1000 );
		ply.GiveAmmo( AmmoType.RPG, 1000 );
		ply.GiveAmmo( AmmoType.Tripmine, 1000 );
		ply.GiveAmmo( AmmoType.Satchel, 1000 );
		ply.GiveAmmo( AmmoType.Uranium, 1000 );
		ply.GiveAmmo( AmmoType.Snark, 1000 );

		var battery = new Battery();
		battery.Position = ConsoleSystem.Caller.Pawn.Position;
		battery.Spawn();

		var suit = new Suit();
		suit.Position = ConsoleSystem.Caller.Pawn.Position;
		suit.Spawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();
		DeleteHands();
		RemoveFlashlight();
		if ( HLGame.hl_gamemode == "deathmatch" )
		{
			var coffin = new Coffin();
			coffin.Position = Position + Vector3.Up * 30;
			coffin.Rotation = Rotation;
			coffin.Velocity = Velocity + Rotation.Forward * 100;
			coffin.Populate( this );
		}

		Inventory.DeleteContents();

		if ( LastDamage.Flags.HasFlag( DamageFlags.Blast ) )
		{
			using ( Prediction.Off() )
			{
				HLCombat.CreateGibs( CollisionWorldSpaceCenter, LastDamage.Position, Health, this.CollisionBounds );

			}
		}
		else
		{
			CreateCorpse( Velocity, LastDamage.Flags, LastDamage.Position, LastDamage.Force, GetHitboxBone( LastDamage.HitboxIndex ), this );
		}

		Controller = null;

		CameraMode = new HLDeadCamera();

		EnableAllCollisions = false;
		EnableDrawing = false;

		foreach ( var child in Children.OfType<ModelEntity>() )
		{
			child.EnableDrawing = false;
		}
	}

	public override void BuildInput( InputBuilder input )
	{
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
		base.Simulate( cl );
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
		SimulateFlashlight( cl );
		//
		// Input requested a weapon switch
		//

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
				CameraMode = new HLFirstPersonCamera();
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
	}

	public void Deafen( float strength )
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

		base.StartTouch( other );
	}

	public override void PostCameraSetup( ref CameraSetup setup )
	{
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
		if ( Health == 0 ) return;
		var speed = Velocity.WithZ( 0 ).Length.LerpInverse( 0, 2 );
		var up = setup.Rotation.Up;

		if ( GroundEntity != null )
		{
			walkBob += Time.Delta * 12f * speed;
		}
		setup.Position += up * MathF.Sin( walkBob ) * speed * 2;

		setup.FieldOfView += fov;

	}

	DamageInfo LastDamage;

	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Dead )
			return;

		LastDamage = info;

		// hack - hitbox group 1 is head
		// we should be able to get this from somewhere (it's pretty specific to citizen though?)
		if ( GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			info.Damage *= 2.0f;
		}

		this.ProceduralHitReaction( info );

		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;

		if ( IsServer && Armour > 0 )
		{
			Armour -= info.Damage;

			if ( Armour < 0 )
			{
				info.Damage = Armour * -1;
				Armour = 0;
			}
			else
			{
				info.Damage = 0;
			}
		}

		if ( info.Flags.HasFlag( DamageFlags.Blast ) )
		{
			Deafen( To.Single( Client ), info.Damage.LerpInverse( 0, 60 ) );
		}

		if ( Health > 0 && info.Damage > 0 )
		{
			Health -= info.Damage;
			if ( Health <= 0 )
			{
				Health = 0;
				OnKilled();
			}
		}

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

	TimeSince timeSinceLastFootstep = 0;

	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
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

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) return;
		tr.Surface.DoHLFootstep( this, tr, foot, volume * 5 );
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

		tr.Surface.DoHLJump( this, tr, 1 );
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
