/// <summary>
/// A generic button that is useful to control other map entities via inputs/outputs.
/// </summary>
[Library( "func_button" )]
[HammerEntity, SupportsSolid]
[DoorHelper( "movedir", "movedir_islocal", "movedir_type", "distance" )]
[RenderFields, VisGroup( VisGroup.Dynamic )]
[Model( Archetypes = ModelArchetype.animated_model | ModelArchetype.static_prop_model )]
[Title( "Button" ), Category( "Brush Entities" ), Icon( "radio_button_checked" )]
public partial class ButtonEntity : KeyframeEntity, IUse
{
	// TODO: Make sure only 1 player can be pressing a button
	// TOOO: Do not reset the button if it is being continuously pressed in?

	// TODO: SetPosition input?
	// TODO: start position keyvalue?

	// TODO: Model animations?
	// TODO: moving sound?

	/// <summary>
	/// Specifies the direction to move in when the button is used, or axis of rotation for rotating buttons.
	/// </summary>
	[Property( Title = "Move Direction" )]
	public Angles MoveDir { get; set; } = new Angles( 0, 0, 0 );

	/// <summary>
	/// If checked, the movement direction angle is in local space and should be rotated by the entity's angles after spawning.
	/// </summary>
	[Property( "movedir_islocal", Title = "Move Direction is Expressed in Local Space" )]
	public bool MoveDirIsLocal { get; set; } = true;

	public enum ButtonMoveType
	{
		Moving,
		Rotating,
		NotMoving
	}

	/// <summary>
	/// Movement type of the button.
	/// </summary>
	[Property( "movedir_type", Title = "Movement Type" )]
	public ButtonMoveType MoveDirType { get; set; } = ButtonMoveType.NotMoving;

	/// <summary>
	/// Moving button: The amount, in inches, of the button to leave sticking out of the wall it recedes into when pressed. Negative values make the button recede even further into the wall.
	/// Rotating button: The amount, in degrees, that the button should rotate when it's pressed.
	/// </summary>
	[Property]
	public float Distance { get; set; } = 1.0f;

	/// <summary>
	/// The speed at which the button moves, in inches per second or degress per second.
	/// </summary>
	[Property]
	public float Speed { get; set; } = 100.0f;

	/// <summary>
	/// The speed at which the button returns to the initial position. 0 or less will function as the 'forward' move speed.
	/// </summary>
	[Property]
	public float ReturnSpeed { get; set; } = 0;

	/// <summary>
	/// Amount of time, in seconds, after the button has been fully pressed before it starts to return to the starting position. Once it has returned, it can be used again. If the value is set to -1, the button never returns.
	/// </summary>
	[Property( "reset_delay", Title = "Reset Delay (-1 stay)" )]
	public float ResetDelay { get; set; } = 1.0f;

	/// <summary>
	/// Sound played when the button is pressed and is unlocked
	/// </summary>
	[Property( "unlocked_sound", Title = "Activation Sound" ), FGDType( "sound" )]
	public string UnlockedSound { get; set; }

	/// <summary>
	/// Sound played when the button is pressed and is locked
	/// </summary>
	[Property( "locked_sound", Title = "Locked Activation Sound" ), FGDType( "sound" )]
	public string LockedSound { get; set; }

	/// <summary>
	/// If enabled, the button will have to be activated again to return to initial position.
	/// </summary>
	[Property]
	public bool Toggle { get; set; } = false;

	/// <summary>
	/// If enabled, the button will have to be held to reach the pressed in state.
	/// </summary>
	[Property]
	public bool Momentary { get; set; } = false;

	/// <summary>
	/// Whether the button is locked or not.
	/// </summary>
	public bool Locked { get; set; }

	[Flags]
	public enum ActivationFlags
	{
		UseActivates = 1,
		DamageActivates = 2
	}

	/// <summary>
	/// How this button can be activated
	/// </summary>
	[Property]
	public ActivationFlags ActivationSettings { get; set; } = ActivationFlags.UseActivates;

	[Flags]
	public enum Flags
	{
		StartsLocked = 1,
		NonSolid = 2,
	}

	/// <summary>
	/// Settings that are only relevant on spawn
	/// </summary>

	[Property( "spawnflags", Title = "Spawn Settings" )]
	public Flags SpawnSettings { get; set; }

	/// <summary>
	/// Fired when the button position changes, carries 0...1 as argument.
	/// </summary>
	protected Output<float> OnProgress { get; set; }

	float _progress = 0.0f;
	public float Progress { protected set { _progress = value; OnProgress.Fire( this, _progress ); } get { return _progress; } }

	TimeSince LastUsed;
	bool State;
	bool Moving;

