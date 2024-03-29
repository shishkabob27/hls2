﻿using Sandbox.UI;

public class MenuRootPanel : RootPanel, Sandbox.Menu.IGameMenuPanel
{
	public static MenuRootPanel Current;

	public MenuRootPanel()
	{
		StyleSheet.Load( "UI/Menu/Menu.scss" );
		Current = this;
		AddChild<Menu>();
	}

	[GameEvent.Tick.Client]
	void MenuTick()
	{
		SetClass( "WantsNewButton", HLGame.hl_menu_newer_buttons );
		SetClass( "LockRes", HLGame.hl_menu_lock_res );
		SetClass( "fourbythree", HLGame.hl_menu_fourbythree );
	}

	public override void Tick()
	{
		if ( Game.InGame )
		{
			SetClass( "ingame", true );
			SetClass( "notingame", false );
		}
		else
		{
			SetClass( "ingame", false );
			SetClass( "notingame", true );
		}
	}

	protected override void UpdateScale( Rect screenSize )
	{
		if ( HLGame.hl_menu_lock_res )
		{
			Scale = 0.44444445f;
		}
		else
		{

			base.UpdateScale( screenSize );
		}
	}
}
