using Sandbox.UI;
using Sandbox.UI.Construct;

public class ArmourHud : Panel
{
	//public IconPanel ArmourBar;

	public Panel IconContainer;
	public IconPanel IconFull;
	public IconPanel IconEmpty;

	public Label Value;

	public ArmourHud()
	{
		IconContainer = Add.Panel( "iconcontainer" );

		IconEmpty = Add.Icon( string.Empty, "iconempty" );
		IconFull = Add.Icon( string.Empty, "iconfull" );

		Value = Add.Label( "0", "label" );

		IconContainer.AddChild( IconEmpty );
		IconContainer.AddChild( IconFull );
	}

	public override void Tick()
	{
		var player = Local.Pawn as HLPlayer;
		if ( player == null ) return;

		Value.Text = $"{player.Armour.CeilToInt()}";

		IconEmpty.Style.Height = 100 - player.Armour;
		IconFull.Style.Height = player.Armour;

		if ( !player.HasHEV || player.Health <= 0 )
		{
			SetClass( "invisible", true );
		}
		else
		{
			SetClass( "invisible", false );
		}
	}
}