	Vector3 PositionStart;
	Vector3 PositionEnd;
	Rotation RotationStart;

	public override void Spawn()
	{
		base.Spawn();

		if ( SetupPhysicsFromModel( PhysicsMotionType.Keyframed ) == null )
		{
			Log.Warning( $"{this} has a model {Model} with no physics!" );
		}

		if ( SpawnSettings.HasFlag( Flags.NonSolid ) )
		{
			EnableSolidCollisions = false;
		}

		Locked = SpawnSettings.HasFlag( Flags.StartsLocked );

		// ButtonMoveType.Moving
		{
			PositionStart = LocalPosition;

			// Get the direction we want to move in
			var dir = MoveDir.Forward;

			// Open position is the size of the bbox in the direction minus the lip size
			var boundSize = CollisionBounds.Size;

			PositionEnd = PositionStart + dir * (MathF.Abs( boundSize.Dot( dir ) ) - Distance);

			if ( MoveDirIsLocal )
			{
				var dir_world = Transform.NormalToWorld( dir );
				PositionEnd = PositionStart + dir_world * (MathF.Abs( boundSize.Dot( dir ) ) - Distance);
			}
		}

		// ButtonMoveType.Rotating
		RotationStart = LocalRotation;
	}

	public virtual bool IsUsable( Entity user ) => !Moving && ActivationSettings.HasFlag( ActivationFlags.UseActivates );

	int globalTimers = 0;
	async Task FireRelease()
	{
		var thisTimer = ++globalTimers;

		await Task.DelaySeconds( 0.1f );

		if ( thisTimer != globalTimers ) return;

		_ = OnReleased.Fire( this );
	}

	/// <summary>
	/// Fired when the button is used while locked
	/// </summary>
	protected Output OnUseLocked { get; set; }

	/// <summary>
	/// A player has pressed this
	/// </summary>
	public virtual bool OnUse( Entity user )
	{
		if ( !ActivationSettings.HasFlag( ActivationFlags.UseActivates ) ) return false;

		if ( Locked )
		{
			OnUseLocked.Fire( user );
			PlaySound( LockedSound );
			return false;
		}

		if ( LastUsed > 0.1f ) DoPress( user );

		LastUsed = 0;
		_ = FireRelease();

		return Momentary;
	}

	/// <summary>
	/// Fired when the button is damaged
	/// </summary>
	protected Output OnDamaged { get; set; }

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		OnDamaged.Fire( info.Attacker );

