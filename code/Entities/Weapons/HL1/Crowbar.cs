[Library( "weapon_crowbar" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/crowbar.vmdl" )]
[Title( "Crowbar" ), Category( "Weapons" ), MenuCategory( "Half-Life" )]
partial class Crowbar : Weapon
{
	public static Model WorldModel = Model.Load( "models/hl1/weapons/world/crowbar.vmdl" );
	public override string ViewModelPath => "models/hl1/weapons/view/v_crowbar.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/crowbar.vmdl";
	public override float PrimaryRate => 0.5f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 3.0f;
	public override AmmoType AmmoType => AmmoType.None;
	public override int ClipSize => 0;
	public override int Bucket => 0;
	public override int BucketWeight => 1;
	public override bool HasHDModel => true;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair0.png";
	public override string InventoryIcon => "/ui/weapons/weapon_crowbar.png";
	public override string InventoryIconSelected => "/ui/weapons/weapon_crowbar_selected.png";
	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		AmmoClip = 0;
	}

	public override bool CanPrimaryAttack()
	{
		if ( Client.IsUsingVr )
		{
			if ( !Owner.IsValid() || (Input.VR.RightHand.AngularVelocity.pitch >= -600) ) return false;
			//if ( !Owner.IsValid() || !(Input.VR.RightHand.Trigger.Value > 0.2) ) return false;
		}
		else
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.PrimaryAttack ) ) return false;
		}

		var rate = PrimaryRate;
		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > rate;
	}

	Entity hitEntity;
	public override void AttackPrimary()
	{

		// woosh sound
		// screen shake


		Game.SetRandomSeed( Time.Tick );

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

			if ( !Game.IsServer ) continue;
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
			PlaySound( "sounds/hl1/weapons/cbar_miss.sound" );
			TimeSincePrimaryAttack = 0;
		}
		else
		{
			TimeSincePrimaryAttack = 0.25f;
			PlaySound( "sounds/hl1/weapons/cbar_miss.sound" );
			ViewModelEntity?.SetAnimParameter( "attack_has_hit", true );

			if ( hitEntity != this && hitEntity is NPC && Game.IsServer )
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
					PlaySound( "sounds/hl1/weapons/cbar_hitbod.sound" );
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
			else if ( hitEntity is not NPC && Game.IsServer )
			{
				using ( Prediction.Off() )
					PlaySound( "sounds/hl1/weapons/cbar_hit.sound" );
			}
		}


		ViewModelEntity?.SetAnimParameter( "attack", true );
		ViewModelEntity?.SetAnimParameter( "holdtype_attack", false ? 2 : 1 );

		( Owner as AnimatedEntity ).SetAnimParameter( "b_attack", true );
	}

	public override void SimulateAnimator( CitizenAnimationHelper anim )
	{
		SetHoldType( HLCombat.HoldTypes.Swing, anim );
		//anim.SetAnimParameter( "aim_body_weight", 1.0f );

		if ( (Owner as HLPlayer).IsValid() )
		{
			ViewModelEntity?.SetAnimParameter( "b_grounded", (Owner as HLPlayer).GroundEntity.IsValid() );
			ViewModelEntity?.SetAnimParameter( "aim_pitch", (Owner as HLPlayer).EyeRotation.Pitch() );

		}
	}
}
