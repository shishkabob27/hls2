﻿using Sandbox.UI;

class InventoryIcon : Panel
{
	public HLWeapon Weapon;
	public Panel Icon;
	public Panel AmmoCountFull;
	public Panel AmmoCountEmpty;

	public InventoryIcon( HLWeapon weapon )
	{
		Weapon = weapon;
		Icon = Add.Panel( "icon" );
		AmmoCountFull = Add.Panel( "ammocountf" );
		AmmoCountEmpty = Add.Panel( "ammocounte" );
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

		if ( Local.Pawn is HLPlayer ply && ply.MaxAmmo( Weapon.AmmoType ) != 0 )
		{
			float a = ((float)ply.AmmoCount( Weapon.AmmoType ) / (float)ply.MaxAmmo( Weapon.AmmoType ));
			float b = 1 - ((float)ply.AmmoCount( Weapon.AmmoType ) / (float)ply.MaxAmmo( Weapon.AmmoType ));
			AmmoCountFull.Style.Width = a * 24;
			AmmoCountEmpty.Style.Width = b * 24;
		}
	}
}
