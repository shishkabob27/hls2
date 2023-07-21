using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class Subtitle : Panel
{
	public static Subtitle Current;

	public Subtitle()
	{
		Current = this;
	}

	[ClientRpc]
	public static void DisplaySubtitle( string SentenceName )
    {
		if (HLGame.cc_subtitles == 0)
			return;
		Current?.AddSubtitle( SentenceName );
	}

	private async Task AddSubtitle( string SentenceName )
	{
		var panel = Current.Add.Label( "#" + SentenceName, SentenceName );
		if (panel.Text == SentenceName)
		{
			panel.Delete();
			return;
		}

		var sound = Sound.FromScreen( SentenceName );
		sound.SetVolume( 0f );
		await Task.DelayRealtime( 100 );

		while ( sound.IsPlaying )
		{
			await Task.DelayRealtime( 100 );
		}

		panel.Delete();

	}
}
