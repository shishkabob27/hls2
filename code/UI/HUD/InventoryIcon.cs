using Sandbox.UI;

class InventoryIcon : Panel
{
	public HLWeapon Weapon;
	public Panel Icon;

	public InventoryIcon( HLWeapon weapon )
	{
		Weapon = weapon;
		Icon = Add.Panel( "icon" );
		Icon.Style.SetBackgroundImage( weapon.InventoryIcon );
	}

	internal void TickSelection( HLWeapon selectedWeapon )
	{
		SetClass( "active", selectedWeapon == Weapon );
		if ( selectedWeapon == Weapon )
		{
			Icon.Style.SetBackgroundImage( Weapon.InventoryIconSelected );
		}
		else
		{
			Icon.Style.SetBackgroundImage( Weapon.InventoryIcon );
		}
		SetClass( "empty", !Weapon?.IsUsable() ?? true );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !Weapon.IsValid() || Weapon.Owner != Local.Pawn )
			Delete( true );
	}
}
