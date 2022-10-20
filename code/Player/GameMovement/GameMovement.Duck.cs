
partial class HL1GameMovement
{
	[ConVar.Replicated] public static bool sv_debug_duck { get; set; }

	/// <summary>
	/// Is the player currently fully ducked? This is what defines whether we apply duck slow down or not.
	/// </summary>
	public virtual bool IsDucked => Player.Tags.Has( PlayerTags.Ducked );
	public virtual bool IsDucking => DuckAmount > 0;
	public virtual float TimeToDuck => .2f;
	public virtual float DuckProgress => DuckAmount / TimeToDuck;

	public float DuckAmount { get; set; }

	public virtual bool WishDuck()
	{
		if (Client.IsUsingVr)
		{
			return Input.VR.Head.Position.z - Position.z <= HLGame.hl_vr_crouch_height;
		}
		return Input.Down( InputButton.Duck ) || Input.VR.RightHand.ButtonB;
	}

	public virtual void SimulateDucking()
	{
		if ( WishDuck() )
		{
			OnDucking();
		}
		else
		{
			OnUnducking();
		}

		if ( IsDucked )
			SetTag( PlayerTags.Ducked );

		HandleDuckingSpeedCrop();
	}

	public virtual void OnDucking()
	{
		if ( !CanDuck() )
			return;

		if ( !IsDucked && DuckAmount >= TimeToDuck || IsInAir )
		{
			FinishDuck();
		}

		if ( DuckAmount < TimeToDuck )
		{
			DuckAmount += Time.Delta;

			if ( DuckAmount > TimeToDuck )
				DuckAmount = TimeToDuck;
		}
	}

	public virtual void OnUnducking()
	{
		if ( !CanUnduck() )
			return;

		if ( IsDucked && DuckAmount == 0 || IsInAir )
		{
			FinishUnDuck();
		}

		if ( DuckAmount > 0 )
		{
			DuckAmount -= Time.Delta;

			if ( DuckAmount < 0 )
				DuckAmount = 0;
		}
	}

	public virtual void FinishDuck()
	{
		if ( Pawn.Tags.Has( PlayerTags.Ducked ) )
			return;

		Pawn.Tags.Add( PlayerTags.Ducked );
		DuckAmount = TimeToDuck;

		if ( IsGrounded )
		{
			Position -= GetPlayerMins( true ) - GetPlayerMins( false );
		}
		else
		{
			var hullSizeNormal = GetPlayerMaxs( false ) - GetPlayerMins( false );
			var hullSizeCrouch = GetPlayerMaxs( true ) - GetPlayerMins( true );
			var viewDelta = hullSizeNormal - hullSizeCrouch;
			Position += viewDelta;
		}

		// See if we are stuck?
		FixPlayerCrouchStuck( true );
		CategorizePosition();
	}

	public virtual void FinishUnDuck()
	{
		if ( !Pawn.Tags.Has( PlayerTags.Ducked ) )
			return;

		Player.Tags.Remove( PlayerTags.Ducked );
		DuckAmount = 0;

		if ( IsGrounded )
		{
			Position += GetPlayerMins( true ) - GetPlayerMins( false );
		}
		else
		{
			var hullSizeNormal = GetPlayerMaxs( false ) - GetPlayerMins( false );
			var hullSizeCrouch = GetPlayerMaxs( true ) - GetPlayerMins( true );
			var viewDelta = hullSizeNormal - hullSizeCrouch;
			Position -= viewDelta;
		}

		// Recategorize position since ducking can change origin
		CategorizePosition();
	}

	public virtual bool CanDuck()
	{
		return true;
	}

	public virtual bool CanUnduck()
	{
		var origin = Position;

		if ( IsGrounded )
		{
			origin += GetPlayerMins( true ) - GetPlayerMins( false );
		}
		else
		{
			var normalHull = GetPlayerMaxs( false ) - GetPlayerMins( false );
			var duckedHull = GetPlayerMaxs( true ) - GetPlayerMins( true );
			var viewDelta = normalHull - duckedHull;
			origin -= viewDelta;
		}

		bool wasDucked = Player.Tags.Has( PlayerTags.Ducked );

		if ( wasDucked ) Player.Tags.Remove( PlayerTags.Ducked );
		var trace = TraceBBox( Position, origin );
		if ( wasDucked ) Player.Tags.Add( PlayerTags.Ducked );

		if ( trace.StartedSolid || trace.Fraction != 1 )
			return false;

		return true;
	}

	public virtual void FixPlayerCrouchStuck( bool upward )
	{
		int direction = upward ? 1 : 0;

		var trace = TraceBBox( Position, Position );
		if ( trace.Entity == null )
			return;

		var test = Position;
		for ( int i = 0; i < 36; i++ )
		{
			var org = Position;
			org.z += direction;

			Position = org;
			trace = TraceBBox( Position, Position );
			if ( trace.Entity == null )
				return;
		}

		Position = test;
	}

	public virtual void HandleDuckingSpeedCrop()
	{
		if ( IsDucked && IsGrounded )
		{
			ForwardMove *= sv_duckkeyspeed;
			RightMove *= sv_duckkeyspeed;
			UpMove *= sv_duckkeyspeed;
		}
	}
}
