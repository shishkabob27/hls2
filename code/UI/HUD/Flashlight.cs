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


        switch (player.HasHEV && player.Health >= 0)
        {
            case true: SetClass( "invisible", false); break;
            case false: SetClass( "invisible", true); break;
        }

        if (!player.HasHEV || player.Health == 0)
        {
            SetClass("invisible", true);
        }
        else
        {
            SetClass("invisible", false);
        }

    }

}
