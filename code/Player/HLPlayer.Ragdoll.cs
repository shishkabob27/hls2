public partial class HLPlayer
{
	// TODO - make ragdolls one per entity
	// TODO - make ragdolls dissapear after a load of seconds


	[ClientRpc]
	private void BecomeRagdollOnClient(Vector3 velocity, DamageFlags damageFlags, Vector3 forcePos, Vector3 force, int bone)
	{
		var ent = new ModelEntity();
		ent.Tags.Add("ragdoll", "solid", "debris");
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.Scale = Scale;
		ent.UsePhysicsCollision = true;
		ent.EnableAllCollisions = true;
		ent.SetModel(GetModelName());
		ent.CopyBonesFrom(this);
		ent.CopyBodyGroups(this);
		ent.CopyMaterialGroup(this);
		ent.CopyMaterialOverrides(this);
		ent.TakeDecalsFrom(this);
		ent.EnableAllCollisions = true;
		ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		ent.RenderColor = RenderColor;
		ent.PhysicsGroup.Velocity = velocity;
		ent.PhysicsEnabled = true;

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

		Corpse = ent;

		ent.DeleteAsync(10.0f);
	}
}
