﻿[Library("weapon_handgrenade"), HammerEntity]
[EditorModel("models/hl1/weapons/world/grenade.vmdl")]
[Title( "Grenade" ), Category( "Weapons" )]
partial class GrenadeWeapon : HLWeapon
{
	public static readonly Model WorldModel = Model.Load("models/hl1/weapons/world/grenade.vmdl");
	public override string ViewModelPath => "models/hl1/weapons/view/v_grenade.vmdl";

	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 0.0f;
	public override AmmoType AmmoType => AmmoType.Grenade;
	public override int ClipSize => 1;
	public override int Bucket => 4;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 1;
	}

	public override bool CanPrimaryAttack()
	{
		return Input.Released( InputButton.PrimaryAttack );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( Owner is not HLPlayer player ) return;

		if ( !TakeAmmo( 1 ) )
		{
			Reload();
			return;
		}

		// woosh sound
		// screen shake

		PlaySound( "dm.grenade_throw" );

		Rand.SetSeed( Time.Tick );


		if ( IsServer )
			using ( Prediction.Off() )
			{
				var grenade = new HandGrenade
				{
					Position = Owner.EyePosition + Owner.EyeRotation.Forward * 3.0f,
					Owner = Owner
				};

				grenade.PhysicsBody.Velocity = Owner.EyeRotation.Forward * 600.0f + Owner.EyeRotation.Up * 200.0f + Owner.Velocity;
		
				// This is fucked in the head, lets sort this this year
				grenade.CollisionGroup = CollisionGroup.Debris;
				grenade.SetInteractsExclude( CollisionLayer.Player );
				grenade.SetInteractsAs( CollisionLayer.Debris );

				_ = grenade.BlowIn( 3.0f );
			}

		player.SetAnimParameter( "b_attack", true );

		Reload();
		player.SetAnimParameter("attack", true);
        
		if ( IsServer && AmmoClip == 0 && player.AmmoCount( AmmoType.Grenade ) == 0 )
		{
			Delete();
			player.SwitchToBestWeapon();
		}
        
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 5 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
