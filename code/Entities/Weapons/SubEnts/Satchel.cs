﻿[Library( "monster_satchel" )]
partial class Satchel : HLMovement
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/satchel.vmdl" );
	public override void Spawn()
	{

		base.Spawn();

		Friction = 0.9f;
		bHeight = 8;
		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		minsOverride = CollisionBounds.Mins;
		maxsOverride = CollisionBounds.Maxs;
	}
	public override void Touch( Entity other )
	{
		PlaySound( "g_bounce" );
		base.StartTouch( other );
	}
	public void Explode()
	{

		Sound.FromWorld( "debris", Position );
		HLExplosion.Explosion( this, Owner, Position, 256, 100, 24.0f, "grenade" );
		Delete();
	}
}
