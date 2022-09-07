[Library( "hl_rpg_rocket" )]
[HideInEditor]
partial class RPGRocket : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/rpg_rocket.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}


	[Event.Tick.Server]
	public virtual void Tick()
	{
		if ( !IsServer )
			return;

		float Speed = 2000.0f;
		var velocity = Rotation.Forward * Speed;

		var start = Position;
		var end = start + velocity * Time.Delta;

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				.Ignore( Owner )
				.Ignore( this )
				.Size( 1.0f )
				.Run();


		if ( tr.Hit )
		{
			BlowUp();
		}
		else
		{
			Position = end;
		}
	}

	public void BlowUp()
	{
		HLExplosion.Explosion( this, Owner, Position, 250, 100, 24.0f, "grenade");
		Delete();
	}
}
