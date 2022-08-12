using System;

namespace Sandbox
{
	/// <summary>
	/// A procedurally shattering glass shard.
	/// </summary>
	[Library( "glass_shard" )]
	[HideInEditor]
	[Title( "Glass Shard" ), Icon( "play_arrow" ), Category( "Glass Shards" )]
	public partial class GlassShard : ModelEntity
	{
		[Net] public ShatterGlass ParentPanel { get; set; }
		[Net] private ModelDesc Desc { get; set; }

		public Vector2 StressPosition { get; set; }
		public GlassShard ParentShard { get; set; }

		private readonly List<Vector2> PanelVertices = new();
		private Vector2 LocalPanelSpaceOrigin;

		public Vector3 StressVelocity { get; set; }

		private int ShatterTypeIndex = 1;
		private bool IsModelGenerated;
		private bool IsMarkedForDeletion;

		private enum OnFrame : byte
		{
			Unknown = 0,
			True,
			False,
		};

		private OnFrame OnFrameEdge = OnFrame.Unknown;

		public override void ClientSpawn()
		{
			base.ClientSpawn();

			TryGenerateShardModel();
		}

		[Event.Tick.Client]
		protected void ClientTick()
		{
			TryGenerateShardModel();
		}

		[Event.Tick.Server]
		protected void OnServerTick()
		{
			if ( IsMarkedForDeletion && this.IsValid() )
			{
				Delete();
				IsMarkedForDeletion = false;
			}
		}

		public void MarkForDeletion()
		{
			PhysicsEnabled = false;
			EnableAllCollisions = false;
			EnableDrawing = false;
			IsMarkedForDeletion = true;
		}

		protected void TryGenerateShardModel()
		{
			if ( IsModelGenerated )
				return;

			if ( !ParentPanel.IsValid() || string.IsNullOrEmpty( ParentPanel.Material ) )
				return;

			var model = GenerateShardModel( out LocalPanelSpaceOrigin );

			if ( model != null )
			{
				Model = model;
			}

			IsModelGenerated = true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if ( ParentPanel.IsValid() )
			{
				ParentPanel.RemoveShard( this );
			}
		}

		protected override void OnPhysicsCollision( CollisionEventData eventData )
		{
			var other = eventData.Other;

			if ( other.Entity.IsWorld )
				return;

			if ( !other.Entity.PhysicsGroup.IsValid() )
				return;

			other.Entity.PhysicsGroup.Velocity = other.PreVelocity;
			other.Entity.PhysicsGroup.AngularVelocity = other.PreAngularVelocity;
		}

		public override void TakeDamage( DamageInfo info )
		{
			base.TakeDamage( info );

			var position = info.Position;
			var plane = new Plane( Position, Rotation.Up );
			var hit = plane.Trace( new Ray( info.Position, info.Force.Normal ), true );

			if ( hit.HasValue )
			{
				position = hit.Value;
			}

			if ( info.Flags.HasFlag( DamageFlags.Bullet ) )
			{
				ShatterTypeIndex = 1;
			}
			else if ( info.Flags.HasFlag( DamageFlags.PhysicsImpact ) )
			{
				if ( !ParentPanel.IsValid() || ParentPanel.IsBroken )
					return;

				ShatterTypeIndex = 0;
				info.Force /= 10;
			}
			else if ( info.Flags.HasFlag( DamageFlags.Blast ) )
			{
				ShatterTypeIndex = 2;
			}
			else
			{
				return;
			}

			var localPosition = TransformPosWorldToPanel( position );

			if ( ParentPanel.IsValid() )
			{
				var shards = ParentPanel.Shards.ToList();

				foreach ( var shard in shards )
				{
					if ( !shard.IsValid() )
						continue;

					shard.StressVelocity = info.Force;

					if ( shard.ShatterLocalSpace( localPosition ) )
					{
						break;
					}
				}
			}
			else
			{
				StressVelocity = info.Force;

				ShatterLocalSpace( localPosition );
			}
		}

		public void AddVertex( Vector2 vertex )
		{
			PanelVertices.Add( vertex );
		}

		void ScaleVerts( float scale )
		{
			if ( scale <= 0.0f )
				return;

			var average = GetPanelVertexAverage();

			for ( int i = 0; i < PanelVertices.Count; ++i )
			{
				PanelVertices[i] = Vector2.Lerp( average, PanelVertices[i], scale );
			}
		}

		public Vector2 GetPanelVertexAverage()
		{
			var vecAverageVertPosition = Vector2.Zero;

			if ( PanelVertices.Count > 0 )
			{
				foreach ( var vertex in PanelVertices )
				{
					vecAverageVertPosition += vertex;
				}

				vecAverageVertPosition /= PanelVertices.Count;
			}

			return vecAverageVertPosition;
		}

