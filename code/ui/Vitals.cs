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
		this.Style.Width = 1050 + (Screen.Height / Screen.Width) * -1200 ;
		var player = Local.Pawn as HLPlayer;
		if ( player == null ) return;

		Value.Text = $"{player.Health.CeilToInt()}";

		SetClass( "low", player.Health < 40.0f );
		SetClass( "empty", player.Health <= 0.0f );

		if (!player.HasHEV)
		{
			SetClass( "invisible", true);
		}
		else{
			SetClass( "invisible", false);
		}
	}
}

public class ArmourHud : Panel
{
	//public IconPanel ArmourBar;

	public Panel IconContainer;
	public IconPanel IconFull;
	public IconPanel IconEmpty;

	public Label Value;

	public ArmourHud()
	{
		IconContainer = Add.Panel("iconcontainer");
		
		IconEmpty = Add.Icon( string.Empty, "iconempty" );
		IconFull = Add.Icon( string.Empty, "iconfull" );

		Value = Add.Label( "0", "label" );

		IconContainer.AddChild(IconEmpty);
		IconContainer.AddChild(IconFull);
	}

	public override void Tick()
	{
		var player = Local.Pawn as HLPlayer;
		if ( player == null ) return;

		Value.Text = $"{player.Armour.CeilToInt()}";

		IconEmpty.Style.Height = 100 - player.Armour;
		IconFull.Style.Height = player.Armour;

		if (!player.HasHEV)
		{
			SetClass( "invisible", true);
		}
		else{
			SetClass( "invisible", false);
		}
	}
}
