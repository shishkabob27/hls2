using Sandbox.UI;
using Sandbox.UI.Construct;

public class Ammo : Panel
{
	public Label AmmoCount;
	public IconPanel Seperator;
	public Label Inventory;
	public IconPanel AmmoIcon1;
	public Label AltAmmoInventory;
	public IconPanel AmmoIcon2;


	public Ammo()
	{
		AmmoCount = Add.Label( "0", "ammocount" );

		Seperator = Add.Icon( string.Empty, "seperator" );
		Inventory = Add.Label( "0", "inventory" );
		AmmoIcon1 = Add.Icon( string.Empty, "ammoicon1" );

		//TODO - AMMO ICON HERE
		AltAmmoInventory = Add.Label( "0", "altammocount" );
		AmmoIcon2 = Add.Icon( string.Empty, "ammoicon2" );
		//TODO - ALT AMMO ICON HERE

	}

	public override void Tick()
	{
		Inventory.Text = "";

		AmmoCount.Text = "";
		AltAmmoInventory.Text = "";

		Seperator.SetClass( "invisible", true );

		var player = Local.Pawn as Player;
		if ( player == null ) return;

		var weapon = player.ActiveChild as HLWeapon;
		SetClass( "active", weapon != null );

		if ( weapon == null ) return;

		var inv = weapon.AvailableAmmo();
		Inventory.SetClass( "invisible", weapon.ClipSize == 0 || Inventory.Text == "0" );
		Inventory.Text = $"{inv}";

		var clip = weapon.AmmoClip;
		AmmoCount.Text = $"{clip}";
		AmmoCount.SetClass( "active", clip >= 0 );

		Seperator.SetClass( "invisible", weapon.ClipSize == 0 || Inventory.Text == "0" );

		AmmoIcon1.SetClass( "invisible", weapon.ClipSize == 0 );
		if ( weapon.ClipSize <= 0 )
		{
			AmmoCount.Text = $" "; // hide it, since it would be a grenade or a tripmine TODO - Better way to do this? add a ShowClip bool in HLWeapon maybe?	   

			Seperator.SetClass( "invisible", true );
		}
		AmmoIcon1.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, weapon.AmmoIcon, true );

		AltAmmoInventory.SetClass( "invisible", !weapon.HasAltAmmo );
		AmmoIcon2.SetClass( "invisible", !weapon.HasAltAmmo );


		if ( weapon.HasAltAmmo )
		{
			var altinv = weapon.AvailableAltAmmo() + weapon.AltAmmoClip;
			AltAmmoInventory.Text = $"{altinv}";
			AltAmmoInventory.SetClass( "active", altinv >= 0 );
			AmmoIcon2.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, weapon.AltAmmoIcon, true );
		}


	}


}