		public void TryGenerateShardModel( Vector3 vShatterPos, Vector3 vShatterVel, bool bVelocity, bool bFreeze = false )
		{
			if ( !CanGenerateShardModel() || !GenerateShardModel() )
			{
				Delete();

				return;
			}

			bool bApplyVelocity = bVelocity && !bFreeze;

			if ( bFreeze )
			{
				if ( IsOnFrameEdge() && IsShardNearPanelSpacePosition() )
				{
					Freeze();
				}
				else
				{
					bApplyVelocity = true;
				}
			}

			if ( !IsFrozen() && !IsABigPiece() )
			{
				DeleteAsync( 5.0f );
			}

			if ( bApplyVelocity )
			{
				var overallVel = vShatterVel * PhysicsBody.Mass;

				// Scale by distance from impact
				var smallestAxis = Math.Min( ParentPanel.PanelSize.x, ParentPanel.PanelSize.y );
				var scale = Math.Max( 0f, smallestAxis - vShatterPos.Distance( WorldSpaceBounds.Center ) ) / smallestAxis;
				overallVel *= scale;

				// Apply majority of velocity to center of mass, so pieces don't just go spinning around like crazy
				PhysicsBody.ApplyImpulse( overallVel * 0.90f );
				PhysicsBody.ApplyImpulseAt( vShatterPos, overallVel * 0.10f );
			}
		}

		public bool GenerateShardModel()
		{
			if ( PanelVertices.Count < 3 )
				return false;

			Desc = new ModelDesc()
			{
				PanelSize = ParentPanel.PanelSize,
				StressPosition = StressPosition,
				PanelVertices = PanelVertices.ToArray(),
				HalfThickness = ParentPanel.HalfThickness,
				IsParentFrozen = ParentShard.IsValid() && ParentShard.IsFrozen(),
				HasParent = ParentShard.IsValid(),
				TextureOffset = new Vector2( ParentPanel.QuadAxisU.w, ParentPanel.QuadAxisV.w ),
				TextureScale = ParentPanel.QuadTexScale,
				TextureSize = ParentPanel.QuadTexSize,
				TextureAxisU = ParentPanel.QuadAxisU,
				TextureAxisV = ParentPanel.QuadAxisV,
				PanelTransform = ParentPanel.InitialPanelTransform,
			};

			Desc.WriteNetworkData();

			var model = GenerateShardModel( out LocalPanelSpaceOrigin );

			Transform = GetSpawnTransform();

			Model = model;
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

			Unfreeze();

			if ( !ParentShard.IsValid() || IsABigPiece() )
			{
				EnableTouch = false;
				Tags.Remove( "glass" );
			}

			return true;
		}

		public override void Touch( Entity other )
		{
			base.Touch( other );

			if ( !IsAuthority )
				return;

			if ( other.IsWorld )
				return;

			if ( !ParentPanel.IsBroken )
				return;

			if ( other is GlassShard shard && shard.ParentPanel == ParentPanel )
				return;

			if ( !IsFrozen() )
				return;

			Unfreeze();

			StressVelocity = other.Velocity;

			if ( CalculateArea() > 800.0f )
			{
				ShatterTypeIndex = 0;
				ShatterLocalSpace( GetPanelVertexAverage() );
			}
			else
			{
				PhysicsBody.ApplyImpulse( StressVelocity * PhysicsBody.Mass );
				DeleteAsync( 5.0f );
			}
		}

		public void Freeze()
		{
			PhysicsEnabled = false;
			PhysicsBody.BodyType = PhysicsBodyType.Keyframed;

			if ( !ParentShard.IsValid() || IsABigPiece() )
			{
				EnableTouch = false;
				Tags.Remove( "glass" );
			}
			else
			{
				EnableTouch = true;
				Tags.Add( "glass" );
			}

			Parent = ParentPanel.Parent;
		}

		public void Unfreeze()
		{
			PhysicsEnabled = true;
			PhysicsBody.BodyType = PhysicsBodyType.Dynamic;
			PhysicsBody.Sleeping = false;

			EnableTouch = false;
			Tags.Add( "glass" );

			Parent = null;
		}

		public bool IsFrozen()
		{
			return !PhysicsEnabled;
		}

		public bool IsOnFrameEdge()
		{
			if ( OnFrameEdge == OnFrame.Unknown )
			{
				OnFrameEdge = OnFrame.False;

				var parentPanelSize = ParentPanel.PanelSize * 0.5f * 0.99f;

				foreach ( var vertex in PanelVertices )
				{
					if ( MathF.Abs( vertex.x ) >= parentPanelSize.x ||
						 MathF.Abs( vertex.y ) >= parentPanelSize.y )
					{
						OnFrameEdge = OnFrame.True;
						break;
					}
				}
			}

			return OnFrameEdge == OnFrame.True;
		}

