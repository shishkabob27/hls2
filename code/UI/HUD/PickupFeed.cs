using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class PickupFeed : Panel
{
	public static PickupFeed Current;

	public PickupFeed()
	{
		Current = this;
	}

	/// <summary>
	/// An RPC which can be called from the server 
	/// </summary>
	[ClientRpc]
	public static void OnPickupItem( string icon )
	{
		Current?.AddItemEntry( icon );
	}

	/// <summary>
	/// An RPC which can be called from the server 
	/// </summary>
	[ClientRpc]
	public static void OnPickupWeapon( string icon )
	{
		Current?.AddWeaponEntry( icon );
	}

	/// <summary>
	/// An RPC which can be called from the server 
	/// </summary>
	[ClientRpc]
	public static void OnPickupAmmo( string icon, int amount )
	{
		Current?.AddAmmoEntry( icon, amount );
	}

	/// <summary>
	/// Spawns a label, waits for half a second and then deletes it
	/// The :outro style in the scss keeps it alive and fades it out
	/// </summary>
	private async Task AddItemEntry( string icon )
	{
		var panel = Current.Add.Panel( "entryIcon" );
		panel.Style.SetBackgroundImage( icon );

		await Task.DelayRealtimeSeconds( 2.0f );
		panel.Delete();
	}

	private async Task AddWeaponEntry( string icon )
	{
		var panel = Current.Add.Panel( "entry" );
		panel.Style.SetBackgroundImage(icon);
		panel.AddClass( icon ); ;
		panel.Add.Panel( "icon" );
		await Task.DelayRealtimeSeconds( 2.0f );
		panel.Delete();
	}

	private async Task AddAmmoEntry( string icon, int amount )
	{
		var panel = Current.Add.Panel( "entryAmmo" );

		panel.Add.Label( $"{amount}" );

		var ammoicon = "";

		switch ( icon )
		{
			case "Pistol": ammoicon = "ui/ammo1.png"; break;
			case "Python": ammoicon = "ui/ammo2.png"; break;
			case "Buckshot": ammoicon = "ui/ammo4.png"; break;
			case "Crossbow": ammoicon = "ui/ammo5.png"; break;
			case "Rpg": ammoicon = "ui/ammo6.png"; break;
			case "Uranium": ammoicon = "ui/ammo7.png"; break;
			case "Grenade": ammoicon = "ui/ammo1.png"; break;
			case "Satchel": ammoicon = "ui/ammo10.png"; break;
			case "Tripmine": ammoicon = "ui/ammo12.png"; break;
			case "Snark": ammoicon = "ui/ammo11.png"; break;
			case "Hornet": ammoicon = "ui/ammo8.png"; break;
			default: ammoicon = "ui/ammo1.png"; break;
		}

		var iconPanel = panel.Add.Panel( "icon" );
		iconPanel.Style.SetBackgroundImage( ammoicon );

		await Task.DelayRealtimeSeconds( 2.0f );
		panel.Delete();
	}
}
