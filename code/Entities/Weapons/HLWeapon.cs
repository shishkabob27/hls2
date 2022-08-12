﻿partial class HLWeapon : BaseWeapon
{
	public virtual AmmoType AmmoType => AmmoType.Pistol;
	public virtual AmmoType AltAmmoType => AmmoType.SMGGrenade;
	public virtual int ClipSize => 16;
	public virtual int AltClipSize => 1; // 1 because all hl1 alts don't have reloadable clips but i'm adding it just in case, yknow for modding and why not.
	public virtual float ReloadTime => 3.0f;
	public virtual float AltReloadTime => 0.0f;

	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;

	public virtual bool HasAltAmmo => false;
	public virtual int Order => (Bucket * 10000) + BucketWeight;

	[Net, Predicted]
	public int AmmoClip { get; set; }

	[Net, Predicted]
	public int AltAmmoClip { get; set; }

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


	public PickupTrigger PickupTrigger { get; protected set; }




	public int AvailableAmmo()
	{
		var owner = Owner as HLPlayer;
		if ( owner == null ) return 0;
		return owner.AmmoCount( AmmoType );
	}
	public int AvailableAltAmmo()
	{
		var owner = Owner as HLPlayer;
		if (owner == null) return 0;
		return owner.AmmoCount(AltAmmoType);
	}
	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;

		IsReloading = false;
	}

	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );

		PickupTrigger = new PickupTrigger();
		PickupTrigger.Parent = this;
		PickupTrigger.Position = Position;
	}

	public override void Reload()
	{
		if ( IsReloading )
			return;

		if ( AmmoClip >= ClipSize )
			return;

		TimeSinceReload = 0;

		if ( Owner is HLPlayer player )
		{
			if ( player.AmmoCount( AmmoType ) <= 0 )
				return;
		}

		IsReloading = true;

		(Owner as AnimatedEntity).SetAnimParameter( "b_reload", true );

		StartReloadEffects();
	}

	public void AltReload()
	{
		if (IsReloadingAlt)
			return;

		if (AltAmmoClip >= AltClipSize)
			return;

		if (Owner is HLPlayer player)
		{
			if (player.AmmoCount(AltAmmoType) <= 0)
				return;
		}

		if ( AltReloadTime == 0.0f) // if it's zero just skip the extra stuff and go straight to a reload
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
		if (IsReloadingAlt && TimeSinceAltReload > AltReloadTime)
		{
			OnAltReloadFinish();
		}        
        
		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			base.Simulate( owner );
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}

		
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

		if (Owner is HLPlayer player)
		{
			var ammo = player.TakeAmmo(AltAmmoType, AltClipSize - AltAmmoClip);
			if (ammo == 0)
				return;

			AltAmmoClip += ammo;
		}
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimParameter( "reload", true );

		// TODO - player third person model reload
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0; // what???? why is secondary reset?
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairLastShoot = 0;

	}

	/// <summary>
	/// Shoot a single bullet
	/// </summary>
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize, int bulletCount = 1 )
	{
		//
		// Seed rand using the tick, so bullet cones match on client and server
		//
		Rand.SetSeed( Time.Tick );

		for ( int i = 0; i < bulletCount; i++ )
		{
			var forward = Owner.EyeRotation.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			//
			// ShootBullet is coded in a way where we can have bullets pass through shit
			// or bounce off shit, in which case it'll return multiple results
			//
			foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 5000, bulletSize ) )
			{
				tr.Surface.DoHLBulletImpact( tr );
				

				if ( tr.Distance > 200 )
				{
					CreateTracerEffect( tr.EndPosition );
				}

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
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
		system?.SetPosition( 1, hitPosition );
	}

	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}

	public bool TakeAltAmmo(int amount)
	{
		if (AltAmmoClip < amount)
			return false;

		AltAmmoClip -= amount;

		return true;
	}

	[ClientRpc]
	public virtual void DryFire()
	{
		PlaySound( "dryfire" );
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new HLViewModel();
		ViewModelEntity.Position = Position;
		ViewModelEntity.Owner = Owner;
		ViewModelEntity.EnableViewmodelRendering = true;
		ViewModelEntity.SetModel( ViewModelPath );
		ViewModelEntity.SetAnimParameter( "deploy", true );
	}

	public override void CreateHudElements()
	{
		if ( Local.Hud == null ) return;
	}

	public bool IsUsable()
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

	protected TimeSince CrosshairLastShoot { get; set; }
	protected TimeSince CrosshairLastReload { get; set; }

	public virtual void RenderHud( in Vector2 screensize )
	{
		var center = screensize * 0.5f;

		if ( IsReloading || (AmmoClip == 0 && ClipSize > 1) )
			CrosshairLastReload = 0;

		RenderCrosshair( center, CrosshairLastShoot.Relative, CrosshairLastReload.Relative );
	}

	public virtual void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var draw = Render.Draw2D;
	}

}
