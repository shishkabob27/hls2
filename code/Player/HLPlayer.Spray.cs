public partial class HLPlayer
{
	public TimeSince TimeSinceSprayed { get; set; }

	public static void Spray()
	{
		if ( ConsoleSystem.Caller.Pawn is HLPlayer player ) 
		{
			//if ( player.TimeSinceSprayed < sv_spray_cooldown ) 
				//return;

			var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * sv_spray_max_distance )
				.WorldOnly()
				.Run();

			if ( tr.Hit )
			{
				Sound.FromWorld( "sprayer", tr.EndPosition );
				if (ResourceLibrary.TryGet<DecalDefinition>("materials/hl1/spraypaint/lambda.decal", out var decal)){
					Decal.Place(decal, tr);

					player.TimeSinceSprayed = 0;
				}
				else
				{
					Log.Error("Decal not found!");
				}
			}
		}
	}

	[ConVar.Replicated] public static float sv_spray_max_distance { get; set; } = 100;
	[ConVar.Replicated] public static float sv_spray_cooldown { get; set; } = 1;

}
