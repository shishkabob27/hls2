using SandboxEditor;
using System.ComponentModel;

namespace Sandbox;

/// <summary>
/// A procedurally shattering glass panel.
/// </summary>
[Library( "func_breakable" )]
[HammerEntity, Solid]
[PhysicsTypeOverrideMesh]
[Title( "Shatter Glass" ), Category( "Destruction" ), Icon( "wine_bar" )]
public partial class ShatterGlass : ModelEntity
{
	public Vector2 PanelSize { get; private set; }
	[Net] public bool IsBroken { get; set; }
	private Transform PanelTransform;
	public Transform InitialPanelTransform { get; private set; }

	private readonly List<GlassShard> ShardsInternal = new();
	public int NumShards => ShardsInternal.Count;
	public IEnumerable<GlassShard> Shards => ShardsInternal;

	/// <summary>
	/// Thickness of the glass
	/// </summary>
	[Property( "glass_thickness" ), Title( "Glass Thickness" )]
	public float Thickness { get; private set; } = 1.0f;
	public float HalfThickness => Thickness * 0.5f;

	/// <summary>
	/// Material to use for the glass
	/// </summary>
	[Property( "glass_material" ), Title( "Glass Material" ), ResourceType( "vmat" )]
	[Net] public string Material { get; set; } = "materials/glass/sbox_glass.vmat";

	/// <summary>
	/// Material to use for the glass when it is broken. If not set, the material will not change on break.
	/// </summary>
	[Property, Title( "Glass Material When Broken" ), ResourceType( "vmat" )]
	[Net] public string BrokenMaterial { get; set; }

	[Property( "DamagePositioningEntity" ), Title( "Damage Position 01" ), FGDType( "target_destination" )]
	public string DamagePositioningEntity { get; set; }

	[Property( "DamagePositioningEntity02" ), Title( "Damage Position 02" ), FGDType( "target_destination" )]
	public string DamagePositioningEntity02 { get; set; }

	[Property( "DamagePositioningEntity03" ), Title( "Damage Position 03" ), FGDType( "target_destination" )]
	public string DamagePositioningEntity03 { get; set; }

	[Property( "DamagePositioningEntity04" ), Title( "Damage Position 04" ), FGDType( "target_destination" )]
	public string DamagePositioningEntity04 { get; set; }

	[Property( "quad_vertex_a" ), HideInEditor]
	public Vector3 QuadVertexA { get; set; }

	[Property( "quad_vertex_b" ), HideInEditor]
	public Vector3 QuadVertexB { get; set; }

	[Property( "quad_vertex_c" ), HideInEditor]
	public Vector3 QuadVertexC { get; set; }

	[Property( "quad_axis_u" ), HideInEditor]
	public Vector4 QuadAxisU { get; set; }

	[Property( "quad_axis_v" ), HideInEditor]
	public Vector4 QuadAxisV { get; set; }

	[Property( "quad_tex_scale" ), HideInEditor]
	public Vector2 QuadTexScale { get; set; }

	[Property( "quad_tex_size" ), HideInEditor]
	public Vector2 QuadTexSize { get; set; }

	public enum ShatterGlassConstraint
	{
		StaticEdges,
		Physics,
		PhysicsButAsleep
	}

	/// <summary>
	/// Glass constraint.<br/>
	/// <b>Glass with static edges</b> will not be affected by gravity (glass pieces will) and will shatter piece by piece.<br/>
	/// <b>Physics glass</b> is affected by gravity and will shatter all at the same time.<br/>
	/// <b>Physics but asleep</b> is same as physics but will not move on spawn.
	/// </summary>
	[Property]
	public ShatterGlassConstraint Constraint { get; private set; } = ShatterGlassConstraint.StaticEdges;

	[ConCmd.Admin( "glass_reset" )]
	public static void ResetGlassCommand()
	{
		var ents = All.OfType<ShatterGlass>().ToList();
		foreach ( var ent in ents )
		{
			if ( ent.IsBroken )
			{
				ent.Reset();
			}
		}
	}

	/// <summary>
	/// Fired when the panel initially breaks.
	/// </summary>
	public Output OnBreak { get; set; }

	readonly List<Vector3> InitialDamagePositions = new();

