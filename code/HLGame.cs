global using Sandbox;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;


global using XeNPC;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
public partial class HLGame : Game
{
	[Net]
	HudPanel Hud { get; set; }
	GUIPanel GUI { get; set; }

	StandardPostProcess postProcess;



	public HLGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside.
		//
		if ( IsServer )
		{
			GUI = new GUIPanel();


			if ( Global.IsDedicatedServer )
			{
				hl_gamemode = "deathmatch";
			}
			if ( hl_gamemode == "deathmatch" )
			{
				_ = GameLoopAsync();
			}
			Hud = new HudPanel();
		}

		if ( IsClient )
		{
			//postProcess = new StandardPostProcess();
			//PostProcess.Add( postProcess );
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
		cl.Pawn = player;

		player.Respawn();
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

			var spawnDist = ( spawnpoint.Position - client.Pawn.Position ).Length;
			distance = MathF.Max( distance, spawnDist );
		}

		//Log.Info( $"{spawnpoint} is {distance} away from any player" );

		return distance;
	}

	public override void OnKilled( Client client, Entity pawn )
	{
		base.OnKilled( client, pawn );

		//Hud.OnPlayerDied( To.Everyone, pawn as HLPlayer);
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
	[Event.Entity.PostCleanup]
	void onclean()
	{
		HLCombat.GibCount = 0;
		HLCombat.GibFadingCount = 0;
	}
	[ConCmd.Server( "resetgui", Help = "resets gui" )]
	public static void resetgui()
	{
		( HLGame.Current as HLGame ).resetgui2();
	}
	public void resetgui2()
	{
		Hud.Delete();
		Hud = new HudPanel();
		GUI.Delete();
		GUI = new GUIPanel();
	}

	[ConCmd.Server( "resetplayer", Help = "resets player" )]
	public static void resetplayer()
	{
		foreach ( var client in Client.All )
		{
			if ( client.Pawn != null )
				client.Pawn.Delete();
			var player = new HLPlayer();
			player.Respawn();

			client.Pawn = player;
		}
	}
}