		public bool IsShardNearPanelSpacePosition()
		{
			if ( ParentPanel.IsValid() )
			{
				var shardPos = ParentPanel.GetPanelTransform().TransformVector( LocalPanelSpaceOrigin );
				return (shardPos - Position).LengthSquared < 4.0f;
			}

			return false;
		}

		public Vector2 TransformPosWorldToPanel( Vector3 position )
		{
			Vector2 localPosition = Transform.PointToLocal( position );
			return localPosition + LocalPanelSpaceOrigin;
		}

		public Vector3 TransformPosPanelToWorld( Vector2 position )
		{
			return Transform.TransformVector( position - LocalPanelSpaceOrigin );
		}

		Transform GetSpawnTransform()
		{
			if ( ParentShard != null )
			{
				var localPos = LocalPanelSpaceOrigin - ParentShard.LocalPanelSpaceOrigin;
				var parentTransform = ParentShard.Transform;
				return parentTransform.WithPosition( parentTransform.Position + parentTransform.Rotation * localPos );
			}

			var transform = ParentPanel.GetPanelTransform();
			return transform.WithPosition( transform.TransformVector( LocalPanelSpaceOrigin ) );
		}

		public void ShatterWorldSpace( Vector3 position )
		{
			if ( !IsAuthority )
				return;

			ShatterLocalSpace( TransformPosWorldToPanel( position ) );
		}

		public bool ShatterLocalSpace( Vector2 position )
		{
			if ( !IsAuthority )
				return false;

			if ( !EnableDrawing )
				return false;

			if ( !IsPointInsideShard( position ) )
				return false;

			if ( IsTooSmall() )
			{
				if ( ParentPanel.IsValid() )
				{
					ParentPanel.IsBroken = true;
				}

				PhysicsEnabled = false;
				EnableAllCollisions = false;
				EnableDrawing = false;
				PhysicsBody.Sleeping = false;

				Delete();

				return true;
			}

			StressPosition = position;

			if ( GenerateShatterShards() )
			{
				if ( ParentPanel.IsValid() )
				{
					ParentPanel.IsBroken = true;
					_ = ParentPanel.OnBreak.Fire( this );
				}

				MarkForDeletion();
			}

			return true;
		}

		private bool CanGenerateShardModel()
		{
			if ( IsTooSmall() )
			{
				return false;
			}

			return true;
		}

		private bool IsTooSmall()
		{
			return CalculateArea() < 4.0f;
		}

		private bool IsABigPiece()
		{
			return CalculateArea() > 5000.0f;
		}

		private float CalculateArea()
		{
			float area = 0;

			var verticies = PanelVertices.ToArray();
			int vertexCount = PanelVertices.Count;

			if ( vertexCount < 3 )
			{
				vertexCount = Desc.PanelVertices.Length;
				verticies = Desc.PanelVertices;
			}

			if ( vertexCount < 3 )
			{
				return 0;
			}
			else
			{
				var v1 = verticies[0];

				for ( int i = 1; i < vertexCount - 1; i++ )
				{
					var v2 = verticies[i];
					var v3 = verticies[i + 1];

					float x1 = v2.x - v1.x;
					float y1 = v2.y - v1.y;
					float x2 = v3.x - v1.x;
					float y2 = v3.y - v1.y;

					area += MathF.Abs( x1 * y2 - x2 * y1 );
				}

				area = MathF.Abs( area * 0.5f );
			}

			return area;
		}

		private bool IsPointInsideShard( Vector2 point )
		{
			if ( PanelVertices.Count < 3 )
				return false;

			if ( PanelVertices.Count > 100 )
				return false;

			int positive = 0;
			int negative = 0;

			for ( int i = 0; i < PanelVertices.Count; i++ )
			{
				var v1 = PanelVertices[i];
				var v2 = PanelVertices[i < PanelVertices.Count - 1 ? i + 1 : 0];

				float cross = (point.x - v1.x) * (v2.y - v1.y) - (point.y - v1.y) * (v2.x - v1.x);

				if ( cross > 0 )
				{
					positive++;
				}
				else if ( cross < 0 )
				{
					negative++;
				}

				if ( positive > 0 && negative > 0 )
				{
					return false;
				}
			}

			return true;
		}

		private static Vector4 ComputeTangentForFace( Vector3 faceS, Vector3 faceT, Vector3 normal )
		{
			var leftHanded = Vector3.Dot( Vector3.Cross( faceS, faceT ), normal ) < 0.0f;
			var tangent = Vector4.Zero;

			if ( !leftHanded )
			{
				faceT = Vector3.Cross( normal, faceS );
				faceS = Vector3.Cross( faceT, normal );
				faceS = faceS.Normal;

				tangent.x = faceS[0];
				tangent.y = faceS[1];
				tangent.z = faceS[2];
				tangent.w = 1.0f;
			}
			else
			{
				faceT = Vector3.Cross( faceS, normal );
				faceS = Vector3.Cross( normal, faceT );
				faceS = faceS.Normal;

				tangent.x = faceS[0];
				tangent.y = faceS[1];
				tangent.z = faceS[2];
				tangent.w = -1.0f;
			}

			return tangent;
		}

