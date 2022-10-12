using Sandbox.UI;
[UseTemplate( "/UI/Menu/NewGame.html" )]
public class BaseMenuScreen : Panel
{
	public void BaseButtonClick()
	{
		PlaySound( "launch_select2" );
	}
	public async Task BaseButtonClickDown( Panel p )
	{
		PlaySound( "launch_dnmenu1" );
		AddClass( "justcldwn" );
		await GameTask.DelaySeconds( 0.2f );
	}

	public async Task BaseButtonClickUp( Panel p )
	{
		PlaySound( "launch_upmenu1" );
		p.AddClass( "justcl" );
		await GameTask.DelaySeconds( 0.2f );
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
