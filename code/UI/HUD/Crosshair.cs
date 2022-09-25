using Sandbox.UI;
using Sandbox.UI.Construct;

public class Crosshair : Panel
{
	public IconPanel Icon;

	public string prevWeapon;

	public Crosshair()
	{
		Icon = Add.Icon( string.Empty, "icon" );
	}

	public override void Tick()
	{
		base.Tick();

		var p = Local.Pawn as HLPlayer;
		

		if ( p.ActiveChild is HLWeapon weapon && prevWeapon != weapon.ClassName)
		{
			Icon.RemoveClass(prevWeapon);
			Icon.AddClass(weapon.ClassName);
			prevWeapon = weapon.ClassName;
		}
	}
}
