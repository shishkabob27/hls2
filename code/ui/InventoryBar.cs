﻿using Sandbox.UI;

/// <summary>
/// The main inventory panel, top left of the screen.
/// </summary>
public class InventoryBar : Panel
{
	List<InventoryColumn> columns = new();
	List<HLWeapon> Weapons = new();

	public bool IsOpen;
	HLWeapon SelectedWeapon;

	public InventoryBar()
	{
		for ( int i = 0; i < 5; i++ )
		{
			var icon = new InventoryColumn( i, this );
			columns.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "active", IsOpen );

		var player = Local.Pawn as Player;
		if ( player == null ) return;

		Weapons.Clear();
		Weapons.AddRange( player.Children.Select( x => x as HLWeapon ).Where( x => x.IsValid() && x.IsUsable() ) );

		foreach ( var weapon in Weapons )
		{
			columns[weapon.Bucket].UpdateWeapon( weapon );
		}
	}

	/// <summary>
	/// IClientInput implementation, calls during the client input build.
	/// You can both read and write to input, to affect what happens down the line.
	/// </summary>
	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		if ( HLGame.CurrentState != HLGame.GameStates.Live ) return;

		bool wantOpen = IsOpen;
		var localPlayer = Local.Pawn as Player;

		// If we're not open, maybe this input has something that will 
		// make us want to start being open?
		wantOpen = wantOpen || input.MouseWheel != 0;
		wantOpen = wantOpen || input.Pressed( InputButton.Slot1 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot2 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot3 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot4 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot5 );
		//wantOpen = wantOpen || input.Pressed( InputButton.Slot6 );

		if ( Weapons.Count == 0 )
		{
			IsOpen = false;
			return;
		}

		// We're not open, but we want to be
		if ( IsOpen != wantOpen )
		{
			SelectedWeapon = localPlayer?.ActiveChild as HLWeapon;
			IsOpen = true;
		}

		// Not open fuck it off
		if ( !IsOpen ) return;

		//
		// Fire pressed when we're open - select the weapon and close.
		//
		if ( input.Down( InputButton.PrimaryAttack ) )
		{
			input.SuppressButton( InputButton.PrimaryAttack );
			input.ActiveChild = SelectedWeapon;
			IsOpen = false;
			Sound.FromScreen( "dm.ui_select" );
			return;
		}

		var sortedWeapons = Weapons.OrderBy( x => x.Order ).ToList();

		// get our current index
		var oldSelected = SelectedWeapon;
		int SelectedIndex = sortedWeapons.IndexOf( SelectedWeapon );
		SelectedIndex = SlotPressInput( input, SelectedIndex, sortedWeapons );

		// forward if mouse wheel was pressed
		SelectedIndex -= input.MouseWheel;
		SelectedIndex = SelectedIndex.UnsignedMod( Weapons.Count );

		SelectedWeapon = sortedWeapons[SelectedIndex];

		for ( int i = 0; i < 5; i++ )
		{
			columns[i].TickSelection( SelectedWeapon );
		}

		input.MouseWheel = 0;

		if ( oldSelected != SelectedWeapon )
		{
			Sound.FromScreen( "dm.ui_tap" );
		}
	}

	int SlotPressInput( InputBuilder input, int SelectedIndex, List<HLWeapon> sortedWeapons )
	{
		var columninput = -1;

		if ( input.Pressed( InputButton.Slot1 ) ) columninput = 0;
		if ( input.Pressed( InputButton.Slot2 ) ) columninput = 1;
		if ( input.Pressed( InputButton.Slot3 ) ) columninput = 2;
		if ( input.Pressed( InputButton.Slot4 ) ) columninput = 3;
		if ( input.Pressed( InputButton.Slot5 ) ) columninput = 4;
		//if ( input.Pressed( InputButton.Slot6 ) ) columninput = 5;

		if ( columninput == -1 ) return SelectedIndex;

		if ( SelectedWeapon.IsValid() && SelectedWeapon.Bucket == columninput )
		{
			return NextInBucket( sortedWeapons );
		}

		// Are we already selecting a weapon with this column?
		var firstOfColumn = sortedWeapons.Where( x => x.Bucket == columninput ).FirstOrDefault();
		if ( firstOfColumn == null )
		{
			// DOOP sound
			return SelectedIndex;
		}

		return sortedWeapons.IndexOf( firstOfColumn );
	}

	int NextInBucket( List<HLWeapon> sortedWeapons )
	{
		Assert.NotNull( SelectedWeapon );

		HLWeapon first = null;
		HLWeapon prev = null;
		foreach ( var weapon in sortedWeapons.Where( x => x.Bucket == SelectedWeapon.Bucket ) )
		{
			if ( first == null ) first = weapon;
			if ( prev == SelectedWeapon ) return sortedWeapons.IndexOf( weapon );
			prev = weapon;
		}

		return sortedWeapons.IndexOf( first );
	}
}
