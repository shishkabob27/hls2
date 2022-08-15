using SandboxEditor;
	/// <summary>
	/// A logic entity that allows to do a multitude of logic operations with Map I/O.<br/>
	/// <br/>
	/// TODO: This is a stop-gap solution and may be removed in the future in favor of "map blueprints" or node based Map I/O.
	/// </summary>
	[Library( "logic_auto" )]
	[HammerEntity]
	[VisGroup( VisGroup.Logic )]
	[EditorSprite( "editor/ent_logic.vmat" )]
	[Title( "Logic Auto" ), Category( "Legacy" ), Icon( "calculate" )]
	public partial class LogicAuto : Entity
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
		/// Toggles the enabled state of the entity.
		/// </summary>
		[Input]
		public void Toggle()
		{
			Enabled = !Enabled;
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
		public void OnMapSpawnEvent(Entity activator = null)
		{
		//Log.Info("Activating logic auto by " + activator);
		//OnMapSpawn.Fire( this );
		}

		
	}