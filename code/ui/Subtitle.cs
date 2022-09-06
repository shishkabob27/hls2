using Sandbox.UI;
using Sandbox.UI.Construct;

public class Subtitle : Panel
{

	public Panel SubBackground;
	public Label SubText;


	public Subtitle()
	{

		SubBackground = Add.Panel("subbackground");
		SubText = Add.Label("");

		SubBackground.AddChild(SubText);	
	}

	public void DisplaySubtitle(string SentenceName)
    {
		if (HLGame.cc_subtitles >= 1)
		{
			SubText.Text = "#" + SentenceName;
			SubBackground.Style.Opacity = 100;
		}
	}

	public override void Tick()
	{
        base.Tick();
	}
}
