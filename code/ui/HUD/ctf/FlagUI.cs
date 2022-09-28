using Sandbox.UI;
using Sandbox.UI.Construct;

public class FlagUI : Panel
{

    public  IconPanel flagBM { get; set; }
    public IconPanel flagOF { get; set; }

	public FlagUI()
    {
        SetTemplate("/resource/templates/flag.html");
	}

}
