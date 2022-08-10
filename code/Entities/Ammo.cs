﻿partial class BaseAmmo : ModelEntity, IRespawnableEntity
{
	//public static Model WorldModel = Model.Load( "models/dm_battery.vmdl" );

	public virtual AmmoType AmmoType => AmmoType.None;
	public virtual int AmmoAmount => 17;
	public virtual Model WorldModel => Model.Load( "models/dm_battery.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		Tags.Add("weapon");
		SetInteractsAs( CollisionLayer.Debris );
	}

	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other is not HLPlayer player )
			return;

		if ( other.LifeState != LifeState.Alive )
			return;

		var ammoTaken = player.GiveAmmo( AmmoType, AmmoAmount );

		if ( ammoTaken == 0 )
			return;

		Sound.FromWorld( "dm.pickup_ammo", Position );
		PickupFeed.OnPickup( To.Single( player ), $"+{ammoTaken} {AmmoType}" );

		ItemRespawn.Taken( this );
		Delete();
	}
}


[Library( "hl_ammo9mmclip" ), HammerEntity]
[EditorModel( "models/dm_ammo_9mmclip.vmdl" )]
[Title( "9mm Clip" ), Category( "Ammo" )]
partial class Ammo9mmClip : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 17;
	public override Model WorldModel => Model.Load( "models/dm_ammo_9mmclip.vmdl" );

}

[Library( "hl_ammo9mmbox" ), HammerEntity]
[EditorModel( "models/dm_ammo_9mmbox.vmdl" )]
[Title( "9mm Box" ), Category( "Ammo" )]
partial class Ammo9mmBox : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 200;

	public override Model WorldModel => Model.Load( "models/dm_ammo_9mmbox.vmdl" );
}



[Library( "hl_ammobuckshot" ), HammerEntity]
[EditorModel( "models/dm_ammo_buckshot.vmdl" )]
[Title( "Buckshot" ), Category( "Ammo" )]
partial class AmmoBuckshot : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Buckshot;
	public override int AmmoAmount => 12;

	public override Model WorldModel => Model.Load( "models/dm_ammo_buckshot.vmdl" );
}

[Library( "hl_ammo357" ), HammerEntity]
[EditorModel( "models/dm_ammo_357.vmdl" )]
[Title( "357 Ammo" ), Category( "Ammo" )]
partial class Ammo357 : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Python;
	public override int AmmoAmount => 6;

	public override Model WorldModel => Model.Load( "models/dm_ammo_357.vmdl" );
}

[Library( "hl_ammocrossbow" ), HammerEntity]
[EditorModel( "models/dm_ammo_crossbow.vmdl" )]
[Title( "Crossbow Bolts" ), Category( "Ammo" )]
partial class AmmoCrossbow : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Crossbow;
	public override int AmmoAmount => 5;

	public override Model WorldModel => Model.Load( "models/dm_ammo_crossbow.vmdl" );
}
