﻿
using Sandbox.UI;

[UseTemplate]
class MapVotePanel : Panel
{
	public string TitleText { get; set; } = "Map Vote";
	public string SubtitleText { get; set; } = "Choose your next map";
	public string TimeText { get; set; } = "00:33";

	public Panel Body { get; set; }

	public List<MapIcon> MapIcons = new();

	public MapVotePanel()
	{
		_ = PopulateMaps();
	}

	public async Task PopulateMaps()
	{
		var query = "game:shishkabob.hls2";
		if ( HLGame.hl_extended_mapvote )
		{
			query += "game:matt.ttt";
			query += "game:facepunch.dm98";
			query += "game:gman.dm04";
			query += "game:facepunch.boomer";
		}
		 
		var packages = await Package.FindAsync( query );

		foreach ( var package in packages.Packages )
		{
			AddMap( package.FullIdent );
		}
	}

	private MapIcon AddMap( string fullIdent )
	{
		var icon = MapIcons.FirstOrDefault( x => x.Ident == fullIdent );

		if ( icon != null )
			return icon;

		icon = new MapIcon( fullIdent );
		icon.AddEventListener( "onmousedown", () => MapVoteEntity.SetVote( fullIdent ) );
		Body.AddChild( icon );

		MapIcons.Add( icon );
		return icon;
	}

	public override void Tick()
	{
		base.Tick();
	}

	internal void UpdateFromVotes( IDictionary<IClient, string> votes )
	{
		foreach ( var icon in MapIcons )
			icon.VoteCount = "0";

		foreach ( var group in votes.GroupBy( x => x.Value ).OrderByDescending( x => x.Count() ) )
		{
			var icon = AddMap( group.Key );
			icon.VoteCount = group.Count().ToString( "n0" );
		}
	}
}

