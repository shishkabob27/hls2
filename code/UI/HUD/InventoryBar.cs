using Sandbox.UI;

/// <summary>
/// The main inventory panel, top left of the screen.
/// </summary>
public class InventoryBar : Panel
{
	List<InventoryColumn> columns = new();
	List<Weapon> Weapons = new();

	public bool IsOpen;
	Weapon SelectedWeapon;
	int invslots = 7;
	Sound CurrentSound;

	public InventoryBar()
	{

		for ( int i = 0; i < invslots; i++ )
		{
			var icon = new InventoryColumn( i, this );
			columns.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();
		var player = Local.Pawn as HLPlayer;
		SetClass( "active", IsOpen );

		if ( player == null ) return;
		if ( !player.HasHEV ) return;

		Weapons.Clear();
		Weapons.AddRange( player.Children.Select( x => x as Weapon ).Where( x => x.IsValid() && x.IsUsable() ) );

		foreach ( var weapon in Weapons )
		{
			columns[weapon.Bucket].UpdateWeapon( weapon );
		}
	}
	TimeSince SinceSelectedWeapon;
	/// <summary>
	/// IClientInput implementation, calls during the client input build.
	/// You can both read and write to input, to affect what happens down the line.
	/// </summary>
	[Event.BuildInput]
	public void ProcessClientInput()
	{
		if ( HLGame.CurrentState != HLGame.GameStates.Live ) return;

		bool wantOpen = IsOpen;
		var localPlayer = Local.Pawn as HLPlayer;

		if ( !localPlayer.HasHEV ) return;

		// If we're not open, maybe this input has something that will 
		// make us want to start being open?
		wantOpen = wantOpen || Input.MouseWheel != 0;
		wantOpen = wantOpen || Input.Pressed( InputButton.SlotNext ) || Input.VR.RightHand.Joystick.Value.y < -0.5;
		wantOpen = wantOpen || Input.Pressed( InputButton.SlotPrev ) || Input.VR.RightHand.Joystick.Value.y > 0.5;
		wantOpen = wantOpen || Input.Pressed( InputButton.Slot1 );
		wantOpen = wantOpen || Input.Pressed( InputButton.Slot2 );
		wantOpen = wantOpen || Input.Pressed( InputButton.Slot3 );
		wantOpen = wantOpen || Input.Pressed( InputButton.Slot4 );
		wantOpen = wantOpen || Input.Pressed( InputButton.Slot5 );
		wantOpen = wantOpen || Input.Pressed( InputButton.Slot6 );
		wantOpen = wantOpen || Input.Pressed( InputButton.Slot7 );

		bool wantOpen2 = IsOpen;

		wantOpen2 = wantOpen2 || Input.Pressed( InputButton.Slot1 );
		wantOpen2 = wantOpen2 || Input.Pressed( InputButton.Slot2 );
		wantOpen2 = wantOpen2 || Input.Pressed( InputButton.Slot3 );
		wantOpen2 = wantOpen2 || Input.Pressed( InputButton.Slot4 );
		wantOpen2 = wantOpen2 || Input.Pressed( InputButton.Slot5 );
		wantOpen2 = wantOpen2 || Input.Pressed( InputButton.Slot6 );
		wantOpen2 = wantOpen2 || Input.Pressed( InputButton.Slot7 );

		if ( Weapons.Count == 0 )
		{
			IsOpen = false;
			return;
		}

		// We're not open, but we want to be
		if ( IsOpen != wantOpen )
		{
			SelectedWeapon = localPlayer?.ActiveChild as Weapon;
			if ( wantOpen2 ) SelectedWeapon = null;
			IsOpen = true;
		}

		// Not open fuck it off
		if ( !IsOpen ) return;

		//
		// Fire pressed when we're open - select the weapon and close.
		//
		if ( Input.Down( InputButton.PrimaryAttack ) || Input.VR.RightHand.JoystickPress )
		{
			Input.SuppressButton( InputButton.PrimaryAttack ); 
			localPlayer.ActiveChildInput = SelectedWeapon;
			IsOpen = false;
			CurrentSound.Stop();
			CurrentSound = Sound.FromScreen( "wpn_select" );
			return;
		}
		var sortedWeapons = Weapons.OrderBy( x => x.Order ).ToList();

		// get our current index
		var oldSelected = SelectedWeapon;
		int SelectedIndex = sortedWeapons.IndexOf( SelectedWeapon );
		SelectedIndex = SlotPressInput( SelectedIndex, sortedWeapons );

		// forward if mouse wheel was pressed
		SelectedIndex -= Input.MouseWheel;

		if ( Input.Pressed( InputButton.SlotNext ) || (Input.VR.RightHand.Joystick.Value.y < -0.5 && SinceSelectedWeapon > 3) )
		{
			SinceSelectedWeapon = 0;
			SelectedIndex++;
		}
		if ( Input.Pressed( InputButton.SlotPrev ) || (Input.VR.RightHand.Joystick.Value.y > 0.5 && SinceSelectedWeapon > 3) )
		{
			SinceSelectedWeapon = 0;
			SelectedIndex--;
		}
		if ( Input.VR.RightHand.Joystick.Value.y.AlmostEqual( 0 ) )
		{
			SinceSelectedWeapon = 3;
		}
		SelectedIndex = SelectedIndex.UnsignedMod( Weapons.Count );

		SelectedWeapon = sortedWeapons[SelectedIndex];

		for ( int i = 0; i < invslots; i++ )
		{
			columns[i].TickSelection( SelectedWeapon );
		}

		Input.MouseWheel = 0;

	}

	int SlotPressInput(int SelectedIndex, List<Weapon> sortedWeapons )
	{
		var columninput = -1;

		if ( Input.Pressed( InputButton.Slot1 ) ) columninput = 0;
		if ( Input.Pressed( InputButton.Slot2 ) ) columninput = 1;
		if ( Input.Pressed( InputButton.Slot3 ) ) columninput = 2;
		if ( Input.Pressed( InputButton.Slot4 ) ) columninput = 3;
		if ( Input.Pressed( InputButton.Slot5 ) ) columninput = 4;
		if ( Input.Pressed( InputButton.Slot6 ) ) columninput = 5;
		if ( Input.Pressed( InputButton.Slot7 ) ) columninput = 6;

		if ( columninput == -1 ) return SelectedIndex;

		if ( SelectedWeapon.IsValid() && SelectedWeapon.Bucket == columninput )
		{
			return NextInBucket( sortedWeapons );
		}

		CurrentSound.Stop();
		CurrentSound = Sound.FromScreen( "wpn_hudon" );
		// Are we already selecting a weapon with this column?
		var firstOfColumn = sortedWeapons.Where( x => x.Bucket == columninput ).FirstOrDefault();
		if ( firstOfColumn == null )
		{
			// DOOP sound
			return SelectedIndex;
		}

		return sortedWeapons.IndexOf( firstOfColumn );
	}

	int NextInBucket( List<Weapon> sortedWeapons )
	{
		Assert.NotNull( SelectedWeapon );

		CurrentSound.Stop();
		CurrentSound = Sound.FromScreen( "wpn_moveselect" );

		Weapon first = null;
		Weapon prev = null;
		foreach ( var weapon in sortedWeapons.Where( x => x.Bucket == SelectedWeapon.Bucket ) )
		{
			if ( first == null ) first = weapon;
			if ( prev == SelectedWeapon ) return sortedWeapons.IndexOf( weapon );
			prev = weapon;
		}


		return sortedWeapons.IndexOf( first );
	}
}
