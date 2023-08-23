using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Text.RegularExpressions;

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
		var panel = Current.Add.Label( "#" + SentenceName );
		if ( panel.Text == SentenceName )
		{
			panel.Delete();
			//Log.Warning( $"[HL:S2] {SentenceName} has not been localized yet." );
			return;
		}
		else if (panel.Text == "" || panel.Text == " ")
		{
			panel.Delete();
			return;
		}

		//Caption codes
		//These currently effect the entire sentence

		if ( panel.Text.Contains( "<sfx>" ) )
		{
			if ( HLGame.cc_subtitles == 2 )
			{
				panel.Text = panel.Text.Replace( "<sfx>", "" );
			}
			else
			{
				panel.Delete();
				return;
			}
		}

		if ( panel.Text.Contains( "<clr:" ) )
		{
			var match = Regex.Match( panel.Text, "<clr:(\\d{1,3}),(\\d{1,3}),(\\d{1,3})>" );
			if ( match.Success )
			{
				var r = int.Parse( match.Groups[1].Value );
				var g = int.Parse( match.Groups[2].Value );
				var b = int.Parse( match.Groups[3].Value );
				panel.Style.FontColor = new Color( r, g, b );
			}

			panel.Text = Regex.Replace( panel.Text, "<clr:(\\d{1,3}),(\\d{1,3}),(\\d{1,3})>", "" );
		}

		if ( panel.Text.Contains( "<I>" ) )
		{
			panel.Style.FontStyle = FontStyle.Italic;
			panel.Text = panel.Text.Replace( "<I>", "" );
		}

		if ( panel.Text.Contains( "<B>" ) )
		{
			panel.Style.FontWeight = 600;
			panel.Text = panel.Text.Replace( "<B>", "" );
		}

		if ( panel.Text.Contains( "<cr>" ) )
		{
			panel.Text = panel.Text.Replace( "<cr>", "\n" );
		}

		//TODO: <playerclr>, <norepeat>, <len>, <delay>

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
