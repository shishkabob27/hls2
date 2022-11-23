public partial class HLGame : Game
{
	[Net]
	public int ScoreTeamBM { get; set; } = 0;

	[Net]
	public int ScoreTeamOF { get; set; } = 0;

	public static void MoveToCTFSpawnpoint( Entity pawn )
	{
		var spawnpoint = Entity.All
								.OfType<info_player_deathmatch>()
								.OrderByDescending( x => CTFSpawnpointWeight( pawn, x ) )
								.ThenBy( x => Guid.NewGuid() )
								.FirstOrDefault();

		//Log.Info( $"chose {spawnpoint}" );

		if ( spawnpoint == null )
		{
			Log.Warning( $"Couldn't find spawnpoint for {pawn}!" );
			HLGame.MoveToSpawnpoint( pawn );
			return;
		}

		pawn.Transform = spawnpoint.Transform;
		pawn.Rotation = spawnpoint.Rotation;
	}

	public static float CTFSpawnpointWeight( Entity pawn, Entity spawnpoint )
	{
		float distance = 0;

		foreach ( var client in Client.All )
		{
			if ( client.Pawn == null ) continue;
			if ( client.Pawn == pawn ) continue;
			if ( client.Pawn.LifeState != LifeState.Alive ) continue;

			var spawnDist = (spawnpoint.Position - client.Pawn.Position).Length;
			distance = MathF.Max( distance, spawnDist );
		}

		//Log.Info( $"{spawnpoint} is {distance} away from any player" );

		return distance;
	}

}