		if ( ActivationSettings.HasFlag( ActivationFlags.DamageActivates ) )
		{
			DoPress( info.Attacker );
			OnReleased.Fire( this );
		}
	}

	[Input]
	public void Kill()
	{
		if (Game.IsServer) Delete();
	}

	/// <summary>
	/// Fired when the button is pressed by any means. This is not the same as button reaching its pressed in position.
	/// </summary>
	protected Output OnPressed { get; set; }

	/// <summary>
	/// Fired when the button was released. This is not the same as button reaching its initial position.
	/// </summary>
	protected Output OnReleased { get; set; }

	void DoPress( Entity activator )
	{
		if ( Moving ) return;

		if ( Locked )
		{
			PlaySound( LockedSound );
			return;
		}

		var targetState = !State;
		if ( Momentary && !Toggle )
		{
			targetState = true;
		}

		PlaySound( UnlockedSound );
		OnPressed.Fire( activator );

		_ = MoveToPosition( targetState, activator );
	}

	/// <summary>
	/// Fired when the button reaches the in/pressed position
	/// </summary>
	protected Output OnIn { get; set; }

	/// <summary>
	/// Fired when the button reaches the out/released position
	/// </summary>
	protected Output OnOut { get; set; }

	float lastThinkTime = 0;
	[GameEvent.Tick.Server]
	void Think()
	{
		// Decay for momentary type
		if ( Momentary && !Toggle && LastUsed > ResetDelay && !Moving && Progress > 0 && !Locked )
		{
			float progDir = -1;
			if ( MoveDirType == ButtonMoveType.Moving )
			{
				var targetPos = PositionStart;
				var distance = Vector3.DistanceBetween( LocalPosition, targetPos );
				var timeToTake = Math.Abs( distance / Math.Max( ReturnSpeed > 0 ? ReturnSpeed : Speed, 0.1f ) );
				Progress = Math.Clamp( Progress + (Time.Now - lastThinkTime) / timeToTake * progDir, 0.0f, 1.0f );

				LocalPosition = PositionStart.LerpTo( PositionEnd, Progress );

			}
			else if ( MoveDirType == ButtonMoveType.Rotating )
			{
				var timeToTake = Math.Abs( Distance / Math.Max( ReturnSpeed > 0 ? ReturnSpeed : Speed, 0.1f ) );
				Progress = Math.Clamp( Progress + (Time.Now - lastThinkTime) / timeToTake * progDir, 0.0f, 1.0f );

				var axis = Rotation.From( MoveDir ).Up;
				if ( !MoveDirIsLocal ) axis = Transform.NormalToLocal( axis );

				LocalRotation = RotationStart.RotateAroundAxis( axis, Distance * Progress );
			}

			if ( Progress == 0 )
			{
				OnOut.Fire( this );
			}
		}
		lastThinkTime = Time.Now;

		if ( DebugFlags.HasFlag( EntityDebugFlags.Text ) )
		{
			DebugOverlay.Text( $"State: {State}\nProgress: {Progress}", WorldSpaceBounds.Center, 10, Color.White );

			var dir_world = MoveDir.Forward;
			if ( MoveDirIsLocal ) dir_world = Transform.NormalToWorld( MoveDir.Forward );

			DebugOverlay.Line( Position, Position + dir_world * 100 );
		}
	}

	async Task MoveToPosition( bool state, Entity activator )
	{
		State = state;
		Moving = true;
		LastUsed = 0;

		float progDir = state ? 1 : -1;
		if ( MoveDirType == ButtonMoveType.Moving )
		{
			LocalPosition = PositionStart.LerpTo( PositionEnd, Progress );

			var targetPos = state ? PositionEnd : PositionStart;
			var distance = Vector3.DistanceBetween( LocalPosition, targetPos );
			var timeToTake = Math.Abs( distance / Math.Max( Speed, 0.1f ) );
			if ( !state && ReturnSpeed > 0 && !Momentary ) timeToTake = Math.Abs( distance / Math.Max( ReturnSpeed, 0.1f ) );

			var startTime = Time.Now;
			while ( (state && Progress < 1.0f) || (!state && Progress > 0.0f) )
			{
				await Task.NextPhysicsFrame();
				if ( !this.IsValid() ) return;

				if ( Momentary && (LastUsed > 0.1f || Locked) )
				{
					// We were let go
					Moving = false;
					return;
				}

				Progress = Math.Clamp( Progress + (Time.Now - startTime) / timeToTake * progDir, 0.0f, 1.0f );
				startTime = Time.Now;

				LocalPosition = PositionStart.LerpTo( PositionEnd, Progress );
			}
		}
		else if ( MoveDirType == ButtonMoveType.Rotating )
		{
			var axis = Rotation.From( MoveDir ).Up;
			if ( !MoveDirIsLocal ) axis = Transform.NormalToLocal( axis );

			LocalRotation = RotationStart.RotateAroundAxis( axis, Distance * Progress );

			var timeToTake = Math.Abs( Distance / Math.Max( Speed, 0.1f ) );
			if ( !state && ReturnSpeed > 0 && !Momentary ) timeToTake = Math.Abs( Distance / Math.Max( ReturnSpeed, 0.1f ) );

			var startTime = Time.Now;
			while ( (state && Progress < 1.0f) || (!state && Progress > 0.0f) )
			{
				await Task.NextPhysicsFrame();
				if ( !this.IsValid() ) return;

				if ( Momentary && (LastUsed > 0.1f || Locked) )
				{
					// We were let go
					Moving = false;
					return;
				}

				Progress = Math.Clamp( Progress + (Time.Now - startTime) / timeToTake * progDir, 0.0f, 1.0f );
				startTime = Time.Now;

				LocalRotation = RotationStart.RotateAroundAxis( axis, Distance * Progress );
			}
		}
		else if ( MoveDirType != ButtonMoveType.NotMoving )
		{
			Log.Warning( $"{this}: Unknown button move type {MoveDirType}!" );
		}

		if ( state ) _ = OnIn.Fire( activator );
		else _ = OnOut.Fire( activator );

		if ( state == false )
		{
			Moving = false;
			return;
		}

		if ( ResetDelay < 0 )
		{
			// Continue to be moving so we can't be de pressed
			//Moving = false;
			return;
		}

		if ( !Toggle ) await Task.DelaySeconds( ResetDelay );

		Moving = false;

		if ( Toggle || Momentary ) return;

		await MoveToPosition( false, activator );
	}

	/// <summary>
	/// Become locked
	/// </summary>
	[Input]
	protected void Lock()
	{
		Locked = true;
	}

	/// <summary>
	/// Become unlocked
	/// </summary>
	[Input]
	protected void Unlock()
	{
		Locked = false;
	}

	/// <summary>
	/// Simulates the button being pressed
	/// </summary>
	[Input]
	protected void Press( Entity activator )
	{
		DoPress( activator );
		OnReleased.Fire( this );
	}
}
