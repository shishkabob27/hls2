﻿global using Sandbox;
global using Editor;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using XeNPC;

/// <summary>
/// This is the heart of the sv_gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
public partial class HLGame : GameManager
{
	[Net]
	public HudPanel Hud { get; set; }
	public HLGUI GUI { get; set; }

	MenuPanel Menu { get; set; }

	public HLGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside.
		//
		if ( Game.IsServer )
		{
			//Temp fix because setting lobby convars dont seem to work
			if ( Game.Server.ServerTitle.ToLower().Contains( "deathmatch" ) )
			{
				sv_gamemode = "deathmatch";
			}
			else
			{
				sv_gamemode = "campaign";
			}

			GUI = new HLGUI();

			if ( sv_gamemode == "deathmatch" || sv_gamemode == "ctf" )
			{
				_ = GameLoopAsync();
			}
			Hud = new HudPanel();
		}
		Audio.ReverbScale = 1;
		Audio.ReverbVolume = 1;
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		ItemRespawn.Init();
	}

	/// <summary>
	/// The player wants to enable the devcam. Probably shouldn't allow this
	/// unless you're in a sandbox mode or they're a dev.
	/// </summary>
	public override void DoPlayerDevCam( IClient client )
	{
		Game.AssertServer();

		//if ( !client.HasPermission( "devcam" ) ) 
		//	return;

		var camera = client.Components.Get<DevCamera>( true );

		if ( camera == null )
		{
			camera = new DevCamera();
			client.Components.Add( camera );
			return;
		}

		camera.Enabled = !camera.Enabled;
		RPCHIDE( To.Single( client ), camera.Enabled );
	}

	[ClientRpc]
	void RPCHIDE( bool enabled )
	{
		GUIRootPanel.Current?.SetClass( "devcamera", enabled );
		HudRootPanel.Current?.SetClass( "devcamera", enabled );
	}

	public override void ClientJoined( IClient cl )
	{
		base.ClientJoined( cl );

		var player = new HLPlayer();
		cl.Pawn = player;

		player.Respawn();
	}

	[ClientRpc]
	public override void OnKilledMessage( long leftid, string left, long rightid, string right, string method )
	{
		Sandbox.UI.KillFeed.Current?.AddEntry( leftid, left, rightid, right, method );
	}

	public override void RenderHud()
	{
		var localPawn = Game.LocalPawn as HLPlayer;
		if ( localPawn == null ) return;

		//
		// scale the screen using a matrix, so the scale math doesn't invade everywhere
		// (other than having to pass the new scale around)
		//

		var scale = Screen.Height / 1080.0f;
		var screenSize = Screen.Size / scale;
		var matrix = Matrix.CreateScale( scale );
	}

	[GameEvent.Entity.PostCleanup]
	void onclean()
	{
		HLCombat.GibCount = 0;
		HLCombat.GibFadingCount = 0;
	}

	[ConCmd.Server( "resetgui", Help = "resets gui" )]
	public static void resetgui()
	{
		(HLGame.Current as HLGame).resetgui2();
	}

	public void resetgui2()
	{
		GUI.Delete();
		GUI = new HLGUI();
		Hud.Delete();
		Hud = new HudPanel();


	}

	[ConCmd.Client( "menu", Help = "resets gui" )]
	public static void menu1()
	{
		(HLGame.Current as HLGame).menu2();
	}

	public void menu2()
	{
		if ( Menu == null )
		{
			Menu = new MenuPanel();
		}
		else
		{
			Menu.Delete();
			Menu = null;
		}


	}

	[ConCmd.Server( "resetplayer", Help = "resets player" )]
	public static void resetplayer()
	{
		foreach ( var client in Game.Clients )
		{
			if ( client.Pawn != null )
				client.Pawn.Delete();
			var player = new HLPlayer();
			player.Respawn();

			client.Pawn = player;
		}
	}
}
