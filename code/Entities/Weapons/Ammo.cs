partial class BaseAmmo : HLMovement, IRespawnableEntity
{
	public virtual AmmoType AmmoType => AmmoType.None;
	public virtual int AmmoAmount => 17;
	public virtual Model WorldModel => Model.Load( "models/dm_battery.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		UsePhysicsCollision = true;

		Tags.Add( "weapon" );
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


[Library( "ammo_9mmclip" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_9mmclip.vmdl" )]
[Title( "9mm Clip" ), Category( "Ammo" )]
partial class Ammo9mmClip : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 17;
	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_9mmclip.vmdl" );

}

[Library( "ammo_glockclip" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_9mmclip.vmdl" )]
[Title( "Glock Clip" ), Category( "Ammo" )]
partial class AmmoGlockClip : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 17;
	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_9mmclip.vmdl" );

}

[Library( "ammo_9mmbox" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_9mmbox.vmdl" )]
[Title( "9mm Box" ), Category( "Ammo" )]
partial class Ammo9mmBox : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 200;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_9mmbox.vmdl" );
}

[Library( "ammo_9mmAR" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_9mmar.vmdl" )]
[Title( "9mm AR" ), Category( "Ammo" )]
partial class Ammo9mmAR : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 20;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_9mmar.vmdl" );
}

[Library( "ammo_mp5clip" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_9mmar.vmdl" )]
[Title( "Ammo MP5 Clip" ), Category( "Ammo" )]
partial class AmmoMP5clip : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override int AmmoAmount => 20;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_9mmar.vmdl" );
}


[Library( "ammo_buckshot" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_shotbox.vmdl" )]
[Title( "Buckshot" ), Category( "Ammo" )]
partial class AmmoBuckshot : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Buckshot;
	public override int AmmoAmount => 12;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_shotbox.vmdl" );
}

[Library( "ammo_357" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_357ammobox.vmdl" )]
[Title( "357 Ammo" ), Category( "Ammo" )]
partial class Ammo357 : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Python;
	public override int AmmoAmount => 6;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_357ammobox.vmdl" );
}

[Library( "ammo_crossbow" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_crossbow_clip.vmdl" )]
[Title( "Crossbow Bolts" ), Category( "Ammo" )]
partial class AmmoCrossbow : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Crossbow;
	public override int AmmoAmount => 5;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_crossbow_clip.vmdl" );
}

[Library( "ammo_uranium" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_gaussammo.vmdl" )]
[Title( "Uranium Ammo" ), Category( "Ammo" )]
partial class AmmoUranium : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Uranium;
	public override int AmmoAmount => 5;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_gaussammo.vmdl" );
}

[Library( "ammo_gaussclip" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_gaussammo.vmdl" )]
[Title( "Gauss Ammo Clip" ), Category( "Ammo" )]
partial class AmmoGaussClip : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Uranium;
	public override int AmmoAmount => 5;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_gaussammo.vmdl" );
}

[Library( "ammo_rockets" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_rpgammo.vmdl" )]
[Title( "RPG Ammo" ), Category( "Ammo" )]
partial class AmmoRPG : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.RPG;
	public override int AmmoAmount => 5;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_rpgammo.vmdl" );
}

[Library( "ammo_rpgclip" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_rpgammo.vmdl" )]
[Title( "RPG Ammo Clip" ), Category( "Ammo" )]
partial class AmmoRPGClip : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.RPG;
	public override int AmmoAmount => 5;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_rpgammo.vmdl" );
}

[Library( "ammo_ARgrenades" ), HammerEntity]
[EditorModel( "models/hl1/weapons/ammo/w_argrenade.vmdl" )]
[Title( "SMG Grenade" ), Category( "Ammo" )]
partial class AmmoSMGGrenade : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.SMGGrenade;
	public override int AmmoAmount => 3;

	public override Model WorldModel => Model.Load( "models/hl1/weapons/ammo/w_argrenade.vmdl" );
}

//Opposing Force

[Library( "ammo_762" ), HammerEntity]
[EditorModel( "models/op4/weapons/ammo/w_m40a1clip.vmdl" )]
[Title( "Sniper Ammo" ), Category( "Ammo" )]
partial class AmmoSniper : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.Sniper;
	public override int AmmoAmount => 5;

	public override Model WorldModel => Model.Load( "models/op4/weapons/ammo/w_m40a1clip.vmdl" );
}

[Library( "ammo_556" ), HammerEntity]
[EditorModel( "models/op4/weapons/ammo/w_saw_clip.vmdl" )]
[Title( "Sniper Ammo" ), Category( "Ammo" )]
partial class AmmoM249 : BaseAmmo
{
	public override AmmoType AmmoType => AmmoType.M249;
	public override int AmmoAmount => 50;

	public override Model WorldModel => Model.Load( "models/op4/weapons/ammo/w_saw_clip.vmdl" );
}