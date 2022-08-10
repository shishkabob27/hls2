[Library( "hl_crowbar" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/crowbar.vmdl" )]
[Title(  "Crowbar" ), Category( "Weapons" )]
partial class Crowbar : HLWeapon
{
	public static Model WorldModel = Model.Load( "models/hl1/weapons/world/crowbar.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_crowbar.vmdl";

	public override float PrimaryRate => 2.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 3.0f;
	public override AmmoType AmmoType => AmmoType.None;
	public override int ClipSize => 0;
	public override int Bucket => 0;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 0;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack();
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		// woosh sound
		// screen shake
		PlaySound( "cbar" );

		Rand.SetSeed( Time.Tick );

		var forward = Owner.EyeRotation.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * 0.1f;
		forward = forward.Normal;

		foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 70, 15 ) )
		{
			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 32, 25 )
				.UsingTraceResult( tr )
				.WithAttacker( Owner )
				.WithWeapon( this );

			tr.Entity.TakeDamage( damageInfo );
		}
		ViewModelEntity?.SetAnimParameter( "attack_has_hit", true );
		ViewModelEntity?.SetAnimParameter( "attack", true );
		ViewModelEntity?.SetAnimParameter( "holdtype_attack", false ? 2 : 1 );
		if ( Owner is HLPlayer player )
		{
			player.SetAnimParameter( "attack", true );
		}
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 5 ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );

		if ( Owner.IsValid() )
		{
			ViewModelEntity?.SetAnimParameter( "b_grounded", Owner.GroundEntity.IsValid() );
			ViewModelEntity?.SetAnimParameter( "aim_pitch", Owner.EyeRotation.Pitch() );

		}
	}
}
