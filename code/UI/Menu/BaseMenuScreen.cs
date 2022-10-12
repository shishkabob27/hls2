using Sandbox.UI;
[UseTemplate( "/UI/Menu/NewGame.html" )]
public class BaseMenuScreen : Panel
{
	public void BaseButtonClick()
	{
		PlaySound( "launch_select2" );
	}
	public async Task BaseButtonClickDown( Panel p, bool doanim = true )
	{
		PlaySound( "launch_dnmenu1" );
		if ( doanim ) AddClass( "justcldwn" );
		if ( doanim ) await GameTask.DelaySeconds( 0.2f );
	}

	public async Task BaseButtonClickUp( Panel p, bool doanim = true )
	{
		PlaySound( "launch_upmenu1" );
		if ( doanim ) p.AddClass( "justcl" );
		if ( doanim ) await GameTask.DelaySeconds( 0.2f );
	}

	[ConCmd.Server]
	static void chnglvlad( string MAP )
	{
		if ( ConsoleSystem.Caller.IsListenServerHost )
		{
			Global.ChangeLevel( MAP );
		}
	}
}
