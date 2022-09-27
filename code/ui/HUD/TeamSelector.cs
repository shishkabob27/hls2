using Sandbox.UI;
using Sandbox.UI.Construct;

[UseTemplate( "/resource/templates/teamselect.html" )]
public class TeamSelector : Panel
{
    
	public TeamSelector()
	{
	}

	public void SelectTeam(string team){
		var p = Local.Pawn as HLPlayer;

		p.team = team;
		Delete();
	}

}
