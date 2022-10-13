namespace Sandbox
{
	/// <summary>
	/// Extensions for Surfaces
	/// </summary>
	public static partial class HLSurface
	{

		[ConVar.Replicated] public static bool hl_debug_printsurface { get; set; } = false;
		[ConVar.Replicated] public static bool hl_debug_printmat { get; set; } = false;

		/// <summary>
		/// Replace any base surface with our HL1 equivelents, so we get our crispy footstep noises on any surface on any map.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="texturename"></param>
		/// <returns></returns>
		public static Surface ReplaceSurface( this Surface self, string texturename = "concrete" )
		{
			var surf = self;
			var newName = self.ResourceName;

			switch ( surf.ResourceName )
			{

				//-------------------------- BASE SURFACE --------------------------//
				case "flesh":
					newName = "surface/hl_flesh.surface";
					break;
				case "metal":
					newName = "surface/hl_metal.surface";
					break;
				case "metal.sheet":
					newName = "surface/hl_metal.surface";
					break;
				case "metal.weapon":
					newName = "surface/hl_metal.surface";
					break;
				case "grate":
					newName = "surface/hl_grate.surface";
					break;
				case "concrete":
					newName = "surface/hl_concrete.surface";
					break;
				case "glass":
					newName = "surface/hl_glass.surface";
					break;
				case "glass.pane":
					newName = "surface/hl_glass.surface";
					break;
				case "glass.shard":
					newName = "surface/hl_glass.surface";
					break;
				case "dirt":
					newName = "surface/hl_dirt.surface";
					break;
				case "carpet":
					newName = "surface/hl_concrete.surface";
					break;
				case "mud":
					newName = "surface/hl_slosh.surface";
					break;
				case "plaster":
					newName = "surface/hl_concrete.surface";
					break;
				case "plastic":
					newName = "surface/hl_tile.surface";
					break;
				case "plastic.sheet":
					newName = "surface/hl_tile.surface";
					break;
				case "plastic.sheet.watercontainer":
					newName = "surface/hl_tile.surface";
					break;
				case "rubber":
					newName = "surface/hl_tile.surface";
					break;
				case "sand":
					newName = "surface/hl_dirt.surface";
					break;
				case "slippy_wheels":
					newName = "surface/hl_concrete.surface";
					break;
				case "water":
					newName = "surface/hl_slosh.surface";
					break;
				case "watermelon":
					newName = "surface/hl_concrete.surface";
					break;
				case "wood":
					newName = "surface/hl_wood.surface";
					break;
				//-------------------------- NON VANILLA --------------------------//
				case "brick":
					newName = "surface/hl_tile.surface";
					break;
				case "flesh_yellow":
					newName = "surface/hl_flesh_yellow.surface";
					break;
				case "tile":
					newName = "surface/hl_tile.surface";
					break;
				//-------------------------- HL DEFAULTS --------------------------//
				case "hl_computer":
					newName = "surface/hl_computer.surface";
					break;
				case "hl_concrete":
					newName = "surface/hl_concrete.surface";
					break;
				case "hl_dirt":
					newName = "surface/hl_dirt.surface";
					break;
				case "hl_flesh":
					newName = "surface/hl_flesh.surface";
					break;
				case "hl_flesh_yellow":
					newName = "surface/hl_flesh_yellow.surface";
					break;
				case "hl_glass":
					newName = "surface/hl_glass.surface";
					break;
				case "hl_grate":
					newName = "surface/hl_grate.surface";
					break;
				case "hl_metal":
					newName = "surface/hl_metal.surface";
					break;
				case "hl_slosh":
					newName = "surface/hl_slosh.surface";
					break;
				case "hl_tile":
					newName = "surface/hl_tile.surface";
					break;
				case "hl_vent":
					newName = "surface/hl_vent.surface";
					break;
				case "hl_wood":
					newName = "surface/hl_wood.surface";
					break;
				case "default":
					newName = trysmarttexturesurfreplacement( texturename );
					break;
				default:
					newName = "surface/hl_concrete.surface";
					break;
			}
			if ( hl_debug_printsurface )
			{
				Log.Info( $"{surf.ResourceName} => {newName}" );
			}
			if ( hl_debug_printmat )
			{
				Log.Info( $"{texturename} => {newName}" );
			}
			if ( ResourceLibrary.TryGet<Surface>( newName, out var surfNew ) )
			{
				surf = surfNew;
			}
			return surf;
		}
		public static string trysmarttexturesurfreplacement( string texturename = "concrete" )
		{
			if ( texturename.Contains( "concrete" ) )
				return "surface/hl_concrete.surface";
			if ( texturename.Contains( "metal" ) )
				return "surface/hl_metal.surface";
			if ( texturename.Contains( "metal" ) )
				return "surface/hl_metal.surface";
			if ( texturename.Contains( "metal" ) )
				return "surface/hl_metal.surface";
			if ( texturename.Contains( "metal" ) )
				return "surface/hl_metal.surface";
			return "surface/hl_concrete.surface";
		}
		public static float GetFootstepVolumeOffset( this Surface self )
		{
			var surf = self;
			var voldoff = 0.0f;
			switch ( surf.ResourceName )
			{
				case "hl_concrete":
					voldoff = 0;
					break;
				case "hl_metal":
					voldoff = 0;
					break;
				case "hl_dirt":
					voldoff = 0.05f;
					break;
				case "hl_vent":
					voldoff = 0.2f;
					break;
				case "hl_grate":
					voldoff = 0;
					break;
				case "hl_tile":
					voldoff = 0;
					break;
				case "hl_slosh":
					voldoff = 0;
					break;
				case "default":
					voldoff = 0;
					break;
				default:
					voldoff = 0;
					break;
			}
			return voldoff;
		}

		/// <summary>
		/// Create a particle effect and play an impact sound for this surface being hit by a bullet
		/// </summary>
		public static Particles DoHLBulletImpact( this Surface self, TraceResult tr, bool Particle = true, string texturename = "concrete" )
		{
			//
			// No effects on resimulate
			//
			if ( !Prediction.FirstTime )
				return null;



			//
			// Drop a decal
			//
			var decalPath = "decals/bullet_hole.decal";

			var surf = ReplaceSurface( self, texturename );


			while ( string.IsNullOrWhiteSpace( decalPath ) && surf != null )
			{
				decalPath = Rand.FromArray( surf.ImpactEffects.BulletDecal );
				surf = surf.GetBaseSurface();
			}

			if ( !string.IsNullOrWhiteSpace( decalPath ) )
			{
				if ( ResourceLibrary.TryGet<DecalDefinition>( decalPath, out var decal ) )
				{
					Decal.Place( decal, tr );
				}
			}

			//
			// Make an impact sound
			//
			var sound = self.Sounds.Bullet;

			//surf = self.GetBaseSurface();
			while ( string.IsNullOrWhiteSpace( sound ) && surf != null )
			{
				sound = surf.Sounds.Bullet;
				surf = surf.GetBaseSurface();
			}

			if ( !string.IsNullOrWhiteSpace( sound ) )
			{
				Sound.FromWorld( sound, tr.EndPosition );
			}

			//
			// Get us a particle effect
			//

			//surf = tr.Surface;




			if ( surf == null )
			{
				surf = tr.Surface;
			}



			if ( Particle == false )
			{
				if ( surf.ResourceName == "hl_computer" )
				{
					new spark( tr.EndPosition );
				}
				return default;
			}

			string particleName = Rand.FromArray( surf.ImpactEffects.Bullet );
			if ( string.IsNullOrWhiteSpace( particleName ) ) particleName = Rand.FromArray( self.ImpactEffects.Regular );


			while ( string.IsNullOrWhiteSpace( particleName ) && surf != null )
			{
				particleName = Rand.FromArray( surf.ImpactEffects.Bullet );
				if ( string.IsNullOrWhiteSpace( particleName ) ) particleName = Rand.FromArray( surf.ImpactEffects.Regular );

				surf = surf.GetBaseSurface();
			}

			if ( !string.IsNullOrWhiteSpace( particleName ) )
			{
				var ps = Particles.Create( particleName, tr.EndPosition );
				ps.SetForward( 0, tr.Normal );

				return ps;
			}

			return default;
		}

		/// <summary>
		/// Create a footstep effect
		/// </summary>
		public static void DoHLFootstep( this Surface self, Entity ent, TraceResult tr, int foot, float volume, string texturename = "concrete" )
		{

			self = ReplaceSurface( self, texturename );

			var sound = foot == 0 ? self.Sounds.FootLeft : self.Sounds.FootRight;
			var offset = GetFootstepVolumeOffset( self );
			var vol = (volume + offset) * 3;
			if ( !string.IsNullOrWhiteSpace( sound ) )
			{
				Sound.FromWorld( sound, tr.EndPosition ).SetVolume( vol );
			}
			else if ( self.GetBaseSurface() != null )
			{
				// Give base surface a chance
				self.GetBaseSurface().DoFootstep( ent, tr, foot, vol );
			}
		}
		/// <summary>
		/// Create a jump effect
		/// </summary>
		public static void DoHLJump( this Surface self, Entity ent, TraceResult tr, float volume, string texturename = "concrete" )
		{
			self = ReplaceSurface( self, texturename );

			var sound = self.Sounds.FootLaunch;

			if ( !string.IsNullOrWhiteSpace( sound ) )
			{
				Sound.FromWorld( sound, tr.EndPosition ).SetVolume( volume );
			}
			else if ( self.GetBaseSurface() != null )
			{
				// Give base surface a chance
				self.GetBaseSurface().DoFootstep( ent, tr, 1, volume );
			}
		}

		/// <summary>
		/// Returns a random gib taking into account base surface.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="texturename"></param>
		/// <returns></returns>
		public static string GetRandomGib( this Surface self, string texturename = "concrete" )
		{
			var surf = ReplaceSurface( self, texturename );
			string text = Rand.FromArray( surf.Breakables.GenericGibs );
			while ( string.IsNullOrWhiteSpace( text ) && self.GetBaseSurface() != null )
			{
				text = Rand.FromArray( self.GetBaseSurface().Breakables.GenericGibs );
			}

			return text;
		}
		/// <summary>
		/// Get the sound used when a gib bounces.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="pos"></param>
		/// <param name="volume"></param>
		/// <param name="texturename"></param>
		public static void GetBounceSound( this Surface self, Vector3 pos, float volume = 1, string texturename = "concrete" )
		{
			self = ReplaceSurface( self, texturename );

			var sound = self.Sounds.ImpactHard;

			if ( !string.IsNullOrWhiteSpace( sound ) )
			{
				Sound.FromWorld( sound, pos ).SetVolume( volume );
			}
		}
		/// <summary>
		/// Get the sound used when a surface bursts into gibs.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="pos"></param>
		/// <param name="volume"></param>
		/// <param name="texturename"></param>
		public static void GetBustSound( this Surface self, Vector3 pos, float volume = 1, string texturename = "concrete" )
		{
			self = ReplaceSurface( self, texturename );

			var sound = self.Breakables.BreakSound;

			if ( !string.IsNullOrWhiteSpace( sound ) )
			{
				Sound.FromWorld( sound, pos ).SetVolume( volume );
			}
		}
	}
}
