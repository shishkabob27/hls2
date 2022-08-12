
namespace Sandbox
{
	public partial class GlassShard
	{
		private static readonly Vector2[] EdgeUVs = new[]
		{
			new Vector2( 0.0f, 0.0f ),
			new Vector2( 0.0f, 0.01f ),
			new Vector2( 0.01f, 0.01f ),
			new Vector2( 0.01f, 0.0f )
		};

		private Mesh CreateMeshForShard( RenderData renderData )
		{
			var vertices = new ShardVertex[renderData.TotalShardVertices];
			var indices = new int[renderData.TotalSharedIndices];
			var bounds = new BBox();

			for ( int i = 0; i < renderData.TotalShardVertices; i++ )
			{
				vertices[i].Position = renderData.VertexPositions[i] - renderData.LocalPanelSpaceOrigin;
				bounds = bounds.AddPoint( vertices[i].Position );

				var vertexPos = Desc.PanelTransform.PointToWorld( new Vector3( renderData.VertexPositions[i].x, renderData.VertexPositions[i].y, 0 ) );
				var u = Vector3.Dot( Desc.TextureAxisU, vertexPos ) / Desc.TextureScale.x;
				var v = Vector3.Dot( Desc.TextureAxisV, vertexPos ) / Desc.TextureScale.y;

				u += Desc.TextureOffset.x;
				v += Desc.TextureOffset.y;

				u /= Desc.TextureSize.x;
				v /= Desc.TextureSize.y;

				var uv = new Vector2( u, v );

				vertices[i].TexCoord0 = uv;
				vertices[i].TexCoord1 = renderData.VertexPositions[i];

				if ( i < renderData.EdgeVerticesStart )
				{
					vertices[i].Color = Vector3.Zero;
				}
				else
				{
					vertices[i].TexCoord0 += EdgeUVs[i % 4];
					vertices[i].TexCoord1 += EdgeUVs[i % 4];
					vertices[i].Color[0] = 1;
					vertices[i].Color[1] = 0;
					vertices[i].Color[2] = 0;
				}
			}

			{
				ComputeTriangleNormalAndTangent( out var normalSideA, out var tangentSideA,
					vertices[1].Position, vertices[0].Position, vertices[2].Position,
					vertices[1].TexCoord1, vertices[0].TexCoord1, vertices[2].TexCoord1 );

				ComputeTriangleNormalAndTangent( out var normalSideB, out var tangentSideB,
					vertices[renderData.FaceVertexCount].Position, vertices[renderData.FaceVertexCount + 1].Position, vertices[renderData.FaceVertexCount + 2].Position,
					vertices[renderData.FaceVertexCount].TexCoord1, vertices[renderData.FaceVertexCount + 1].TexCoord1, vertices[renderData.FaceVertexCount + 2].TexCoord1 );

				for ( int i = 0; i < renderData.FaceTriangleCount; i++ )
				{
					var index = i * 3;
					var offset0 = i + 1;
					var offset1 = (i + 2 < renderData.FaceVertexCount) ? i + 2 : 1;
					var offset2 = 0;

					indices[index] = offset1;
					indices[index + 1] = offset0;
					indices[index + 2] = offset2;

					vertices[offset0].Normal = normalSideA;
					vertices[offset1].Normal = normalSideA;
					vertices[offset2].Normal = normalSideA;

					vertices[offset0].Tangent = tangentSideA;
					vertices[offset1].Tangent = tangentSideA;
					vertices[offset2].Tangent = tangentSideA;

					index += renderData.FaceIndexCount;
					offset0 += renderData.FaceVertexCount;
					offset1 += renderData.FaceVertexCount;
					offset2 = renderData.FaceVertexCount;

					indices[index] = offset0;
					indices[index + 1] = offset1;
					indices[index + 2] = offset2;

					vertices[offset0].Normal = normalSideB;
					vertices[offset1].Normal = normalSideB;
					vertices[offset2].Normal = normalSideB;

					vertices[offset0].Tangent = tangentSideB;
					vertices[offset1].Tangent = tangentSideB;
					vertices[offset2].Tangent = tangentSideB;
				}
			}

			var edgeIndexOffset = renderData.TotalSharedIndices - renderData.EdgeIndexCount;
			for ( int i = 0; i < renderData.EdgeQuadCount; i++ )
			{
				var index = edgeIndexOffset + (i * 6);
				var vertexOffset = renderData.EdgeVerticesStart + (i * 4);

				indices[index] = vertexOffset + 2;
				indices[index + 1] = vertexOffset + 1;
				indices[index + 2] = vertexOffset;

				indices[index + 3] = vertexOffset + 3;
				indices[index + 4] = vertexOffset + 2;
				indices[index + 5] = vertexOffset;

				{
					ComputeTriangleNormalAndTangent( out var faceNormal, out var faceTangent,
						vertices[vertexOffset + 2].Position, vertices[vertexOffset + 1].Position, vertices[vertexOffset].Position,
						vertices[vertexOffset + 2].TexCoord1, vertices[vertexOffset + 1].TexCoord1, vertices[vertexOffset].TexCoord1 );

					vertices[vertexOffset].Normal = faceNormal;
					vertices[vertexOffset + 1].Normal = faceNormal;
					vertices[vertexOffset + 2].Normal = faceNormal;
					vertices[vertexOffset + 3].Normal = faceNormal;

					vertices[vertexOffset].Tangent = faceTangent;
					vertices[vertexOffset + 1].Tangent = faceTangent;
					vertices[vertexOffset + 2].Tangent = faceTangent;
					vertices[vertexOffset + 3].Tangent = faceTangent;
				}
			}

			var mat = "materials/glass/sbox_glass.vmat";
			if ( ParentPanel.IsValid() )
			{
				if ( !string.IsNullOrEmpty( ParentPanel.Material ) ) mat = ParentPanel.Material;
				if ( Desc.HasParent && !string.IsNullOrEmpty( ParentPanel.BrokenMaterial ) ) mat = ParentPanel.BrokenMaterial;
			}

			var mesh = new Mesh( Material.Load( mat ) );
			mesh.CreateVertexBuffer<ShardVertex>( vertices.Length, ShardVertex.Layout, vertices );
			mesh.CreateIndexBuffer( indices.Length, indices );
			mesh.Bounds = bounds;

			return mesh;
		}
	}
}
