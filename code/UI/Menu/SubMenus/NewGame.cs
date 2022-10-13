﻿using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/NewGame.html" )]
class NewGame : BaseMenuScreen
{
	public void ClickDown()
	{
		BaseButtonClick();
	}

	public void Easy( Panel p )
	{
		ConsoleSystem.Run( "skill 0" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Medium( Panel p )
	{
		ConsoleSystem.Run( "skill 1" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Difficult( Panel p )
	{
		ConsoleSystem.Run( "skill 2" );
		ConsoleSystem.Run( "chnglvlad shishkabob.hls2_c1p0" );
	}
	public void Cancel( Panel p )
	{
		Delete();
		var a = Parent.AddChild<Menu>();
		BaseButtonClickDown( p, a, true, "New game" );
	}
}
