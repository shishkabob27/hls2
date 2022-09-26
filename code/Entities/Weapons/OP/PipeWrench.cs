[Library( "weapon_pipewrench" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/pipe_wrench.vmdl" )]
[Title( "weapon_pipewrench" ), Category( "Weapons" )]
partial class PipeWrench : HLWeapon
{
	public static Model WorldModel = Model.Load( "models/hl1/weapons/world/pipe_wrench.vmdl" );
	public override string ViewModelPath => "models/op/weapons/view/v_pipe_wrench.vmdl";

	public override float PrimaryRate => 1.33f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 3.0f;
	public override AmmoType AmmoType => AmmoType.None;
	public override int ClipSize => 0;
	public override int Bucket => 0;

	public override string InventoryIcon => "/ui/weapons/op/weapon_pipewrench.png";
	public override string InventoryIconSelected => "/ui/weapons/op/weapon_pipewrench_selected.png";
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

	Entity hitEntity;
	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		// woosh sound
		// screen shake

		// i didn't change anything i just really wanted to make the commit joke 

		Rand.SetSeed( Time.Tick );

		var forward = GetFiringRotation().Forward;
		forward += ( Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random ) * 0.1f;
		forward = forward.Normal;

		hitEntity = this;
		bool didHit = false;
		var endPos = Vector3.Zero;
		var trNormal = Vector3.Zero;
		foreach ( var tr in TraceBullet( GetFiringPos(), GetFiringPos() + forward * 70, 1 ) )
		{

			tr.Surface.DoHLBulletImpact( tr, false );
			if ( tr.Hit )
			{
				didHit = true;
			}
			else
			{
				didHit = false;
			}

			if ( !IsServer ) continue;
			if ( !tr.Entity.IsValid() ) continue;

			var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 32, 10 )
				.UsingTraceResult( tr )
				.WithAttacker( Owner )
				.WithWeapon( this );

			tr.Entity.TakeDamage( damageInfo );

			hitEntity = tr.Entity;
			endPos = tr.EndPosition;
			trNormal = tr.Normal;

		}




		ViewModelEntity?.SetAnimParameter( "attack_has_hit", false );


		if ( !didHit )
		{
			PlaySound( "sounds/op/weapons/pwrench_miss.sound" );
		}
		else
		{
			PlaySound( "sounds/op/weapons/pwrench_miss.sound" );
			TimeSincePrimaryAttack = 0.26f;
			ViewModelEntity?.SetAnimParameter( "attack_has_hit", true );

			if ( hitEntity != this && hitEntity is NPC && IsServer )
			{
				// recreate that funny glitch :)
				if ( hitEntity.LifeState == LifeState.Dead )
					TimeSincePrimaryAttack = 5f;

				var trace = Trace.Ray( GetFiringPos(), GetFiringPos() + forward * 70 * 2 )
					.WorldOnly()
					.Ignore( this )
					.Size( 1.0f )
					.Run();
				if ( ( hitEntity as NPC ).BloodColour == NPC.BLOOD_COLOUR_RED )
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

				using ( Prediction.Off() )
				{
					PlaySound( "sounds/op/weapons/pwrench_hitbod.sound" );
					if ( ( hitEntity as NPC ).BloodColour == NPC.BLOOD_COLOUR_RED )
					{

						var ps = Particles.Create( "particles/hlimpact_blood.vpcf", endPos );
					}
					else
					{

						var ps = Particles.Create( "particles/hlimpact_blood_yellow.vpcf", endPos );
					}
					//ps.SetForward(0, trNormal);
					//ps.SetPosition(0, endPos);
				}
			}
			else if ( hitEntity is not NPC && IsServer )
			{
				using ( Prediction.Off() )
					PlaySound( "sounds/op/weapons/pwrench_hit.sound" );
			}
		}


		ViewModelEntity?.SetAnimParameter( "attack", true );
		ViewModelEntity?.SetAnimParameter( "holdtype_attack", false ? 2 : 1 );

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Swing ); // TODO this is shit
		anim.SetAnimParameter( "aim_body_weight", 1.0f );

		if ( Owner.IsValid() )
		{
			ViewModelEntity?.SetAnimParameter( "b_grounded", Owner.GroundEntity.IsValid() );
			ViewModelEntity?.SetAnimParameter( "aim_pitch", Owner.EyeRotation.Pitch() );

		}
	}
}
