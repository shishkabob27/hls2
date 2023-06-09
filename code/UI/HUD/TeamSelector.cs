using Sandbox.UI;
using Sandbox.UI.Construct;

[UseTemplate( "/Resource/templates/teamselect.html" )]
public class TeamSelector : Panel
{
    
	public TeamSelector()
	{
	}

	public void SelectTeam(string team){
		var p = Game.LocalPawn as HLPlayer;

		switch (team)
		{
			case "bm": p.team = 1; break;
			case "of": p.team = 2; break;
			default: p.team = 0; break;
		}

		Delete();
	}

}
