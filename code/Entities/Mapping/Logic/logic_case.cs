[Library( "logic_case" )]
[HammerEntity]
[VisGroup( VisGroup.Logic )]
[EditorSprite( "editor/logic_case.vmat" )]
[Title( "logic_case" ), Category( "Logic" ), Icon( "calculate" )]
public partial class logic_case : Entity
{
	[Property] public bool Enabled { get; set; } = true; 

	[Input]
	public void Enable()
	{
		Enabled = true;
	}

	[Input]
	public void Disable()
	{
		Enabled = false;
	}

	[Input]
	public void Toggle()
	{
		Enabled = !Enabled;
	}

	/// <summary>
	/// Fired when the this entity receives the "Trigger" input.
	/// </summary>
	protected Output OnTrigger { get; set; }


	/// <summary>
	/// Choose a random case
	/// </summary>

	[Input]
	public void PickRandom()
	{
		if ( !Enabled ) return; 
		Tuple<Output, string>[] Cases 
			= 
		{ 
		new Tuple<Output, string>( OnCase01, Case01 ), 
		new Tuple<Output, string>( OnCase02, Case02 ),
		new Tuple<Output, string>( OnCase03, Case03 ),
		new Tuple<Output, string>( OnCase04, Case04 ),
		new Tuple<Output, string>( OnCase05, Case05 ),
		new Tuple<Output, string>( OnCase06, Case06 ),
		new Tuple<Output, string>( OnCase07, Case07 ),
		new Tuple<Output, string>( OnCase08, Case08 ),
		new Tuple<Output, string>( OnCase09, Case09 ),
		new Tuple<Output, string>( OnCase10, Case10 ),
		new Tuple<Output, string>( OnCase11, Case11 ),
		new Tuple<Output, string>( OnCase12, Case12 ),
		new Tuple<Output, string>( OnCase13, Case13 ),
		new Tuple<Output, string>( OnCase14, Case14 ),
		new Tuple<Output, string>( OnCase15, Case15 ),
		new Tuple<Output, string>( OnCase16, Case16 ),
		};

		var CasesList = new List<Tuple<Output, string>>( Cases );
		var a = Game.Random.Int(1,16);
		var b = CasesList.OrderBy( x => Game.Random.Float( 0, 1000 ));
		foreach ( var Outcase in b)
		{
			if (Outcase.Item2 != "")
			{
				Outcase.Item1.Fire(this);
				return;
			}
		}



	}
	List<Tuple<Output, string>> Shuffled = new();

	[Input]
	public void PickRandomShuffle()
	{
		if ( !Enabled ) return; 
		Tuple<Output, string>[] Cases 
			= 
		{ 
		new Tuple<Output, string>( OnCase01, Case01 ), 
		new Tuple<Output, string>( OnCase02, Case02 ),
		new Tuple<Output, string>( OnCase03, Case03 ),
		new Tuple<Output, string>( OnCase04, Case04 ),
		new Tuple<Output, string>( OnCase05, Case05 ),
		new Tuple<Output, string>( OnCase06, Case06 ),
		new Tuple<Output, string>( OnCase07, Case07 ),
		new Tuple<Output, string>( OnCase08, Case08 ),
		new Tuple<Output, string>( OnCase09, Case09 ),
		new Tuple<Output, string>( OnCase10, Case10 ),
		new Tuple<Output, string>( OnCase11, Case11 ),
		new Tuple<Output, string>( OnCase12, Case12 ),
		new Tuple<Output, string>( OnCase13, Case13 ),
		new Tuple<Output, string>( OnCase14, Case14 ),
		new Tuple<Output, string>( OnCase15, Case15 ),
		new Tuple<Output, string>( OnCase16, Case16 ),
		};
		if (Shuffled.Count == 0)
		{
			Shuffled = new List<Tuple<Output, string>>( Cases );
			var a = Game.Random.Int( 1, 16 );
			Shuffled = Shuffled.OrderBy( x => Game.Random.Float( 0, 1000 ) ).ToList();
		}
		foreach ( var Outcase in Shuffled)
		{
			if (Outcase.Item2 != "")
			{
				Outcase.Item1.Fire(this);
				Shuffled.Remove( Outcase );
				return;
			}
		}



	}

	/// <summary>
	/// Run a value through the switch
	/// </summary>
	[Input]
	public void InValue(string VALUE)
	{
		if ( !Enabled ) return;
		// else if chain, switch cases only work with constants
		if (VALUE == Case01) { OnCase01.Fire(this); } else
		if (VALUE == Case02) { OnCase02.Fire(this); } else
		if (VALUE == Case03) { OnCase03.Fire(this); } else
		if (VALUE == Case04) { OnCase04.Fire(this); } else
		if (VALUE == Case05) { OnCase05.Fire(this); } else
		if (VALUE == Case06) { OnCase06.Fire(this); } else
		if (VALUE == Case07) { OnCase07.Fire(this); } else
		if (VALUE == Case08) { OnCase08.Fire(this); } else
		if (VALUE == Case09) { OnCase09.Fire(this); } else
		if (VALUE == Case10) { OnCase10.Fire(this); } else
		if (VALUE == Case11) { OnCase11.Fire(this); } else
		if (VALUE == Case12) { OnCase12.Fire(this); } else
		if (VALUE == Case13) { OnCase13.Fire(this); } else
		if (VALUE == Case14) { OnCase14.Fire(this); } else
		if (VALUE == Case15) { OnCase15.Fire(this); } else
		if (VALUE == Case16) { OnCase16.Fire(this); } else
		{ OnDefault.Fire(this); } 
	}

	[Property] public string Case01 { get; set; } = "";
	[Property] public string Case02 { get; set; } = "";
	[Property] public string Case03 { get; set; } = "";
	[Property] public string Case04 { get; set; } = "";
	[Property] public string Case05 { get; set; } = "";
	[Property] public string Case06 { get; set; } = "";
	[Property] public string Case07 { get; set; } = "";
	[Property] public string Case08 { get; set; } = "";
	[Property] public string Case09 { get; set; } = "";
	[Property] public string Case10 { get; set; } = "";
	[Property] public string Case11 { get; set; } = "";
	[Property] public string Case12 { get; set; } = "";
	[Property] public string Case13 { get; set; } = "";
	[Property] public string Case14 { get; set; } = "";
	[Property] public string Case15 { get; set; } = "";
	[Property] public string Case16 { get; set; } = "";

	protected Output OnCase01 { get; set; }
	protected Output OnCase02 { get; set; }
	protected Output OnCase03 { get; set; }
	protected Output OnCase04 { get; set; }
	protected Output OnCase05 { get; set; }
	protected Output OnCase06 { get; set; }
	protected Output OnCase07 { get; set; }
	protected Output OnCase08 { get; set; }
	protected Output OnCase09 { get; set; }
	protected Output OnCase10 { get; set; }
	protected Output OnCase11 { get; set; }
	protected Output OnCase12 { get; set; }
	protected Output OnCase13 { get; set; }
	protected Output OnCase14 { get; set; }
	protected Output OnCase15 { get; set; }
	protected Output OnCase16 { get; set; }
	protected Output OnDefault { get; set; }
}
