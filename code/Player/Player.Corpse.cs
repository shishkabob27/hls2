public partial class HLPlayer
{
	// TODO - make ragdolls one per entity
	// TODO - make ragdolls dissapear after a load of seconds



	[ClientRpc]
	private void CreateCorpse( Vector3 velocity, DamageFlags damageFlags, Vector3 forcePos, Vector3 force, int bone, Entity owner )
	{
		var ent = new AnimatedEntity();
		var c = ent.Components.Create<Movement>();
		ent.Parent = owner;
		ent.EnableHideInFirstPerson = true;
		ent.UseAnimGraph = false;
		ent.Tags.Add( "debris" );
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.Scale = Scale;
		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.CopyBodyGroups( this );
		ent.CopyMaterialGroup( this );
		ent.CopyMaterialOverrides( this );
		ent.TakeDecalsFrom( this );
		ent.RenderColor = RenderColor;
		ent.Velocity = velocity;
		List<string> DeathAnimList = new List<string>{
		"headshot",
		"gutshot",
		"die_simple",
		"die_forwards",
		"die_backwards",
		"die_backwards1",
		"die_spin",
		};
		ent.CurrentSequence.Name = Rand.FromList<string>( DeathAnimList );
		ent.SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
		c.Friction = 3;
		ent.Spawn();
		//ent.PhysicsEnabled = true;
		/*
		if (damageFlags.HasFlag(DamageFlags.Bullet) ||
			 damageFlags.HasFlag(DamageFlags.PhysicsImpact))
		{
			PhysicsBody body = bone > 0 ? ent.GetBonePhysicsBody(bone) : null;

			if (body != null)
			{
				body.ApplyImpulseAt(forcePos, force * body.Mass);
			}
			else
			{
				ent.PhysicsGroup.ApplyImpulse(force);
			}
		}

		if (damageFlags.HasFlag(DamageFlags.Blast))
		{
			if (ent.PhysicsGroup != null)
			{
				ent.PhysicsGroup.AddVelocity((Position - (forcePos + Vector3.Down * 100.0f)).Normal * (force.Length * 0.2f));
				var angularDir = (Rotation.FromYaw(90) * force.WithZ(0).Normal).Normal;
				ent.PhysicsGroup.AddAngularVelocity(angularDir * (force.Length * 0.02f));
			}
		}
		*/

		Corpse = ent;

		ent.DeleteAsync( 5.0f );
	}
}
