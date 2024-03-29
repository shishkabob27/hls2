public partial class HLExplosion
{
	public static void Explosion( Entity weapon, Entity owner, Vector3 position, float radius, float damage, float forceScale, string type, bool sparkshower = true )
	{
		// Effects
		Sound.FromWorld( "explode", position );
		ExplosionParticle( position, type, sparkshower );
		// Damage, etc
		var overlaps = Entity.FindInSphere( position, radius );

		foreach ( var overlap in overlaps )
		{
			if ( overlap is not ModelEntity ent || !ent.IsValid() )
				continue;

			if ( ent.LifeState != LifeState.Alive )
				continue;

			if ( !ent.PhysicsBody.IsValid() )
				continue;

			if ( ent.IsWorld )
				continue;

			var targetPos = ent.PhysicsBody.MassCenter;

			var dist = Vector3.DistanceBetween( position, targetPos );
			if ( dist > radius )
				continue;

			var tr = Trace.Ray( position, targetPos )
				.Ignore( weapon )
				.WorldOnly()
				.Run();

			if ( tr.Fraction < 0.98f )
				continue;

			var distanceMul = 1.0f - Math.Clamp( dist / radius, 0.0f, 1.0f );
			var dmg = damage * distanceMul;
			var force = (forceScale * distanceMul) * ent.PhysicsBody.Mass;
			force /= 2;
			var forceDir = (targetPos - position).Normal;

			var damageInfo = DamageInfo.FromExplosion( position, forceDir * force, dmg )
				.WithWeapon( weapon )
				.WithAttacker( owner );

			ent.TakeDamage( damageInfo );
			if ( ent is ModelEntity md && md.PhysicsBody != null )
			{
				//tr.Body.ApplyForceAt( tr.EndPosition, BForward * (2000000 * force) );
				md.PhysicsBody.ApplyForceAt( tr.EndPosition, forceDir * (150000 * force) );
			}
		}
	}

	[ClientRpc]
	public static void ExplosionParticle( Vector3 position, string type, bool sparkshower = true )
	{

		if ( HLGame.hl_classic_explosion )
		{
			switch ( type )
			{
				case "grenade": Particles.Create( "particles/explosion.vpcf", position ); break;
				case "tripmine": Particles.Create( "particles/explosion_tripmine.vpcf", position ); break;
				case "electro": Particles.Create( "particles/gauss_impact.vpcf", position ); break;
				default: Particles.Create( "particles/explosion.vpcf", position ); break;
			}
		}
		else
		{
			switch ( type )
			{
				case "grenade": Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", position ); break;
				case "tripmine": Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", position ); break;
				case "electro": Particles.Create( "particles/gauss_impact.vpcf", position ); break;
				default: Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", position ); break;
			}
		}
		if ( sparkshower )
		{
			Vector3 offset = new Vector3( 0, 0, 10 );
			int randSparks = Game.Random.Int( 1, 3 );
			for ( int i = 0; i < randSparks; i++ )
			{
				var a = new Sparkshower();
				a.Position = position + offset;
			}
		}
		// TODO: Sparkshower.
	}
}
