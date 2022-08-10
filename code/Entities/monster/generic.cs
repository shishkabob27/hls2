/// <summary>
/// Generic Actor
/// </summary>

    [Library( "monster_generic" )]
	[HammerEntity, Model, RenderFields, VisGroup( VisGroup.Physics )]
    [Title(  "monster_generic" ), Category( "Gameplay" )]
	public partial class Prop : BasePhysics
	{
		/// <summary>
		/// If set, the prop will spawn with motion disabled and will act as a nav blocker until broken.
		/// </summary>
		[Property]
		public bool Static { get; set; } = false;

		[Property( "boneTransforms" ), HideInEditor]
		private string BoneTransforms { get; set; }

		public override void Spawn()
		{
			base.Spawn();

			PhysicsEnabled = true;
			UsePhysicsCollision = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			Tags.Add( "prop", "solid" );

			if ( Static )
			{
				PhysicsEnabled = false;
				//Components.Add( new Component.NavBlocker() );
			}
			else
			{
				// Apply any saved bone transforms
				ApplyBoneTransforms();
			}
		}

		private void ApplyBoneTransforms()
		{
			if ( string.IsNullOrWhiteSpace( BoneTransforms ) )
				return;

			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

			var bones = BoneTransforms.Split( ';', StringSplitOptions.RemoveEmptyEntries );
			foreach ( var bone in bones )
			{
				var split = bone.Split( ':', StringSplitOptions.TrimEntries );
				if ( split.Length != 2 )
					continue;

				var boneName = split[0];
				var boneTransform = Transform.Parse( split[1] );

				var body = GetBonePhysicsBody( GetBoneIndex( boneName ) );
				if ( body.IsValid() )
				{
					body.Transform = Transform.ToWorld( boneTransform );
				}
			}
		}

		public override void OnNewModel( Model model )
		{
			base.OnNewModel( model );

			// When a model is reloaded, all entities get set to NULL model first
			if ( model.IsError ) return;

			if ( IsServer )
			{
				UpdatePropData( model );
			}
		}

		protected virtual void UpdatePropData( Model model )
		{
			Host.AssertServer();

			if ( model.TryGetData( out ModelPropData propInfo ) )
			{
				Health = propInfo.Health;
			}

			//
			// If health is unset, set it to -1 - which means it cannot be destroyed
			//
			if ( Health <= 0 )
				Health = -1;
		}

		DamageInfo LastDamage;

		/// <summary>
		/// Fired when the entity gets damaged.
		/// </summary>
		protected Output OnDamaged { get; set; }

		/// <summary>
		/// This prop won't be able to be damaged for this amount of time
		/// </summary>
		public RealTimeUntil Invulnerable { get; set; }

		public override void TakeDamage( DamageInfo info )
		{
			if ( Invulnerable > 0 )
			{
				// We still want to apply forces
				ApplyDamageForces( info );

				return;
			}

			LastDamage = info;

			base.TakeDamage( info );

			// TODO: Add damage type as argument? Or should it be the new health value?
			OnDamaged.Fire( this );
		}

		public override void OnKilled()
		{
			if ( LifeState != LifeState.Alive )
				return;

			LifeState = LifeState.Dead;

			if ( LastDamage.Flags.HasFlag( DamageFlags.PhysicsImpact ) )
			{
				Velocity = lastCollision.This.PreVelocity;
			}

			if ( HasExplosionBehavior() )
			{
				if ( LastDamage.Flags.HasFlag( DamageFlags.Blast ) )
				{
					LifeState = LifeState.Dying;

					// Don't explode right away and cause a stack overflow
					var rand = new Random();
					_ = ExplodeAsync( RandomExtension.Float( rand, 0.05f, 0.25f ) );

					return;
				}
				else
				{
					DoGibs();
					DoExplosion();
					Delete(); // LifeState.Dead prevents this in OnKilled
				}
			}
			else
			{
				DoGibs();
				Delete(); // LifeState.Dead prevents this in OnKilled
			}

			base.OnKilled();
		}

		CollisionEventData lastCollision;

		protected override void OnPhysicsCollision( CollisionEventData eventData )
		{
			lastCollision = eventData;

			base.OnPhysicsCollision( eventData );
		}

		private bool HasExplosionBehavior()
		{
			if ( Model == null || Model.IsError )
				return false;

			return Model.HasData<ModelExplosionBehavior>();
		}

		/// <summary>
		/// Fired when the entity gets destroyed.
		/// </summary>
		protected Output OnBreak { get; set; }

		private void DoGibs()
		{
			var result = new Breakables.Result();
			result.CopyParamsFrom( LastDamage );
			Breakables.Break( this, result );

			// This applies forces from explosive damage to our gibs... But this is already done by DoExplosion, we just need to make sure its called after spawning gibs.
			/*if ( LastDamage.Flags.HasFlag( DamageFlags.Blast ) )
			{
				foreach ( var prop in result.Props )
				{
					if ( !prop.IsValid() )
						continue;

					var body = prop.PhysicsBody;
					if ( !body.IsValid() )
						continue;

					body.ApplyImpulseAt( LastDamage.Position, LastDamage.Force * 25.0f );
				}
			}*/

			OnBreak.Fire( LastDamage.Attacker );
		}

		public async Task ExplodeAsync( float fTime )
		{
			if ( LifeState != LifeState.Alive && LifeState != LifeState.Dying )
				return;

			LifeState = LifeState.Dead;

			await Task.DelaySeconds( fTime );

			DoGibs();
			DoExplosion();

			Delete();
		}

		private void DoExplosion()
		{
			if ( Model == null || Model.IsError )
				return;

			if ( !Model.TryGetData( out ModelExplosionBehavior explosionBehavior ) )
				return;

			var srcPos = Position;
			if ( PhysicsBody.IsValid() ) srcPos = PhysicsBody.MassCenter;
			// Damage and push away all other entities
			if ( explosionBehavior.Radius > 0.0f )
			{
				new ExplosionEntity
				{
					Position = srcPos,
					Radius = explosionBehavior.Radius,
					Damage = explosionBehavior.Damage,
					ForceScale = explosionBehavior.Force,
					ParticleOverride = explosionBehavior.Effect,
					SoundOverride = explosionBehavior.Sound
				}.Explode( this );
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			//Unweld( true );
		}

		/// <summary>
		/// Causes this prop to break, regardless if it is actually breakable or not. (i.e. ignores health and whether the model has gibs)
		/// </summary>
		[Input]
		public void Break()
		{
			OnKilled();
			LifeState = LifeState.Dead;
			Delete();
		}
	}