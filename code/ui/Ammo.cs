using Sandbox.UI;
using Sandbox.UI.Construct;

public class Ammo : Panel
{
	public Label AmmoCount;
	public IconPanel Seperator;
	public Label Inventory;
	public Label AltAmmoInventory;
 

	public Ammo()
	{
		AmmoCount = Add.Label("100", "ammocount");

		Seperator = Add.Icon(string.Empty, "seperator2");
		Inventory = Add.Label( "100", "inventory" );

	}

	int weaponHash;

	public override void Tick()
	{
		var player = Local.Pawn as Player;
		if ( player == null ) return;

		var weapon = player.ActiveChild as HLWeapon;
		SetClass( "active", weapon != null );

		if ( weapon == null ) return;

		var inv = weapon.AvailableAmmo();
		Inventory.Text = $"{inv}";
		Inventory.SetClass( "active", inv >= 0 );

		var clip = weapon.AmmoClip;
		AmmoCount.Text = $"{clip}";
		AmmoCount.SetClass("active", clip >= 0);
        if (weapon.ClipSize == 1)
			AmmoCount.Text = $" "; // hide it, since it would be a grenade or a tripmine


	}


}
