global using Sandbox;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
partial class HLGame : Game
{
	[Net]
	HLHud Hud { get; set; }

	StandardPostProcess postProcess;

	[ConVar.Replicated] public static string HLDificulty { get; set; } = "Medium";
	[ConVar.Replicated] public static string HLGamemode { get; set; } = "Campaign";


	public HLGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside.
		//
		if ( IsServer )
		{
			Hud = new HLHud();

			_ = GameLoopAsync();
		}

		if ( IsClient )
		{
			postProcess = new StandardPostProcess();
			PostProcess.Add( postProcess );
		}
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		ItemRespawn.Init();
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		var player = new HLPlayer();
		player.Respawn();

		cl.Pawn = player;
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		var spawnpoint = Entity.All
								.OfType<SpawnPoint>()
								.OrderByDescending( x => SpawnpointWeight( pawn, x ) )
								.ThenBy( x => Guid.NewGuid() )
								.FirstOrDefault();

		//Log.Info( $"chose {spawnpoint}" );

		if ( spawnpoint == null )
		{
			Log.Warning( $"Couldn't find spawnpoint for {pawn}!" );
			return;
		}

		pawn.Transform = spawnpoint.Transform;
		pawn.Rotation = spawnpoint.Rotation;
	}

	/// <summary>
	/// The higher the better
	/// </summary>
	public float SpawnpointWeight( Entity pawn, Entity spawnpoint )
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

	public override void OnKilled( Client client, Entity pawn )
	{
		base.OnKilled( client, pawn );

		Hud.OnPlayerDied( To.Everyone, pawn as HLPlayer);
	}


	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		postProcess.Sharpen.Enabled = false;
		postProcess.Sharpen.Strength = 0.5f;

		postProcess.FilmGrain.Enabled = false;
		postProcess.FilmGrain.Intensity = 0;
		postProcess.FilmGrain.Response = 1;

		postProcess.Vignette.Enabled = false;
		postProcess.Vignette.Intensity = 1.0f;
		postProcess.Vignette.Roundness = 1.5f;
		postProcess.Vignette.Smoothness = 0.5f;
		postProcess.Vignette.Color = Color.Black;

		postProcess.Saturate.Enabled = false;
		postProcess.Saturate.Amount = 1;

		postProcess.Blur.Enabled = false;

		Audio.SetEffect( "core.player.death.muffle1", 0 );

		if ( Local.Pawn is HLPlayer localPlayer )
		{
			var timeSinceDamage = localPlayer.TimeSinceDamage.Relative;
			var damageUi = timeSinceDamage.LerpInverse( 0.25f, 0.0f, true ) * 0.3f;
			if ( damageUi > 0 )
			{
				postProcess.Saturate.Amount -= damageUi;
				postProcess.Vignette.Color = Color.Lerp( postProcess.Vignette.Color, Color.Red, damageUi );
				postProcess.Vignette.Intensity += damageUi;
				postProcess.Vignette.Smoothness += damageUi;
				postProcess.Vignette.Roundness += damageUi;

				postProcess.Blur.Enabled = true;
				postProcess.Blur.Strength = damageUi * 0.5f;
			}


			var healthDelta = localPlayer.Health.LerpInverse( 0, 100.0f, true );

			healthDelta = MathF.Pow( healthDelta, 0.5f );

			postProcess.Vignette.Color = Color.Lerp( postProcess.Vignette.Color, Color.Red, 1 - healthDelta );
			postProcess.Vignette.Intensity += (1 - healthDelta) * 0.5f;
			postProcess.Vignette.Smoothness += (1 - healthDelta);
			postProcess.Vignette.Roundness += (1 - healthDelta) * 0.5f;
			postProcess.Saturate.Amount *= healthDelta;
			postProcess.FilmGrain.Intensity += (1 - healthDelta) * 0;

			Audio.SetEffect( "core.player.death.muffle1", 1 - healthDelta, velocity: 2.0f );

		}


		if ( CurrentState == GameStates.Warmup )
		{
			//postProcess.FilmGrain.Intensity = 0.4f;
			//postProcess.FilmGrain.Response = 0.5f;

			postProcess.Saturate.Amount = 0;
		}
	}

	[ClientRpc]
	public override void OnKilledMessage( long leftid, string left, long rightid, string right, string method )
	{
		Sandbox.UI.KillFeed.Current?.AddEntry( leftid, left, rightid, right, method );
	}

	public override void RenderHud()
	{
		var localPawn = Local.Pawn as HLPlayer;
		if ( localPawn == null ) return;

		//
		// scale the screen using a matrix, so the scale math doesn't invade everywhere
		// (other than having to pass the new scale around)
		//

		var scale = Screen.Height / 1080.0f;
		var screenSize = Screen.Size / scale;
		var matrix = Matrix.CreateScale( scale );

		using ( Render.Draw2D.MatrixScope( matrix ) )
		{
			localPawn.RenderHud( screenSize );
		}
	}

}