		private static Vector3 ComputeTriangleNormal( Vector3 v1, Vector3 v2, Vector3 v3 )
		{
			var e1 = v2 - v1;
			var e2 = v3 - v1;

			return Vector3.Cross( e1, e2 ).Normal;
		}

		private static void ComputeTriangleTangentSpace( Vector3 p0, Vector3 p1, Vector3 p2, Vector2 t0, Vector2 t1, Vector2 t2, out Vector3 s, out Vector3 t )
		{
			const float epsilon = 1e-12f;

			s = Vector3.Zero;
			t = Vector3.Zero;

			var edge0 = new Vector3( p1.x - p0.x, t1.x - t0.x, t1.y - t0.y );
			var edge1 = new Vector3( p2.x - p0.x, t2.x - t0.x, t2.y - t0.y );

			var cross = Vector3.Cross( edge0, edge1 );

			if ( MathF.Abs( cross.x ) > epsilon )
			{
				s.x += -cross.y / cross.x;
				t.x += -cross.z / cross.x;
			}

			edge0 = new Vector3( p1.y - p0.y, t1.x - t0.x, t1.y - t0.y );
			edge1 = new Vector3( p2.y - p0.y, t2.x - t0.x, t2.y - t0.y );

			cross = Vector3.Cross( edge0, edge1 );

			if ( MathF.Abs( cross.x ) > epsilon )
			{
				s.y += -cross.y / cross.x;
				t.y += -cross.z / cross.x;
			}

			edge0 = new Vector3( p1.z - p0.z, t1.x - t0.x, t1.y - t0.y );
			edge1 = new Vector3( p2.z - p0.z, t2.x - t0.x, t2.y - t0.y );

			cross = Vector3.Cross( edge0, edge1 );

			if ( MathF.Abs( cross.x ) > epsilon )
			{
				s.z += -cross.y / cross.x;
				t.z += -cross.z / cross.x;
			}

			s = s.Normal;
			t = t.Normal;
		}

		private static void ComputeTriangleNormalAndTangent( out Vector3 outNormal, out Vector4 outTangent, Vector3 v0, Vector3 v1, Vector3 v2, Vector2 uv0, Vector2 uv1, Vector2 uv2 )
		{
			outNormal = ComputeTriangleNormal( v0, v1, v2 );
			ComputeTriangleTangentSpace( v0, v1, v2, uv0, uv1, uv2, out var faceS, out var faceT );
			outTangent = ComputeTangentForFace( faceS, faceT, outNormal );
		}

		private static bool LineIntersect( Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection )
		{
			intersection = Vector2.Zero;

			float xD1, yD1, xD2, yD2, xD3, yD3;
			float dot, deg, length1, length2;
			float segmentLength1, segmentLength2;
			float ua, div;

			xD1 = p2.x - p1.x;
			xD2 = p4.x - p3.x;
			yD1 = p2.y - p1.y;
			yD2 = p4.y - p3.y;
			xD3 = p1.x - p3.x;
			yD3 = p1.y - p3.y;

			length1 = MathF.Sqrt( xD1 * xD1 + yD1 * yD1 );
			length2 = MathF.Sqrt( xD2 * xD2 + yD2 * yD2 );

			dot = (xD1 * xD2 + yD1 * yD2);
			deg = dot / (length1 * length2);

			if ( Math.Abs( deg ) == 1.0f )
			{
				return false;
			}

			div = yD2 * xD1 - xD2 * yD1;
			ua = (xD2 * yD3 - yD2 * xD3) / div;
			intersection.x = p1.x + ua * xD1;
			intersection.y = p1.y + ua * yD1;

			xD1 = intersection.x - p1.x;
			xD2 = intersection.x - p2.x;
			yD1 = intersection.y - p1.y;
			yD2 = intersection.y - p2.y;
			segmentLength1 = MathF.Sqrt( xD1 * xD1 + yD1 * yD1 ) + MathF.Sqrt( xD2 * xD2 + yD2 * yD2 );

			xD1 = intersection.x - p3.x;
			xD2 = intersection.x - p4.x;
			yD1 = intersection.y - p3.y;
			yD2 = intersection.y - p4.y;
			segmentLength2 = MathF.Sqrt( xD1 * xD1 + yD1 * yD1 ) + MathF.Sqrt( xD2 * xD2 + yD2 * yD2 );

			if ( MathF.Abs( length1 - segmentLength1 ) > 0.01f || MathF.Abs( length2 - segmentLength2 ) > 0.01f )
			{
				return false;
			}

			return true;
		}
	}
}