	[Event.Entity.PostSpawn, Event.Entity.PostCleanup]
	public void PostSpawn()
	{
		var damageEntity = FindByName( DamagePositioningEntity );
		if ( damageEntity.IsValid() )
		{
			InitialDamagePositions.Add( damageEntity.Position );
		}

		damageEntity = FindByName( DamagePositioningEntity02 );
		if ( damageEntity.IsValid() )
		{
			InitialDamagePositions.Add( damageEntity.Position );
		}

		damageEntity = FindByName( DamagePositioningEntity03 );
		if ( damageEntity.IsValid() )
		{
			InitialDamagePositions.Add( damageEntity.Position );
		}

		damageEntity = FindByName( DamagePositioningEntity04 );
		if ( damageEntity.IsValid() )
		{
			InitialDamagePositions.Add( damageEntity.Position );
		}

		foreach ( var damagePosition in InitialDamagePositions )
		{
			var shard = ShardsInternal.OrderBy( x => (x.Position - damagePosition).Length ).FirstOrDefault();
			if ( shard.IsValid() )
			{
				shard.ShatterWorldSpace( damagePosition );
			}
		}
	}

	public override void Spawn()
	{
		base.Spawn();

		Thickness = Thickness.Clamp( 0.2f, 20.0f );

		var a = Transform.TransformVector( QuadVertexA );
		var b = Transform.TransformVector( QuadVertexB );
		var c = Transform.TransformVector( QuadVertexC );

		var left = b - a;
		var up = b - c;

		PanelSize = new Vector2( left.Length, up.Length );

		var forward = left.Cross( up );
		var rotation = Rotation.LookAt( left.Normal, forward.Normal );

		PanelTransform = Transform.ToLocal( new Transform( CollisionWorldSpaceCenter, rotation ) );
		InitialPanelTransform = GetPanelTransform();

		// Texture params probably haven't been set
		if ( QuadTexScale.IsNearZeroLength )
		{
			QuadTexScale = 0.25f;
			QuadTexSize = 512;
			QuadAxisU = new Vector4( left.Normal, 0 );
			QuadAxisV = new Vector4( up.Normal, 0 );
		}

		SetModel( "" );

		Reset();
	}

	/// <summary>
	/// Cleans up broken shards and creates a new primary shard
	/// </summary>
	[Input]
	public void Reset()
	{
		if ( !IsAuthority )
			return;

		IsBroken = false;

		foreach ( var shard in ShardsInternal )
		{
			shard.MarkForDeletion();
		}

		ShardsInternal.Clear();

		if ( PanelSize.x > 0 && PanelSize.y > 0 )
		{
			var primaryShard = CreateNewShard( null );
			primaryShard.AddVertex( new Vector2( -PanelSize.x * 0.5f, -PanelSize.y * 0.5f ) );
			primaryShard.AddVertex( new Vector2( -PanelSize.x * 0.5f, PanelSize.y * 0.5f ) );
			primaryShard.AddVertex( new Vector2( PanelSize.x * 0.5f, PanelSize.y * 0.5f ) );
			primaryShard.AddVertex( new Vector2( PanelSize.x * 0.5f, -PanelSize.y * 0.5f ) );
			primaryShard.GenerateShardModel();

			if ( Constraint == ShatterGlassConstraint.StaticEdges )
			{
				primaryShard.Freeze();
			}
			else if ( Constraint == ShatterGlassConstraint.PhysicsButAsleep )
			{
				primaryShard.PhysicsBody.Sleeping = true;
			}

			primaryShard.Parent = Parent;
		}
	}

	public GlassShard CreateNewShard( GlassShard parentShard )
	{
		var newShard = new GlassShard
		{
			ParentPanel = this,
			ParentShard = parentShard,
		};

		if ( parentShard != null )
		{
			newShard.StressPosition = parentShard.StressPosition;
		}

		ShardsInternal.Insert( 0, newShard );
		return newShard;
	}

	public void RemoveShard( GlassShard shard )
	{
		ShardsInternal.Remove( shard );
	}

	public Transform GetPanelTransform()
	{
		return Transform.ToWorld( PanelTransform );
	}

	/// <summary>
	/// Breaks the glass at its center.
	/// </summary>
	[Input]
	public void Break()
	{
		if ( !IsAuthority )
			return;

		if ( IsBroken )
			return;

		ShardsInternal.FirstOrDefault()?.ShatterLocalSpace( Vector2.Zero );
	}
}
