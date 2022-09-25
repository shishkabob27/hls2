
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
		var query = new Package.Query
		{
			Type = Package.Type.Map,
			Order = Package.Order.User,
			Take = 50,
		};

		query.Tags.Add( "game:shishkabob.hls2" ); // maybe this should be a "for this game" type of thing instead
		if ( HLGame.hl_extended_mapvote )
		{
			query.Tags.Add( "game:matt.ttt" );
			query.Tags.Add( "game:facepunch.dm98" );
			query.Tags.Add( "game:gman.dm04" );
			query.Tags.Add( "game:facepunch.boomer" );
		}
		var packages = await query.RunAsync( default );

		foreach ( var package in packages )
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

	internal void UpdateFromVotes( IDictionary<Client, string> votes )
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

