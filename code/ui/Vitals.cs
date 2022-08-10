using Sandbox.UI;
using Sandbox.UI.Construct;

public class HealthHud : Panel
{
	public IconPanel Icon;
	public Label Value;
	public IconPanel Seperator;

	public HealthHud()
	{
		Icon = Add.Icon( string.Empty, "icon" );
		Value = Add.Label( "0", "label" );
		Seperator = Add.Icon( string.Empty, "seperator" );
	}

	public override void Tick()
	{
		var player = Local.Pawn as HLPlayer;
		if ( player == null ) return;

		Value.Text = $"{player.Health.CeilToInt()}";

		SetClass( "low", player.Health < 40.0f );
		SetClass( "empty", player.Health <= 0.0f );
	}
}

public class ArmourHud : Panel
{
	//public IconPanel ArmourBar;
	public IconPanel Icon;
	public Label Value;

	public ArmourHud()
	{
		Icon = Add.Icon( string.Empty, "icon" );
		Value = Add.Label( "0", "label" );

	}

	public override void Tick()
	{
		var player = Local.Pawn as HLPlayer;
		if ( player == null ) return;

		Value.Text = $"{player.Armour.CeilToInt()}";

		SetClass( "low", player.Armour < 40.0f );
		SetClass( "empty", player.Armour <= 0.0f );
	}
}
