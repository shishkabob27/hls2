using System;

namespace Sandbox
{
	public partial class GlassShard
	{
		private class ModelDesc : BaseNetworkable, INetworkSerializer
		{
			public Vector2 PanelSize;
			public Vector2 StressPosition;
			public Vector2[] PanelVertices;
			public float HalfThickness;
			public bool HasParent;
			public bool IsParentFrozen;
			public Vector2 TextureOffset;
			public Vector2 TextureScale;
			public Vector2 TextureSize;
			public Vector3 TextureAxisU;
			public Vector3 TextureAxisV;
			public Transform PanelTransform;

			public void Read( ref NetRead read )
			{
				PanelSize = read.Read<Vector2>();
				StressPosition = read.Read<Vector2>();
				PanelVertices = read.ReadArray( PanelVertices );
				HalfThickness = read.Read<float>();
				HasParent = read.Read<bool>();
				IsParentFrozen = read.Read<bool>();
				TextureOffset = read.Read<Vector2>();
				TextureScale = read.Read<Vector2>();
				TextureSize = read.Read<Vector2>();
				TextureAxisU = read.Read<Vector3>();
				TextureAxisV = read.Read<Vector3>();
				PanelTransform = read.Read<Transform>();
			}

			public void Write( NetWrite write )
			{
				write.Write( PanelSize );
				write.Write( StressPosition );
				write.Write( PanelVertices );
				write.Write( HalfThickness );
				write.Write( HasParent );
				write.Write( IsParentFrozen );
				write.Write( TextureOffset );
				write.Write( TextureScale );
				write.Write( TextureSize );
				write.Write( TextureAxisU );
				write.Write( TextureAxisV );
				write.Write( PanelTransform );
			}

			public Vector2 GetPanelVertexAverage()
			{
				var average = Vector2.Zero;
				var vertexCount = PanelVertices.Length;

				if ( vertexCount > 0 )
				{
					foreach ( var vertex in PanelVertices )
					{
						average += vertex;
					}

					average /= vertexCount;
				}

				return average;
			}

			public float GetArea()
			{
				var area = 0.0f;
				var vertexCount = PanelVertices.Length;

				if ( vertexCount < 3 )
					return 0.0f;

				var v1 = PanelVertices[0];

				for ( int i = 1; i < vertexCount - 1; i++ )
				{
					var v2 = PanelVertices[i];
					var v3 = PanelVertices[i + 1];

					float x1 = v2.x - v1.x;
					float y1 = v2.y - v1.y;
					float x2 = v3.x - v1.x;
					float y2 = v3.y - v1.y;

					area += MathF.Abs( x1 * y2 - x2 * y1 );
				}

				area = MathF.Abs( area * 0.5f );

				return area;
			}

			public float GetLongestAcross()
			{
				var shortestAcross = float.MaxValue;
				var longestAcross = 0.0f;
				var sumOfAllEdges = 0.0f;

				for ( int i = 0; i < PanelVertices.Length; ++i )
				{
					for ( int j = 0; j < PanelVertices.Length; ++j )
					{
						if ( i != j && j > i )
						{
							var v1 = PanelVertices[i];
							var v2 = PanelVertices[j];

							float flEdge = Vector2.DistanceBetween( v1, v2 );

							if ( flEdge < shortestAcross )
								shortestAcross = flEdge;

							if ( flEdge > longestAcross )
								longestAcross = flEdge;

							sumOfAllEdges += flEdge;
						}
					}
				}

				return longestAcross;
			}
		}
	}
}
