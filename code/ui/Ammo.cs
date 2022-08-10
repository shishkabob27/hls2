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

		Seperator = Add.Icon(string.Empty, "seperator");
		Inventory = Add.Label( "100", "inventory" );
		//TODO - AMMO ICON HERE
		AltAmmoInventory = Add.Label("100", "altammocount");
		//TODO - ALT AMMO ICON HERE

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
		Inventory.SetClass( "invisible", weapon.ClipSize <= 0);

		var clip = weapon.AmmoClip;
		AmmoCount.Text = $"{clip}";
		AmmoCount.SetClass("active", clip >= 0);

		Seperator.SetClass("invisible", weapon.ClipSize <= 1);
		if (weapon.ClipSize <= 1)
			AmmoCount.Text = $" "; // hide it, since it would be a grenade or a tripmine TODO - Better way to do this? add a ShowClip bool in HLWeapon maybe?	            



		AltAmmoInventory.SetClass("invisible", !weapon.HasAltAmmo);
		AltAmmoInventory.Text = $"";

		if (weapon.HasAltAmmo)
		{
			var altinv = weapon.AvailableAltAmmo() + weapon.AltAmmoClip;
			AltAmmoInventory.Text = $"{altinv}";
			AltAmmoInventory.SetClass("active", altinv >= 0);
		}


	}


}
