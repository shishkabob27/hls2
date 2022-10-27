partial class Coffin : ModelEntity
{
	public static readonly Model WorldModel = Model.Load( "models/hl1/gameplay/coffin.vmdl" );

	public List<string> Weapons = new List<string>();
	public List<int> Ammos = new List<int>();

	public override void Spawn()
	{
		Components.Create<Movement>();

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed, false );
		base.Spawn();

		Model = WorldModel;
		Tags.Add( "weapon" );
	}

	public void Populate( HLPlayer player )
	{
		Ammos.AddRange( player.Ammo );

		foreach ( var child in player.Children.ToArray() )
		{
			if ( child is Weapon weapon )
			{
				Weapons.Add( weapon.ClassName );
			}
		}
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( IsClient )
			return;

		if ( other is not HLPlayer player )
			return;

		if ( player.LifeState == LifeState.Dead )
			return;

		Sound.FromWorld( "dm.pickup_ammo", Position );

		foreach ( var weapon in Weapons )
		{
			player.Give( weapon );
		}

		for ( int i = 0; i < Ammos.Count; i++ )
		{
			int taken = player.GiveAmmo( (AmmoType)i, Ammos[i] );
			if ( taken > 0 )
			{
				PickupFeed.OnPickup( To.Single( player ), $"+{taken} {((AmmoType)i)}" );
			}
		}

		Delete();
	}

}
