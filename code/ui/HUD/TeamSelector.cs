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

		switch (team)
		{
			case "bm": p.team = 1; break;
			case "of": p.team = 2; break;
			default: p.team = 0; break;
		}

		Delete();
	}

}
