partial class HLPlayer 
{

	public void SetPlayerModel()
	{
		var pm = "";
		switch ( Client.GetClientData( "hl_pm" ) )
		{
			case "player": pm = "models/hl1/player/player.vmdl"; break; // helmet is a different model, it has colour support and this doesn't. I like this better so add it here since yeah
			case "barney": pm = "models/hl1/player/barney.vmdl"; break;
			case "gina": pm = "models/hl1/player/gina.vmdl"; break;
			case "gman": pm = "models/hl1/player/gman.vmdl"; break;
			case "gordon": pm = "models/hl1/player/gordon.vmdl"; break;
			case "helmet": pm = "models/hl1/player/helmet.vmdl"; break;
			case "hgrunt": pm = "models/hl1/player/hgrunt.vmdl"; break;
			case "robo": pm = "models/hl1/player/robo.vmdl"; break;
			case "scientist": pm = "models/hl1/player/scientist.vmdl"; break;
			case "zombie": pm = "models/hl1/player/zombie.vmdl"; break;
			case "freeman": pm = "freeman"; break;
			case "citizen": pm = "citizen"; break;
			default: pm = "models/hl1/player/player.vmdl"; break;
		}

		if (Clothing != null) Clothing.ClearEntities();
		if ( pm == "freeman" )
		{
			SetModel( "models/hl1/player/player.vmdl" );
			SetAnimGraph( "animgraphs/hl1/player.vanmgrph" );
			updBDG( "head", 0 );
		}
		else if ( pm == "citizen" )
		{
			SetModel( "models/citizen/citizen.vmdl" ); 
			SetAnimGraph( "models/citizen/citizen.vanmgrph" );
			UpdateClothes( Client );
			Clothing.DressEntity( this );
		}
		else
		{
			updBDG( "head", 1 );
			SetAnimGraph( "animgraphs/hl1/player.vanmgrph" );
			SetModel( pm );
		}
		updateColours();

	}
	public ClothingContainer Clothing { get; set; }

	/// <summary>
	/// Set the clothes to whatever the player is wearing
	/// </summary>
	public void UpdateClothes( Client cl )
	{
		Clothing ??= new();
		Clothing.LoadFromClient( cl );
	}

	async void updBDG( String bdg, int i )
	{

		await GameTask.DelaySeconds( 0.1f );
		SetBodyGroup( bdg, i );
	}
	[ClientRpc]
	void updateColours()
	{

		var a = HSVtoRGB( Client.GetClientData( "hl_pm_colour1" ).ToInt(), 100, 100 );
		SceneObject.Attributes.Set( "clTintR", a.r );
		SceneObject.Attributes.Set( "clTintG", a.g );
		SceneObject.Attributes.Set( "clTintB", a.b );
	}

	Color HSVtoRGB( float H, float S, float V )
	{
		if ( H > 360 || H < 0 || S > 100 || S < 0 || V > 100 || V < 0 )
		{
			Log.Info( "invalid range" );
		}
		float s = S / 100;
		float v = V / 100;
		float C = s * v;
		float X = C * (1 - Math.Abs( (H / 60.0f % 2) - 1 ));
		float m = v - C;
		float r, g, b;
		if ( H >= 0 && H < 60 )
		{
			r = C;
			g = X;
			b = 0;
		}
		else if ( H >= 60 && H < 120 )
		{
			r = X;
			g = C;
			b = 0;
		}
		else if ( H >= 120 && H < 180 )
		{
			r = 0;
			g = C;
			b = X;
		}
		else if ( H >= 180 && H < 240 )
		{
			r = 0;
			g = X;
			b = C;
		}
		else if ( H >= 240 && H < 300 )
		{
			r = X;
			g = 0;
			b = C;
		}
		else
		{
			r = C;
			g = 0;
			b = X;
		}
		float R = (r + m) * 255;
		float G = (g + m) * 255;
		float B = (b + m) * 255;
		return new Color( R, G, B );
	}



}
