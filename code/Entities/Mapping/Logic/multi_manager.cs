/// <summary>
/// A logic entity that allows to do a multitude of logic operations with Map I/O.<br/>
/// <br/>
/// TODO: This is a stop-gap solution and may be removed in the future in favor of "map blueprints" or node based Map I/O.
/// </summary>
[Library( "multi_manager" )]
[HammerEntity]
[VisGroup( VisGroup.Logic )]
[EditorSprite( "editor/multi_manager.vmat" )]
[Title( "Logic Entity" ), Category( "Logic" ), Icon( "calculate" )] 
public partial class LogicEntity : Entity
	{

	/// <summary>
	/// The (initial) enabled state of the logic entity.
	/// </summary>
	[Property]
		public bool Enabled { get; set; } = true; 

		/// <summary>
		/// Enables the entity.
		/// </summary>
		[Input]
		public void Enable()
		{
			Enabled = true;
		}

		/// <summary>
		/// Disables the entity, so that it would not fire any outputs.
		/// </summary>
		[Input]
		public void Disable()
		{
			Enabled = false;
		}

			/// <summary>
		/// Enables the entity.
		/// </summary>
		[Input]
		public void TurnOn()
		{
			Enabled = true;
		}

		/// <summary>
		/// Disables the entity, so that it would not fire any outputs.
		/// </summary>
		[Input]
		public void TurnOff()
		{
			Enabled = false;
		}

		/// <summary>
		/// Toggles the enabled state of the entity.
		/// </summary>
		[Input]
		public void Toggle()
		{
			Enabled = !Enabled;
		}

		[Input]
		public void Kill()
		{
			if (Game.IsServer)
				Delete();
		}

	/*
	 * logic_auto
	 *
	 */

	/// <summary>
	///
	/// </summary>
	protected Output OnMapSpawn { get; set; }

		/// <summary>
		/// Fired after all map entities have spawned, even if it is disabled.
		/// </summary>
		[Event.Entity.PostSpawn]
		public void OnMapSpawnEvent()
		{
			OnMapSpawn.Fire( this );
		}
		[Event.Entity.PostCleanup]
        public void OnMapCleanupEvent()
        {
            //Log.Info("Deactivating logic auto by " + activator);
            OnMapSpawn.Fire( this );
        }
		/*
		 * logic_relay
		 *
		 */

		/// <summary>
		/// Fired when the this entity receives the "Trigger" input.
		/// </summary>
		protected Output OnTrigger { get; set; }

		/// <summary>
		/// Trigger the "OnTrigger" output.
		/// </summary>
		[Input]
		public void Trigger()
		{
			if ( !Enabled ) return;

			OnTrigger.Fire( this );
			//Log.Error( "[HLS2] multi_manager in use! there might be some legacy keyvalue logic, please make sure you move all of these to hammer logic!" );
		}

		/*
		 * logic_compare
		 *
		 */

		/// <summary>
		/// The (initial) value for Variable A
		/// </summary>
		[Property]
		public float VariableA { get; set; } = 0;

		/// <summary>
		/// The (initial) value for Variable B
		/// </summary>
		[Property]
		public float VariableB { get; set; } = 0;

		/// <summary>
		/// Fired when the value given to "CompareInput" or Variable A ("Compare" input) matches our Variable B.
		/// </summary>
		protected Output OnEqual { get; set; }

		/// <summary>
		/// Fired when the value given to "CompareInput" or Variable A ("Compare" input) is NOT equal our Variable B.
		/// </summary>
		protected Output OnNotEqual { get; set; }

		/// <summary>
		/// Fired when the value given to "CompareInput" or Variable A ("Compare" input) is less than our Variable B.
		/// </summary>
		protected Output OnLessThan { get; set; }

		/// <summary>
		/// Fired when the value given to "CompareInput" or Variable A ("Compare" input) is greater than our Variable B.
		/// </summary>
		protected Output OnGreaterThan { get; set; }

		/// <summary>
		/// Compares the given number to Variable B and fires the appropriate output.
		/// </summary>
		[Input]
		public void CompareInput( float input )
		{
			if ( !Enabled ) return;

			if ( input.Equals( VariableB ) )
			{
				OnEqual.Fire( this );
			}
			else
			{
				OnNotEqual.Fire( this );
			}

			if ( input > VariableB )
			{
				OnGreaterThan.Fire( this );
			}
			else
			{
				OnLessThan.Fire( this );
			}
		}

		/// <summary>
		/// Compares Variable A to Variable B and fires the appropriate output.
		/// </summary>
		[Input]
		public void Compare()
		{
			if ( !Enabled ) return;

			if ( VariableA.Equals( VariableB ) )
			{
				OnEqual.Fire( this );
			}
			else
			{
				OnNotEqual.Fire( this );
			}

			if ( VariableA > VariableB )
			{
				OnGreaterThan.Fire( this );
			}
			else
			{
				OnLessThan.Fire( this );
			}
		}

		/// <summary>
		/// Sets the Variable A and fires approprivate outputs.
		/// </summary>
		[Input]
		public void SetVariableA( float input )
		{
			if ( !Enabled ) return;

			VariableA = input;
			Compare();
		}

		/// <summary>
		/// Sets the Variable B and fires approprivate outputs.
		/// </summary>
		[Input]
		public void SetVariableB( float input )
		{
			if ( !Enabled ) return;

			VariableB = input;
			Compare();
		}
	}
