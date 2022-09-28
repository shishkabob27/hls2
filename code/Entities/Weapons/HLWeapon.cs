﻿partial class HLWeapon : HLMovementCarriable, IRespawnableEntity
{
	[ConVar.Replicated] public static bool hl_sfmmode { get; set; } = false;


	[Net, Predicted]
	public AnimatedEntity VRWeaponModel { get; set; }
	public virtual float PrimaryRate => 5.0f;
	public virtual float SecondaryRate => 15.0f;

	public virtual AmmoType AmmoType => AmmoType.Pistol;
	public virtual AmmoType AltAmmoType => AmmoType.SMGGrenade;
	public virtual int ClipSize => 16;
	public virtual int AltClipSize => 1; // 1 because all hl1 alts don't have reloadable clips but i'm adding it just in case, yknow for modding and why not.
	public virtual float ReloadTime => 3.0f;
	public virtual float AltReloadTime => 0.0f;

	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;

	public virtual bool HasAltAmmo => false;
	public virtual int Order => ( Bucket * 10000 ) + BucketWeight;

	public virtual string AmmoIcon => "ui/ammo1.png";
	public virtual string AltAmmoIcon => "ui/ammo3.png";
	public virtual string InventoryIcon => "/ui/weapons/weapon_error.png";
	public virtual string InventoryIconSelected => "/ui/weapons/weapon_error_selected.png";

	public bool WeaponIsAmmo = false;
	public int WeaponIsAmmoAmount = 1;

	[Net, Predicted]
	public int AmmoClip { get; set; }

	[Net, Predicted]
	public int AltAmmoClip { get; set; }

	[Net, Predicted]
	public TimeSince TimeSincePrimaryAttack { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceSecondaryAttack { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceReload { get; set; }
	[Net, Predicted]
	public TimeSince TimeSinceAltReload { get; set; }

	[Net, Predicted]
	public bool IsReloading { get; set; }
	[Net, Predicted]
	public bool IsReloadingAlt { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	public TouchTrigger PickupTrigger { get; protected set; }

	public virtual void OnPickup()
	{
	}
	public int AvailableAmmo()
	{
		var owner = Owner as HLPlayer;
		if ( owner == null ) return 0;
		return owner.AmmoCount( AmmoType );
	}
	public int AvailableAltAmmo()
	{
		var owner = Owner as HLPlayer;
		if ( owner == null ) return 0;
		return owner.AmmoCount( AltAmmoType );
	}
	public override void ActiveStart( Entity ent )
	{

		EnableDrawing = true;

		if ( ent is Player player )
		{
			var animator = player.GetActiveAnimator();
			if ( animator != null )
			{
				SimulateAnimator( animator );
			}
		}

		//
		// If we're the local player (clientside) create viewmodel
		// and any HUD elements that this weapon wants
		//
		SetModel( ViewModelPath.Replace( "view/v_", "world_player/w_" ) ); //temp solution to change to correct world model when player picks the weapon up

		if ( IsLocalPawn )
		{
			DestroyHudElements();
			DestroyViewModel();
			CreateViewModel();
			CreateHudElements();
		}

		if ( Client.IsUsingVr )
		{

			CreateVRModel();
		}
		TimeSinceDeployed = 0;

		IsReloading = false;
	}

	public async void DeleteIfNotCarriedAfter( float time )
	{
		await GameTask.DelaySeconds( time );
		if ( Owner is not HLPlayer ) Delete();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		//
		// If we're just holstering, then hide us
		//
		if ( !dropped )
		{
			EnableDrawing = false;
		}
		if ( Client.IsUsingVr )
		{
			DestroyVRModel();
		}
		if ( IsClient )
		{
			DestroyViewModel();
			DestroyHudElements();
		}
	}

	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		EnableTouch = false;
		Tags.Add( "weapon" );

		PickupTrigger = new TouchTrigger();
		PickupTrigger.Parent = this;
		PickupTrigger.Position = Position;
	}

	public void Reload()
	{
		if ( IsReloading )
			return;

		if ( AmmoClip >= ClipSize )
			return;

		if ( ClipSize <= 0 )
			return;

		TimeSinceReload = 0;

		if ( Owner is HLPlayer player )
		{
			if ( player.AmmoCount( AmmoType ) <= 0 )
				return;
		}

		IsReloading = true;

		( Owner as AnimatedEntity ).SetAnimParameter( "b_reload", true );

		StartReloadEffects();
	}

	public void AltReload()
	{
		if ( IsReloadingAlt )
			return;

		if ( AltAmmoClip >= AltClipSize )
			return;

		if ( AltClipSize <= 0 )
			return;

		if ( Owner is HLPlayer player )
		{
			if ( player.AmmoCount( AltAmmoType ) <= 0 )
				return;
		}

		if ( AltReloadTime == 0.0f ) // if it's zero just skip the extra stuff and go straight to a reload
		{
			OnAltReloadFinish();
		}
		else
		{
			TimeSinceAltReload = 0;
			IsReloadingAlt = true;
		}
		// Todo, set this somewhere to be used if blah blah is enabled? maybe?
		//(Owner as AnimatedEntity).SetAnimParameter("b_reload", true);

		//StartReloadEffects();
	}

	public override void Simulate( Client owner )
	{
		if ( IsReloadingAlt && TimeSinceAltReload > AltReloadTime )
		{
			OnAltReloadFinish();
		}

		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			if ( CanReload() )
			{
				Reload();
			}

			//
			// Reload could have changed our owner
			//
			if ( !Owner.IsValid() )
				return;

			if ( CanPrimaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSincePrimaryAttack = 0;
					AttackPrimary();
				}
			}

			//
			// AttackPrimary could have changed our owner
			//
			if ( !Owner.IsValid() )
				return;

			if ( CanSecondaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSinceSecondaryAttack = 0;
					AttackSecondary();
				}
			}
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}
		if ( ViewModelEntity != null )
			ViewModelEntity?.Simulate( owner );

	}

	public virtual bool CanReload()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Reload ) ) return false;

		return true;
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;

		if ( Owner is HLPlayer player )
		{
			var ammo = player.TakeAmmo( AmmoType, ClipSize - AmmoClip );
			if ( ammo == 0 )
				return;

			AmmoClip += ammo;
		}
	}

	public virtual void OnAltReloadFinish()
	{
		IsReloadingAlt = false;

		if ( Owner is HLPlayer player )
		{
			var ammo = player.TakeAmmo( AltAmmoType, AltClipSize - AltAmmoClip );
			if ( ammo == 0 )
				return;

			AltAmmoClip += ammo;
		}
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimParameter( "reload", true );
		VRWeaponModel?.SetAnimParameter( "reload", true );
		if ( Owner is HLPlayer player )
		{
			player.SetAnimParameter( "reload", true );
		}
		// TODO - player third person model reload
	}

	public virtual bool CanPrimaryAttack()
	{
		if ( Client.IsUsingVr )
		{
			if ( !Owner.IsValid() || !( Input.VR.RightHand.Trigger.Value > 0.2 ) ) return false;
		}
		else
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.PrimaryAttack ) ) return false;
		}

		var rate = PrimaryRate;
		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > ( 1 / rate );
	}

	public virtual void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0; // what???? why is secondary reset?
	}

	public virtual bool CanSecondaryAttack()
	{
		if ( Client.IsUsingVr )
		{
			if ( !Owner.IsValid() || !( Input.VR.LeftHand.Trigger.Value > 0.2 ) ) return false;
		}
		else
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.SecondaryAttack ) ) return false;
		}


		var rate = SecondaryRate;
		if ( rate <= 0 ) return true;

		return TimeSinceSecondaryAttack > ( 1 / rate );
	}

	public virtual void AttackSecondary()
	{

	}
	protected virtual void ShootEffects( To to )
	{
		ShootEffectsBoth();
		if ( IsServer )
			ShootEffectsSV();
		ShootEffectsRPC( to );
	}
	protected virtual void ShootEffects()
	{
		ShootEffectsBoth();
		if ( IsServer )
			ShootEffectsSV();
		ShootEffectsRPC();
	}
	protected virtual void ShootEffectsSV()
	{
	}
	protected virtual void ShootEffectsBoth()
	{

		VRWeaponModel?.SetAnimParameter( "fire", true );
	}
	[ClientRpc]
	protected virtual void ShootEffectsRPC()
	{

		Host.AssertClient();

		if ( Client.IsUsingVr )
		{
			Particles.Create( "particles/muzflash.vpcf", VRWeaponModel, "muzzle" );
		}
		else
		{
			Particles.Create( "particles/muzflash.vpcf", EffectEntity, "muzzle" );
		}



		ViewModelEntity?.SetAnimParameter( "fire", true );

		if ( Owner is HLPlayer player )
		{
			player.SetAnimParameter( "b_attack", true );
		}
	}

	/// <summary>
	/// This gets the position of where the weapon should fire a bullet
	/// </summary>
	/// <returns>Returns position of the muzzle attachment in VR Mode and Owner.EyePosition elsewhere.</returns>
	public virtual Vector3 GetFiringPos()
	{
		if ( Client.IsUsingVr ) return (Vector3)VRWeaponModel.GetAttachment( "muzzle" )?.Position;
		return Owner.EyePosition;
	}
	/// <summary>
	/// This gets the rotation of where the weapon should fire a bullet
	/// </summary>
	/// <returns>Returns rotation of the muzzle attachment in VR Mode and Owner.EyeRotation elsewhere.</returns>
	public virtual Rotation GetFiringRotation()
	{
		if ( Client.IsUsingVr ) return (Rotation)VRWeaponModel.GetAttachment( "muzzle" )?.Rotation;
		return Owner.EyeRotation;
	}
	public IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		bool underWater = Trace.TestPoint( start, "water" );

		var trace = Trace.Ray( start, end )
				.UseHitboxes()
				.WithAnyTags( "solid", "player", "npc", "glass" )
				.Ignore( this )
				.Size( radius );

		//
		// If we're not underwater then we can hit water
		//
		if ( !underWater )
			trace = trace.WithAnyTags( "water" );

		var tr = trace.Run();

		if ( tr.Hit )
			yield return tr;

		//
		// Another trace, bullet going through thin material, penetrating water surface?
		//
	}


	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize, int bulletCount = 1, bool tracer = true )
	{
		var player = Local.Pawn as HLPlayer;

		if ( Client.IsUsingVr )
		{
			Input.VR.RightHand.TriggerHapticVibration( 0f, 200f, 1.0f );
		}

		//
		// Seed rand using the tick, so bullet cones match on client and server
		//
		Rand.SetSeed( Time.Tick );
		for ( int i = 0; i < bulletCount; i++ )
		{
			var BForward = GetFiringRotation().Forward;
			var BPosition = GetFiringPos();

			BForward += ( Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random ) * spread * 0.25f;
			BForward = BForward.Normal;

			//
			// ShootBullet is coded in a way where we can have bullets pass through shit
			// or bounce off shit, in which case it'll return multiple results
			//
			foreach ( var tr in TraceBullet( BPosition, BPosition + BForward * 5000, bulletSize ) )
			{
				tr.Surface.DoHLBulletImpact( tr );


				if ( tr.Distance > 200 && !hl_sfmmode && tracer )
				{
					CreateTracerEffect( tr.EndPosition );
				}

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, BForward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
				if ( tr.Entity is NPC )
				{
					var trace = Trace.Ray( BPosition, BPosition + BForward * 256 )
					.WorldOnly()
					.Ignore( this )
					.Size( 1.0f )
					.Run();
					if ( ( tr.Entity as NPC ).BloodColour == NPC.BLOOD_COLOUR_RED )
					{
						if ( ResourceLibrary.TryGet<DecalDefinition>( "decals/red_blood.decal", out var decal ) )
						{
							//Log.Info( "Splat!" );
							Decal.Place( decal, trace );
						}
					}
					else
					{
						if ( ResourceLibrary.TryGet<DecalDefinition>( "decals/yellow_blood.decal", out var decal ) )
						{
							//Log.Info( "Splat!" );
							Decal.Place( decal, trace );
						}
					}

				}
			}
		}
	}

	[ClientRpc]
	public void CreateTracerEffect( Vector3 hitPosition )
	{
		// get the muzzle position on our effect entity - either viewmodel or world model
		var pos = EffectEntity.GetAttachment( "muzzle" ) ?? Transform;

		var system = Particles.Create( "particles/tracer.standard.vpcf" );
		system?.SetPosition( 0, pos.Position );
		if ( Client.IsUsingVr ) system?.SetPosition( 0, (Vector3)VRWeaponModel.GetAttachment( "muzzle" )?.Position );
		system?.SetPosition( 1, hitPosition );
	}

	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}

	public bool TakeAltAmmo( int amount )
	{
		if ( AltAmmoClip < amount )
			return false;

		AltAmmoClip -= amount;

		return true;
	}

	[ClientRpc]
	public virtual void DryFire()
	{
		PlaySound( "dryfire" );
	}
	public new virtual Sound PlaySound( string soundName )
	{
		if ( Client.IsUsingVr )
		{
			return Sound.FromEntity( soundName, VRWeaponModel, "muzzle" );
		}
		return Sound.FromEntity( soundName, Owner );
	}
	public void CreateVRModel()
	{

		if ( IsClient ) return;
		VRWeaponModel = new AnimatedEntity();
		VRWeaponModel.Position = Position;
		VRWeaponModel.Owner = Owner;
		VRWeaponModel.SetParent( ( Client.Pawn as HLPlayer ).RightHand, true );
		( Client.Pawn as HLPlayer ).RightHand.RenderColor = Color.Transparent;
		var wmodel = ViewModelPath;
		wmodel = ViewModelPath.Replace( "view/v_", "vr/" ); // get vr model
		var vrmodel = wmodel;
		if ( HLGame.cl_himodels )
		{
			vrmodel = wmodel.Replace( ".vmdl", "_hd.vmdl" );
		}
		VRWeaponModel.SetModel( vrmodel );
		VRWeaponModel.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		VRWeaponModel.Tags.Add( "vrweapon" );
		VRWeaponModel.PhysicsEnabled = true;
		VRWeaponModel.UsePhysicsCollision = true;
		VRWeaponModel.SetAnimParameter( "deploy", true );
	}

	public void DestroyVRModel()
	{
		if ( IsClient ) return;
		VRWeaponModel?.Delete();
		VRWeaponModel = null;
	}
	public override void DestroyViewModel()
	{
		ViewModelEntity?.Delete();
		ViewModelEntity = null;
	}
	public override void CreateViewModel()
	{
		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		if ( !hl_sfmmode )
		{
			ViewModelEntity = new HLViewModel();
			ViewModelEntity.Position = Position;
			ViewModelEntity.Owner = Owner;
			ViewModelEntity.EnableViewmodelRendering = true;
			var wmodel = ViewModelPath;
			if ( HLGame.cl_himodels )
			{
				wmodel = ViewModelPath.Replace( ".vmdl", "_hd.vmdl" ); // get hd model
			}
			ViewModelEntity.SetModel( wmodel );
			ViewModelEntity.SetAnimParameter( "deploy", true );
		}
	}

	public override void CreateHudElements()
	{
		if ( Local.Hud == null ) return;
	}

	public virtual bool IsUsable()
	{
		if ( AmmoClip > 0 ) return true;
		if ( AmmoType == AmmoType.None ) return true;
		return AvailableAmmo() > 0;
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );

		if ( PickupTrigger.IsValid() )
		{
			PickupTrigger.EnableTouch = false;
		}
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );

		if ( PickupTrigger.IsValid() )
		{
			PickupTrigger.EnableTouch = true;
		}
	}
}
