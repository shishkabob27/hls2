using System.Runtime.InteropServices;

namespace Sandbox
{
	public partial class GlassShard
	{
		[StructLayout( LayoutKind.Sequential )]
		private struct ShardVertex
		{
			public Vector3 Position;
			public Vector3 Normal;
			public Vector2 TexCoord0;
			public Vector2 TexCoord1;
			public Vector3 Color;
			public Vector4 Tangent;

			public static readonly VertexAttribute[] Layout =
			{
				new VertexAttribute( VertexAttributeType.Position, VertexAttributeFormat.Float32, 3 ),
				new VertexAttribute( VertexAttributeType.Normal, VertexAttributeFormat.Float32, 3 ),
				new VertexAttribute( VertexAttributeType.TexCoord, VertexAttributeFormat.Float32, 2, 0 ),
				new VertexAttribute( VertexAttributeType.TexCoord, VertexAttributeFormat.Float32, 2, 1 ),
				new VertexAttribute( VertexAttributeType.Color, VertexAttributeFormat.Float32, 3, 0 ),
				new VertexAttribute( VertexAttributeType.Tangent, VertexAttributeFormat.Float32, 4 ),
			};
		}
	}
}
