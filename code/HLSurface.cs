namespace Sandbox
{
	/// <summary>
	/// Extensions for Surfaces
	/// </summary>
	public static partial class HLSurface
	{
		/// <summary>
		/// Create a particle effect and play an impact sound for this surface being hit by a bullet
		/// </summary>
		public static Particles DoHLBulletImpact( this Surface self, TraceResult tr, bool Particle = true)
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
			
			var surf = tr.Surface;


			var newName = tr.Surface.ResourceName;
			switch (surf.ResourceName)
			{
				case "flesh":
					newName = "surface/hlflesh.surface";
					break;
				default:
					newName = "surface/hldefault.surface";
					break;
			}
			if (ResourceLibrary.TryGet<Surface>(newName, out var surfNew))
			{
				surf = surfNew;
			}
            
            
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

            
			if (Particle == false)
				return default;

			if (surf == null)
			{
                surf = tr.Surface;
            }
			string particleName = Rand.FromArray(surf.ImpactEffects.Bullet);
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
		public static void DoHLFootstep( this Surface self, Entity ent, TraceResult tr, int foot, float volume )
		{
			var newName = tr.Surface.ResourceName;
			switch (self.ResourceName)
			{
				case "flesh":
					newName = "surface/hlflesh.surface";
					break;
				default:
					newName = "surface/hldefault.surface";
					break;
			}
			if (ResourceLibrary.TryGet<Surface>(newName, out var surfNew))
			{
				self = surfNew;
			}

			var sound = foot == 0 ? self.Sounds.FootLeft : self.Sounds.FootRight;

			if ( !string.IsNullOrWhiteSpace( sound ) )
			{
				Sound.FromWorld( sound, tr.EndPosition ).SetVolume( volume );
			}
			else if ( self.GetBaseSurface() != null )
			{
				// Give base surface a chance
				self.GetBaseSurface().DoFootstep( ent, tr, foot, volume );
			}
		}
	}

}
