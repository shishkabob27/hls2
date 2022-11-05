[Library("env_sprite")]
[HammerEntity]
[Title("env_sprite"), Category("Effects"), Icon("volume_up")] 
public partial class env_sprite : RenderEntity
{
	[Property( "model" ), Net]
	public string Sprite { get; set; } = "sprites/glow04.vmat";
	public string SpriteActual = "";

	public Material SpriteMaterial;
	public Texture SpriteTex;

	public float SpriteScale { get; set; } = 18f;

	[Property("rendercolor"), Net]
	Color SpriteColour { get; set; }
	public bool Enabled { get; set; } = true;
	[ConVar.Replicated]
	static public bool hl_enable_expermental_sprites { get; set; } = false;
	public override void Spawn()
	{
		base.Spawn();
		//if ( !hl_enable_expermental_sprites && IsServer ) Delete(); return;
		Transmit = TransmitType.Always;
	}
	string SpritePrev;
	public override void DoRender( SceneObject obj )
	{
		if ( !hl_enable_expermental_sprites ) return;
		if ( !Enabled ) return;
		if ( SpriteActual == "")
		{
			SpriteActual = Sprite;
		}
		//if (SpriteMaterial == null || Sprite != SpritePrev)
		{ 
			var a = SpriteActual;
			if (!a.Contains( ".png" ) )
			{
				if ( a.Contains( ".vmdl" ) ) a = a.Replace( ".vmdl", ".png" );
				if ( a.Contains( ".vmat" ) ) a = a.Replace( ".vmat", ".png" );
			}
			if ( !a.Contains( "materials/" ) )
			{
				a = a.Replace( "sprites/", "materials/hl1/sprites/" );
			} 
			//Log.Info( a );

			SpriteMaterial = Material.FromShader( "envsprite.vfx" ); //Material.Load( a ); 
			SpriteTex = Texture.Load( FileSystem.Mounted, a, false);                                    //Log.Info();
		}
		SpriteMaterial.OverrideTexture( "Color", SpriteTex );
		// Allow lights to affect the sprite
		//Render.SetupLighting( obj );
		Graphics.SetupLighting( obj );  

		// Create the vertex buffer for the sprite
		var vb = new VertexBuffer();
		vb.Init(true);

		// Vertex buffers are in local space, so we need the camera position in local space too
		var normal = CurrentView.Rotation.Backward;// Transform.PointToLocal( CurrentView.Position ).Normal;
		var w = normal.Cross( Vector3.Up ).Normal;
		var h = normal.Cross( w ).Normal;
		float halfSpriteSize = SpriteScale;

		// Add a single quad to our vertex buffer
		vb.AddQuad( new Ray( default, normal), w * halfSpriteSize, h * halfSpriteSize );

		// Draw the sprite
		Graphics.Attributes.Set( "rendercolor", SpriteColour );
		vb.Draw( SpriteMaterial );
		SpritePrev = Sprite;
	}
}
