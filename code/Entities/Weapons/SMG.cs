﻿[Library( "hl_smg" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/mp5.vmdl" )]
[Title( "SMG" ), Category( "Weapons" )]
partial class SMG : HLWeapon
{
	public static readonly Model WorldModel = Model.Load("models/hl1/weapons/world/mp5.vmdl");
	public override string ViewModelPath => "models/hl1/weapons/view/v_mp5.vmdl";

	public override float PrimaryRate => 10.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 50;
	public override float ReloadTime => 4.0f;
	public override int Bucket => 2;
	public override int BucketWeight => 1;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 20;
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();

			if ( AvailableAmmo() > 0 )
			{
				Reload();
			}
			return;
		}

		(Owner as AnimatedEntity).SetAnimParameter( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "hks" );

		//
		// Shoot the bullets
		//
		ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );

	}

	public override void AttackSecondary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( Owner is not HLPlayer player ) return;

		if ( !TakeAmmo( 10 ) )//Using SMG ammo for now.
		{
			Reload();
			return;
		}

		// woosh sound
		// screen shake
		PlaySound( "glauncher" );
		
		Rand.SetSeed( Time.Tick );

		if ( IsServer )
		{
			var grenade = new SMGGrenade
			{
				Position = Owner.EyePosition + Owner.EyeRotation.Forward * 3.0f,
				Owner = Owner
			};
			
			grenade.PhysicsBody.Velocity = Owner.EyeRotation.Forward * 1000.0f;
			grenade.PhysicsBody.Rotation = Owner.EyeRotation;

			// This is fucked in the head, lets sort this this year
			grenade.CollisionGroup = CollisionGroup.Debris;
			grenade.SetInteractsExclude( CollisionLayer.Player );
			grenade.SetInteractsAs( CollisionLayer.Debris );
		}

		if ( IsServer && AmmoClip == 0 && player.AmmoCount( AmmoType.Grenade ) == 0 )
		{
			Delete();
			player.SwitchToBestWeapon();
		}
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairLastShoot = 0;
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 2 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}

	public override void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var draw = Render.Draw2D;

		var color = Color.Lerp( Color.Red, Color.Yellow, lastReload.LerpInverse( 0.0f, 0.4f ) );
		draw.BlendMode = BlendMode.Lighten;
		draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

		// center circle
		{
			var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.1f, 0.0f ) );
			var length = 2.0f + shootEase * 2.0f;
			draw.Circle( center, length );
		}


		draw.Color = draw.Color.WithAlpha( draw.Color.a * 0.2f );

		// outer lines
		{
			var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.2f, 0.0f ) );
			var length = 3.0f + shootEase * 2.0f;
			var gap = 30.0f + shootEase * 50.0f;
			var thickness = 2.0f;

			draw.Line( thickness, center + Vector2.Up * gap + Vector2.Left * length, center + Vector2.Up * gap - Vector2.Left * length );
			draw.Line( thickness, center - Vector2.Up * gap + Vector2.Left * length, center - Vector2.Up * gap - Vector2.Left * length );

			draw.Line( thickness, center + Vector2.Left * gap + Vector2.Up * length, center + Vector2.Left * gap - Vector2.Up * length );
			draw.Line( thickness, center - Vector2.Left * gap + Vector2.Up * length, center - Vector2.Left * gap - Vector2.Up * length );
		}
	}

}
