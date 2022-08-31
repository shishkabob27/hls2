using Sandbox.UI;
using Sandbox.UI.Construct;

public class FlashlightUI : Panel
{
    
	public FlashlightUI(){
	}

	public override void Tick()
	{
        var player = Local.Pawn as HLPlayer;

        switch (player.FlashlightEnabled)
        {
            case true: SetClass("on", true); break;
            case false: SetClass("on", false); break;
        }


        switch (player.HasHEV)
        {
            case true: SetClass( "invisible", false); break;
            case false: SetClass( "invisible", true); break;
        }

	}

}
