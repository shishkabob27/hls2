﻿public partial class HLGame : GameManager
{
	public static void MoveToDMSpawnpoint( Entity pawn )
	{
		var spawnpoint = Entity.All
								.OfType<info_player_deathmatch>()
								.OrderByDescending( x => DMSpawnpointWeight( pawn, x ) )
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

	public static float DMSpawnpointWeight( Entity pawn, Entity spawnpoint )
	{
		float distance = 0;

		foreach ( var client in Game.Clients )
		{
			if ( client.Pawn == null ) continue;
			if ( client.Pawn == pawn ) continue;
			if ( (client.Pawn as HLPlayer).LifeState != LifeState.Alive ) continue;

			var spawnDist = (spawnpoint.Position - client.Pawn.Position).Length;
			distance = MathF.Max( distance, spawnDist );
		}

		//Log.Info( $"{spawnpoint} is {distance} away from any player" );

		return distance;
	}

}
