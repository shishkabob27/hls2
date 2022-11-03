[Library("env_sprite")]
[HammerEntity]
[Title("env_sprite"), Category("Effects"), Icon("volume_up")]
public partial class env_sprite : RenderEntity
{
	[Property( "model" ), Net]
	public string Sprite { get; set; } = "materials/dev/target_circle.vmat";
	public Material SpriteMaterial { get; set; } 

	public float SpriteScale { get; set; } = 18f;

	public bool Enabled { get; set; } = true;
	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}
	public override void DoRender( SceneObject obj )
	{
		if ( !Enabled ) return;
		if (SpriteMaterial == null)
		{
			var a = Sprite;
			a = a.Replace( ".vmdl", ".vmat" );
			a = a.Replace( "sprites/", "materials/hl1/sprites/" );
			Log.Info( a );
			SpriteMaterial = Material.Load( a );
		}
		// Allow lights to affect the sprite
		//Render.SetupLighting( obj );
		Graphics.SetupLighting( obj );

		// Create the vertex buffer for the sprite
		var vb = new VertexBuffer();
		vb.Init(true);

		// Vertex buffers are in local space, so we need the camera position in local space too
		var normal = CurrentView.Rotation.Backward;// Transform.PointToLocal( CurrentView.Position ).Normal;
		var w = normal.Cross( Vector3.Down ).Normal;
		var h = normal.Cross( w ).Normal;
		float halfSpriteSize = SpriteScale / 2;

		// Add a single quad to our vertex buffer
		vb.AddQuad( new Ray( default, normal), w * halfSpriteSize, h * halfSpriteSize );

		// Draw the sprite
		vb.Draw( SpriteMaterial );
	}
}
