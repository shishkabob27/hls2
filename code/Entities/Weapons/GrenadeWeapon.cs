[Library( "weapon_handgrenade" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/grenade.vmdl" )]
[Title( "Grenade" ), Category( "Weapons" )]
partial class GrenadeWeapon : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/grenade.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_grenade.vmdl";

	public override float PrimaryRate => 1.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 0.0f;
	public override AmmoType AmmoType => AmmoType.Grenade;
	public override int ClipSize => 1;
	public override int Bucket => 4;
	public override int BucketWeight => 1;
	public override string AmmoIcon => "ui/ammo9.png";
	public override string InventoryIcon => "/ui/weapons/weapon_grenade.png";


	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 0;
		WeaponIsAmmo = true;
	}
	float prevVRtrig = 0;
	public override bool CanPrimaryAttack()
	{
		var a = false;
		if ( Client.IsUsingVr ) a = Input.VR.RightHand.Trigger == 0 && prevVRtrig != 0;
		if ( Client.IsUsingVr ) prevVRtrig = Input.VR.RightHand.Trigger;
		if ( Client.IsUsingVr ) return a;

		return Input.Released( InputButton.PrimaryAttack );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;


		if ( Owner is not HLPlayer player ) return;

		var owner = Owner as HLPlayer;

		if ( owner.TakeAmmo( AmmoType, 1 ) == 0 )
		{
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
					Position = GetFiringPos() + GetFiringRotation().Forward * 3.0f,
					Owner = Owner
				};

				grenade.PhysicsBody.Velocity = GetFiringRotation().Forward * 600.0f + GetFiringRotation().Up * 200.0f + Owner.Velocity;

				// This is fucked in the head, lets sort this this year
				Tags.Add( "debris" );

				//grenade.CollisionGroup = CollisionGroup.Debris;
				//grenade.SetInteractsExclude( CollisionLayer.Player );
				//grenade.SetInteractsAs( CollisionLayer.Debris );

				_ = grenade.BlowIn( 3.0f );
			}

		player.SetAnimParameter( "b_attack", true );

		player.SetAnimParameter( "attack", true );

		if ( IsServer && player.AmmoCount( AmmoType.Grenade ) == 0 )
		{

			player.SwitchToBestWeapon();
		}

	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.HoldItem ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
