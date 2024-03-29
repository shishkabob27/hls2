﻿partial class HLInventory : BaseInventory
{


	public HLInventory( HLPlayer player ) : base( player )
	{

	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		var player = Owner as HLPlayer;
		var weapon = ent as Weapon;
		var notices = !player.SupressPickupNotices;

		if ( weapon == null )
			return false;

		weapon.OnPickup();
		//
		// We don't want to pick up the same weapon twice
		// But we'll take the ammo from it Winky Face
		//


		if ( weapon.WeaponIsAmmo || weapon.ClipSize < 0 )
		{
			var ammo2 = weapon.WeaponIsAmmoAmount;
			var ammoType2 = weapon.AmmoType;
			if ( ammo2 > 0 )
			{
				var taken = player.GiveAmmo( ammoType2, ammo2 );
				if ( taken == 0 )
					return false;

				if ( notices && taken > 0 )
				{
					Sound.FromWorld( "dm.pickup_ammo", ent.Position );
					PickupFeed.OnPickupAmmo( To.Single( player ), $"{ammoType2}", taken );
				}
			}
		}

		if ( weapon != null && IsCarryingType( ent.GetType() ) )
		{
			var ammo = weapon.AmmoClip;
			var ammoType = weapon.AmmoType;

			if ( ammo > 0 )
			{
				var taken = player.GiveAmmo( ammoType, ammo );
				if ( taken == 0 )
					return false;

				if ( notices && taken > 0 )
				{
					Sound.FromWorld( "dm.pickup_ammo", ent.Position );
					PickupFeed.OnPickupAmmo( To.Single( player ), $"{ammoType}", taken );
				}
			}

			ItemRespawn.Taken( ent );

			// Despawn it
			ent.Delete();
			return false;
		}

		if ( !base.Add( ent, makeActive ) )
			return false;

		if ( weapon != null && notices )
		{
			var display = DisplayInfo.For( ent );

			Sound.FromWorld( "dm.pickup_weapon", ent.Position );
			PickupFeed.OnPickupWeapon( To.Single( player ), weapon.InventoryIcon );
		}

		ItemRespawn.Taken( ent );

		return true;
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.IsValid() && x.GetType() == t );
	}
}
