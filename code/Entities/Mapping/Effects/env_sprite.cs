[Library( "env_sprite" )]
[HammerEntity]
[Title( "env_sprite" ), Category( "Effects" ), Icon( "volume_up" )]
public partial class env_sprite : RenderEntity
{
	[Flags]
	public enum Flags
	{
		Starton = 1,
		PlayOnce = 2,
	}
	[Property( "spawnflags", Title = "Spawn Settings" ), Net]
	public Flags SpawnSettings { get; set; } = Flags.Starton;

	[Property( "model" ), Net]
	public string Sprite { get; set; } = "";
	public string SpriteActual = "";

	[Property( "framerate" ), Net]
	public float Framerate { get; set; } = 10;

	public Material SpriteMaterial;
	public Texture SpriteTex;

	public float SpriteScale { get; set; } = 18f;

	[Property( "scale" ), Net]
	public float SpriteMainScale { get; set; } = 1;

	[Property( "rendercolor" ), Net]
	Color SpriteColour { get; set; }
	[Net]
	public bool Enabled { get; set; } = false;
	public bool Animated { get; set; } = false;
	[ConVar.Replicated]
	static public bool hl_enable_expermental_sprites { get; set; } = true;
	public override void Spawn()
	{
		base.Spawn();
		if ( SpawnSettings.HasFlag( Flags.Starton ) ) Enabled = true;
		//if ( !hl_enable_expermental_sprites && IsServer ) Delete(); return;
		Transmit = TransmitType.Always;
	}
	string SpritePrev;
	string a;
	string b;
	int frame = 1;
	bool playOnceHasLooped = false;
	TimeSince SinceFrame;
	public override void DoRender( SceneObject obj )
	{
		if ( !hl_enable_expermental_sprites ) return;
		if ( !Enabled ) return;
		if ( playOnceHasLooped ) return;

		if ( SpriteActual == "" )
		{
			SpriteActual = Sprite;
		}
		if ( !Animated )//if (SpriteMaterial == null || Sprite != SpritePrev)
		{
			a = SpriteActual;
			if ( !a.Contains( ".png" ) )
			{
				if ( a.Contains( ".vmdl" ) ) a = a.Replace( ".vmdl", ".png" );
				if ( a.Contains( ".vmat" ) ) a = a.Replace( ".vmat", ".png" );
			}
			if ( !a.Contains( "materials/" ) )
				/*
				{
					a = a.Replace( "sprites/", "materials/hl1/sprites/" );
				} 
				*/
				//Log.Info( a );

				SpriteMaterial = Material.FromShader( "envsprite.shader" ); //Material.Load( a ); 
			SpriteTex = Texture.Load( FileSystem.Mounted, a, false );
			if ( SpriteTex == null && !Animated )
			{
				var b = a.Replace( ".png", "001.png" );
				SpriteTex = Texture.Load( FileSystem.Mounted, b, false );
				//Log.Info( b );
				if ( SpriteTex != null )
				{
					Animated = true;
					frame++;
					SinceFrame = 0;
				}
			}//Log.Info();
		}
		if ( Animated )
		{
			// TODO: OPTIMISE ME PLEASE STOP LOADING EVERY FRAME
			b = a.Replace( ".png", frame.ToString( "000" ) + ".png" );
			SpriteTex = Texture.Load( FileSystem.Mounted, b, false );
			if ( SpriteTex == null )
			{

				if ( SpawnSettings.HasFlag( Flags.PlayOnce ) )
				{
					playOnceHasLooped = true;
					Enabled = false;
					return;

				}
				else
				{
					frame = 1;
				}
				SinceFrame = 0;
				b = a.Replace( ".png", frame.ToString( "000" ) + ".png" );
				SpriteTex = Texture.Load( FileSystem.Mounted, b, false );

			}
			if ( SinceFrame > (1 / Framerate) )
			{
				if ( !playOnceHasLooped )
				{
					frame++;
					SinceFrame = 0;
				}
			}
		}
		SpriteMaterial.Set( "Color", SpriteTex );
		SpriteMaterial.Set( "g_vColorTint", SpriteColour );
		SpriteMaterial.Set( "g_flTintColor", SpriteColour );
		// Allow lights to affect the sprite
		//Render.SetupLighting( obj );
		Graphics.SetupLighting( obj );

		// Create the vertex buffer for the sprite
		var vb = new VertexBuffer();
		vb.Init( true );

		// Vertex buffers are in local space, so we need the camera position in local space too
		var normal = Camera.Rotation.Backward;// Transform.PointToLocal( CurrentView.Position ).Normal;
		var w = normal.Cross( Vector3.Up ).Normal;
		var h = normal.Cross( w ).Normal;
		float halfSpriteSize = SpriteScale;

		// Add a single quad to our vertex buffer
		vb.AddQuad( new Ray( default, normal ), w * (((SpriteTex == null) ? halfSpriteSize : SpriteTex.Width) * SpriteMainScale), h * (((SpriteTex == null) ? halfSpriteSize : SpriteTex.Height) * SpriteMainScale) );

		// Draw the sprite
		vb.Draw( SpriteMaterial );
		SpritePrev = Sprite;
	}

	/// <summary>
	/// Enables the entity.
	/// </summary>
	[Input]
	public void ShowSprite()
	{
		Enabled = true;
		SpriteRPC( Enabled );
	}

	/// <summary>
	/// Disables the entity, so that it would not fire any outputs.
	/// </summary>
	[Input]
	public void HideSprite()
	{
		Enabled = false;
		SpriteRPC( Enabled );
	}

	/// <summary>
	/// Toggles the enabled state of the entity.
	/// </summary>
	[Input]
	public void ToggleSprite()
	{
		Enabled = !Enabled;
		SpriteRPC( Enabled );
	}
	[ClientRpc]
	public void SpriteRPC( bool bl )
	{
		Enabled = bl;
		if ( Enabled )
		{
			frame = 1;
			SinceFrame = 0;
			playOnceHasLooped = false;
		}
	}
}
