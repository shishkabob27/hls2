
namespace Sandbox
{
	public partial class GlassShard
	{
		private struct ShatterType
		{
			public int SpokesMin;
			public int SpokesMax;
			public float TipScaleMin;
			public float TipScaleMax;
			public float TipSpawnChance;
			public float TipScale;
			public float ShardScale;
			public float SecondTipSpawnChance;
			public float SecondShardScale;
			public bool HasCenterChunk;
			public float CenterChunkScale;
			public int ShardLimit;

			public ShatterType(
				int spokesMin,
				int spokesMax,
				float tipScaleMin,
				float tipScaleMax,
				float tipSpawnChance,
				float tipScale,
				float shardScale,
				float secondTipSpawnChance,
				float secondShardScale,
				bool hasCenterChunk,
				float centerChunkScale,
				int shardLimit )
			{
				SpokesMin = spokesMin;
				SpokesMax = spokesMax;
				TipScaleMin = tipScaleMin;
				TipScaleMax = tipScaleMax;
				TipSpawnChance = tipSpawnChance;
				TipScale = tipScale;
				ShardScale = shardScale;
				SecondTipSpawnChance = secondTipSpawnChance;
				SecondShardScale = secondShardScale;
				HasCenterChunk = hasCenterChunk;
				CenterChunkScale = centerChunkScale;
				ShardLimit = shardLimit;
			}
		};

		private static readonly ShatterType[] ShatterTypes = new[]
		{
			new ShatterType( 5,    10,     0.2f,   0.5f,   1.0f,   0.95f,  1.0f,   8.0f,    0.98f,     false,  0.0f,   4   ),	// Blunt
			new ShatterType( 8,    14,     0.1f,   0.3f,   3.0f,   0.95f,  1.0f,   16.0f,   0.98f,     false,  0.0f,   4   ),	// Ballistic
			new ShatterType( 8,    10,     0.4f,   0.6f,   0.0f,   0.95f,  0.95f,  1.2f,    0.98f,     false,  0.0f,   2   ),	// Pulse
			new ShatterType( 20,   20,     0.7f,   0.99f,  3.0f,   0.95f,  1.0f,   16.0f,   0.98f,     false,  0.9f,   10  ),	// Explosive
		};
	}
}
