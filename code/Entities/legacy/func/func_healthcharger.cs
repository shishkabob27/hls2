﻿/// <summary>
/// A wall-mounted device that gives a limited amount of health
/// </summary>
[Library( "func_healthcharger" ), HammerEntity]
[Solid]
[Category("Gameplay")]
partial class func_healthcharger : KeyframeEntity, IUse
{
	/// <summary>
	/// This controls the amount of charge in the station, Default Value is 50.
	/// </summary>
	[Net]
	[Property( "chargerpower", Title = "Charger Power" )]
	public float DefaultChargerPower { get; set; } = 50f;

	[Net]
	public float ChargerPower { get; set; }

	/// <summary>
	/// This controls the time it takes for the charger to refill, Default Value is 60.
	/// </summary>
	[Net]
	[Property( "chargerresettime", Title = "Charger Reset Time" )]
	public float ChargerResetTime { get; set; } = 60f;

	/// <summary>
	/// This controls the time it takes for the charger to refill, Default Value is 60.
	/// </summary>
	[Net]
	[Property( "armourcharger", Title = "Is Armour Charger" )]
	public bool IsArmourCharger { get; set; } = false;

	private TimeSince TimeSinceUsed;

	public PickupTrigger PickupTrigger { get; protected set; }

	public bool CanUse;

	[Net]
	public Vector3 Mins { get; set; } = new Vector3( -32, -32, -32 );

	[Net]
	public Vector3 Maxs { get; set; } = new Vector3( 32, 32, 32 );

	public override void Spawn()
	{
		base.Spawn();

		ChargerPower = DefaultChargerPower;

		SetupPhysicsFromModel( PhysicsMotionType.Static );

		var trigger = new BaseTrigger();
		trigger.SetParent( this, null, Transform.Zero );
		trigger.SetupPhysicsFromOBB( PhysicsMotionType.Static, Mins, Maxs );
		trigger.Transmit = TransmitType.Always;
		trigger.EnableTouchPersists = true;
	}

	public bool IsUsable( Entity user )
	{
		return true;
	}

	public bool OnUse( Entity user )
	{
		// no power, no health
		if ( ChargerPower <= 0 )
		{
			SetState( false );
			return false;
		}

		if ( CanUse == false ) return false;

		if ( user is not HLPlayer player )
			return false;


		if ( !IsArmourCharger && player.Health >= 100 ) return false;
		if ( IsArmourCharger && player.Armour >= 100 ) return false;

		// standard rate of 10 health per second
		var add = 10 * Time.Delta;

		// check if charger has enough power to heal
		if ( add > ChargerPower )
			add = ChargerPower;

		TimeSinceUsed = 0;
		ChargerPower -= add;

		if ( IsArmourCharger )
		{
			player.Armour += add;
			player.Armour.Clamp( 0, 100 );
			return player.Armour < 100;
		}

		if ( !IsArmourCharger )
		{
			player.Health += add;
			player.Health.Clamp( 0, 100 );
			return player.Health < 100;
		}

		return false;
	}

	public override void StartTouch( Entity other )
	{
		if ( other is not HLPlayer player ) return;
		CanUse = true;
	}

	public override void EndTouch( Entity other )
	{
		if ( other is not HLPlayer player ) return;
		CanUse = false;
	}

	public void SetState( bool state )
	{
		if ( state )
		{
			//Full
			PlaySound( "dm.item_charger_full" );
		}
		else
		{
			//Empty
			PlaySound( "medshotno1" );
		}
	}

	[Event.Tick.Server]
	private void Tick()
	{
		if ( TimeSinceUsed >= ChargerResetTime && ChargerPower <= 0 )
		{
			SetState( true );
			ChargerPower = DefaultChargerPower;
		}
	}

	[Event.Tick.Client]
	private void ClientTick()
	{

		SceneObject?.Attributes.Set( "PowerCharge", (ChargerPower / DefaultChargerPower) * .5f );
	}

}
