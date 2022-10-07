[Library( "hl_hornet" )]
[HideInEditor]
partial class ShockBeam : Entity
{

	bool Stuck;

	Particles Trail;

	public override void Spawn()
	{

		PlaySound( "ag_fire" );
		Tags.Clear();
		Tags.Add( "debris" );
		TrailEffect();
	}
	[ClientRpc]
	public void TrailEffect()
	{
		Trail = Particles.Create( "particles/lgtning.vpcf", Position );
		Trail.SetPosition( 1, Position );
		Trail.SetEntity( 0, this );
	}

	[ClientRpc]
	public void DeleteTrailEffect()
	{
		if ( Trail != null ) Trail.Destroy();
	}


	[Event.Tick.Server]
	public virtual void Tick()
	{
		if ( !IsServer )
			return;

		if ( Stuck ){
			DeleteTrailEffect();
			Delete();
		}  

		float Speed = 3000.0f;
		var velocity = Rotation.Forward * Speed;

		var start = Position;
		var end = start + velocity * Time.Delta;

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				//.HitLayer( CollisionLayer.Water, !InWater )
				.Ignore( Owner )
				.Ignore( this )
				.Size( 1.0f )
				.Run();


		if ( tr.Hit )
		{
			// TODO: CLINK NOISE (unless flesh)

			// TODO: SPARKY PARTICLES (unless flesh)

			Stuck = true;
			Position = tr.EndPosition + Rotation.Forward * -1;

			if ( tr.Entity.IsValid() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, tr.Direction * 200, 20 )
													.UsingTraceResult( tr )
													.WithAttacker( Owner )
													.WithWeapon( this )
													.WithFlag( DamageFlags.DoNotGib );

				tr.Entity.TakeDamage( damageInfo );
			}

			// TODO: Parent to bone so this will stick in the meaty heads
			SetParent( tr.Entity, tr.Bone );
			Owner = null;

			//
			// Surface impact effect
			//
			tr.Normal = Rotation.Forward * -1;
			tr.Surface.DoHLBulletImpact( tr );

			// delete self in 60 seconds
			DeleteTrailEffect();
			Delete();
		}
		else
		{
			Position = end;
		}
	}
}
