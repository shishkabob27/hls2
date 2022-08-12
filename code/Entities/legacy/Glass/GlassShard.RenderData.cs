
namespace Sandbox
{
	public partial class GlassShard
	{
		private struct RenderData
		{
			public List<Vector3> VertexPositions;
			public Vector2 Average;
			public Vector3 LocalPanelSpaceOrigin;

			public int TotalShardVertices;
			public int TotalSharedIndices;
			public int EdgeVerticesStart;
			public int FaceVertexCount;
			public int FaceTriangleCount;
			public int FaceIndexCount;
			public int EdgeQuadCount;
			public int EdgeVertexCount;
			public int EdgeTriangleCount;
			public int EdgeIndexCount;

			public void Init( int numPanelVerts )
			{
				FaceVertexCount = numPanelVerts + 1;
				FaceTriangleCount = FaceVertexCount - 1;
				FaceIndexCount = FaceTriangleCount * 3;

				EdgeQuadCount = FaceVertexCount - 1;
				EdgeVertexCount = EdgeQuadCount * 4;
				EdgeTriangleCount = EdgeQuadCount * 2;
				EdgeIndexCount = EdgeTriangleCount * 3;

				TotalShardVertices = FaceVertexCount + FaceVertexCount + EdgeVertexCount;
				TotalSharedIndices = FaceIndexCount + FaceIndexCount + EdgeIndexCount;

				VertexPositions = new List<Vector3>( (FaceVertexCount * 2) + EdgeVertexCount );
			}
		};
	}
}
