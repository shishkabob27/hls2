using Sandbox.UI;
using Sandbox.UI.Construct;

public class ChapterText : Panel
{

    public Label text;

    RealTimeSince OutroTime = new RealTimeSince();
    public ChapterText()
    {
        OutroTime = 0;
        var mapname = Global.MapName.Replace("#local","");
        var translated = "";
        switch ( mapname )
        {
            case "shishkabob.hls2_t0": translated = "#chaptertext.hz"; break;
            case "shishkabob.hls2_c1p0": translated = "#chaptertext.c1"; break;
            case "shishkabob.hls2_c2p0": translated = "#chaptertext.c2"; break;
            case "shishkabob.hls2_c3p0": translated = "#chaptertext.c3"; break;
            case "shishkabob.hls2_c4p0": translated = "#chaptertext.c4"; break;
            case "shishkabob.hls2_c5p0": translated = "#chaptertext.c5"; break;
            case "shishkabob.hls2_c6p0": translated = "#chaptertext.c6"; break;
            case "shishkabob.hls2_c7p0": translated = "#chaptertext.c7"; break;
            case "shishkabob.hls2_c8p0": translated = "#chaptertext.c8"; break;
            case "shishkabob.hls2_c9p0": translated = "#chaptertext.c9"; break;
            case "shishkabob.hls2_c10p0": translated = "#chaptertext.c10"; break;
            case "shishkabob.hls2_c11p0": translated = "#chaptertext.c11"; break;
            case "shishkabob.hls2_c12p0": translated = "#chaptertext.c12"; break;
            case "shishkabob.hls2_c13p0": translated = "#chaptertext.c13"; break;
            case "shishkabob.hls2_c14p0": translated = "#chaptertext.c14"; break;
            case "shishkabob.hls2_c15p0": translated = "#chaptertext.c15"; break;
            case "shishkabob.hls2_c16p0": translated = "#chaptertext.c16"; break;
            case "shishkabob.hls2_c17p0": translated = "#chaptertext.c17"; break;
            case "shishkabob.hls2_c18p0": translated = "#chaptertext.c18"; break;
            case "shishkabob.hls2_c19p0": translated = "#chaptertext.c19"; break;
            default: translated = mapname; break;
                // we should probably default to the asset.party name of the map so custom maps can get their map title in.
        }
        text = Add.Label( translated );

        text.AddClass( "intro" );
    }

    public override void Tick()
    {
        base.Tick();

        if ( OutroTime.Relative > 5f )
        {
            text.RemoveClass( "intro" );
            text.AddClass( "outro" );
        }
    }

}
