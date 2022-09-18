public partial class HLPlayer
{
	public TimeSince TimeSinceSprayed { get; set; }

	static Color color = Color.Orange;
	public static void Spray()
	{
		if ( ConsoleSystem.Caller.Pawn is HLPlayer player ) 
		{
			if ( player.TimeSinceSprayed.Relative < HLGame.sv_spray_cooldown ) 
				return;

			var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * HLGame.sv_spray_max_distance )
				.WorldOnly()
				.Run();

			if ( tr.Hit )
			{
				Sound.FromWorld( "sprayer", tr.EndPosition );

				var _ = "materials/hl1/spraypaint/"+HLGame.hl_spray_icon+".decal";
				if (ResourceLibrary.TryGet<DecalDefinition>(_, out var decal)){
					Decal.Place(decal, tr, color);
					player.TimeSinceSprayed = 0;
				}
				else
				{
					Log.Error("Decal not found!");
				}
			}
		}
	}

	

}
