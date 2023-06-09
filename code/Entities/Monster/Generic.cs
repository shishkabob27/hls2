[Library( "monster_generic" ), HammerEntity]
[Model]
[Title( "monster_generic" ), Category( "Monsters" ), Icon( "person" )]
public partial class GenericMonster : NPC
{
	// Stub NPC, this does nothing yet

	// [Net, Property, ResourceType( "vmdl" )]
	// public string model { get; set; }

	[Net, Property]
	public int rendermode { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		// SetModel(model);
		Health = 100;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
		//SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
		EnableHitboxes = true;
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		animHelper = new HLAnimationHelper( this );
		Tags.Add( "npc", "playerclip" );



	}
	Vector3 b;

	[GameEvent.Tick.Server]
	public void Ticker()
	{
		
		if ( b != Vector3.Zero ) Position = b;
		//if ( c != -1 ) RenderColor = RenderColor.WithAlpha(c);
		b = Position;
		//c = RenderColor.a;
		if ( rendermode == 5 )
		{
			//Position += new Vector3( Rand.Float( -0.3f, 0.3f ), Rand.Float( -0.3f, 0.3f ), Rand.Float( -0.3f, 0.3f ) );
			if ( Game.Random.Int( 0, 49 ) == 0 )
			{
				var a = Position;
				int axis = Game.Random.Int( 0, 1 );
				if ( axis == 1 ) // Choose between x & z
					axis = 2;
				a[axis] = a[axis] * Game.Random.Float( 1, 1.484f );
				Position = a;
			}
			else if ( Game.Random.Int( 0, 49 ) == 0 )
			{
				var a = Position;
				float offset;
				int axis = Game.Random.Int( 0, 1 );
				if ( axis == 1 ) // Choose between x & z
					axis = 2;
				offset = Game.Random.Int( -10, 10 );
				a[Game.Random.Int( 0, 2 )] += offset;
				Position = a;
			}
		}
	}
	[Event.PreRender]
	public void Render()
	{
		if ( rendermode == 5 )
		{
			var tmp = Position;
			tmp -= Camera.Main.Position;
			var dist = tmp.Dot( Camera.Main.Rotation.Forward );// DotProduct( tmp, RI.refdef.forward );

			var blend = 180.0f;
			var renderAmt = 180.0f;
			if ( dist <= 100 ) blend = renderAmt;
			else blend = (int)((1.0f - (dist - 100) * (1.0f / 400.0f)) * renderAmt);
			blend += Game.Random.Int( -32, 31 );

			RenderColor = RenderColor.WithAlpha( blend / 255.0f );
		}
			 
	}
}
