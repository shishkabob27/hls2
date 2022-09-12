[Library( "weapon_tripmine" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/tripmine.vmdl" )]
[Title(  "Tripmine" ), Category( "Weapons" )]
partial class TripmineWeapon : HLWeapon
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/tripmine.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_tripmine.vmdl";

	public override float PrimaryRate => 100.0f;
	public override float SecondaryRate => 100.0f;
	public override float ReloadTime => 0.0f;
	public override AmmoType AmmoType => AmmoType.Tripmine;
	public override int ClipSize => 1;
	public override int Bucket => 4;
	public override int BucketWeight => 3;
	public override string AmmoIcon => "ui/ammo12.png";

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

        if (Owner is not HLPlayer player) return;

        var owner = Owner as HLPlayer;

        if (owner.TakeAmmo(AmmoType, 1) == 0)
        {
            return;
        }
        // woosh sound
        // screen shake

        Rand.SetSeed( Time.Tick );

		var tr = Trace.Ray( GetFiringPos(), GetFiringPos() + GetFiringRotation().Forward * 150 )
				.Ignore( Owner )
				.Run();

		if ( !tr.Hit )
			return;

		if ( !tr.Entity.IsWorld )
			return;

		if ( IsServer )
		{
			var grenade = new Tripmine
			{
				Position = tr.EndPosition,
				Rotation = Rotation.LookAt( tr.Normal, Vector3.Up ),
				Owner = Owner
			};

			_ = grenade.Arm( 1.0f );
		}

		if ( IsServer && AmmoClip == 0 && player.AmmoCount( AmmoType.Tripmine ) == 0 )
		{
			Delete();
			player.SwitchToBestWeapon();
		}
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 4 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
