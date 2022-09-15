namespace Sandbox
{
	/// <summary>
	/// Extensions for Surfaces
	/// </summary>
	public static partial class HLSurface
    {
        public static Surface ReplaceSurface(this Surface self)
        {
            var surf = self;
            var newName = self.ResourceName;
            switch (surf.ResourceName)
            {
                case "flesh":
                    newName = "surface/hl_flesh.surface";
                    break;
                case "tile":
                    newName = "surface/hl_tile.surface";
                    break;
                case "metal":
                    newName = "surface/hl_metal.surface";
                    break;
                case "grate":
                    newName = "surface/hl_grate.surface";
                    break;
                case "concrete":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "glass":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "glass.pane":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "glass.shard":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "dirt":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "carpet":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "mud":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "plaster":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "plastic":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "plastic.sheet":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "plastic.sheet.watercontainer":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "rubber":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "sand":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "slippy_wheels":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "water":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "watermelon":
                    newName = "surface/hl_concrete.surface";
                    break;
                case "wood":
                    newName = "surface/hl_wood.surface";
                    break;
                case "default":
                    newName = "surface/hl_concrete.surface";
                    break;
                default:
                    newName = "surface/hl_concrete.surface";
                    break;
            }
            if (ResourceLibrary.TryGet<Surface>(newName, out var surfNew))
            {
                surf = surfNew;
            }
            return surf;
        }
        /// <summary>
        /// Create a particle effect and play an impact sound for this surface being hit by a bullet
        /// </summary>
        public static Particles DoHLBulletImpact(this Surface self, TraceResult tr, bool Particle = true)
		{
			//
			// No effects on resimulate
			//
			if (!Prediction.FirstTime)
				return null;



			//
			// Drop a decal
			//
			var decalPath = "decals/bullet_hole.decal";

			var surf = ReplaceSurface(self);


            while (string.IsNullOrWhiteSpace(decalPath) && surf != null)
			{
				decalPath = Rand.FromArray(surf.ImpactEffects.BulletDecal);
				surf = surf.GetBaseSurface();
			}

			if (!string.IsNullOrWhiteSpace(decalPath))
			{
				if (ResourceLibrary.TryGet<DecalDefinition>(decalPath, out var decal))
				{
					Decal.Place(decal, tr);
				}
			}

			//
			// Make an impact sound
			//
			var sound = self.Sounds.Bullet;

			//surf = self.GetBaseSurface();
			while (string.IsNullOrWhiteSpace(sound) && surf != null)
			{
				sound = surf.Sounds.Bullet;
				surf = surf.GetBaseSurface();
			}

			if (!string.IsNullOrWhiteSpace(sound))
			{
				Sound.FromWorld(sound, tr.EndPosition);
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
			if (string.IsNullOrWhiteSpace(particleName)) particleName = Rand.FromArray(self.ImpactEffects.Regular);


			while (string.IsNullOrWhiteSpace(particleName) && surf != null)
			{
				particleName = Rand.FromArray(surf.ImpactEffects.Bullet);
				if (string.IsNullOrWhiteSpace(particleName)) particleName = Rand.FromArray(surf.ImpactEffects.Regular);

				surf = surf.GetBaseSurface();
			}

			if (!string.IsNullOrWhiteSpace(particleName))
			{
				var ps = Particles.Create(particleName, tr.EndPosition);
				ps.SetForward(0, tr.Normal);

				return ps;
			}

			return default;
		}

		/// <summary>
		/// Create a footstep effect
		/// </summary>
		public static void DoHLFootstep(this Surface self, Entity ent, TraceResult tr, int foot, float volume)
		{

            self = ReplaceSurface(self);

            var sound = foot == 0 ? self.Sounds.FootLeft : self.Sounds.FootRight;

			if (!string.IsNullOrWhiteSpace(sound))
			{
				Sound.FromWorld(sound, tr.EndPosition).SetVolume(volume);
			}
			else if (self.GetBaseSurface() != null)
			{
				// Give base surface a chance
				self.GetBaseSurface().DoFootstep(ent, tr, foot, volume);
			}
		}
		/// <summary>
		/// Create a jump effect
		/// </summary>
		public static void DoHLJump(this Surface self, Entity ent, TraceResult tr, float volume)
		{
            self = ReplaceSurface(self);

            var sound = self.Sounds.FootLaunch;

			if (!string.IsNullOrWhiteSpace(sound))
			{
				Sound.FromWorld(sound, tr.EndPosition).SetVolume(volume);
			}
			else if (self.GetBaseSurface() != null)
			{
				// Give base surface a chance
				self.GetBaseSurface().DoFootstep(ent, tr, 1, volume);
			}
		}
		//
		// Summary:
		//     Returns a random gib taking into account base surface.
		public static string GetRandomGib(this Surface self)
		{
			var surf = ReplaceSurface(self);
			string text = Rand.FromArray(surf.Breakables.GenericGibs);
			while (string.IsNullOrWhiteSpace(text) && self.GetBaseSurface() != null)
			{
				text = Rand.FromArray(self.GetBaseSurface().Breakables.GenericGibs);
			}

			return text;
        }
        public static void GetBounceSound(this Surface self, Vector3 pos, float volume = 1)
        {
            self = ReplaceSurface(self);

            var sound = self.Sounds.ImpactHard;

            if (!string.IsNullOrWhiteSpace(sound))
            {
                Sound.FromWorld(sound, pos).SetVolume(volume);
            }
        }
        public static void GetBustSound(this Surface self, Vector3 pos, float volume = 1)
        {
            self = ReplaceSurface(self);

            var sound = self.Breakables.BreakSound;

            if (!string.IsNullOrWhiteSpace(sound))
            {
                Sound.FromWorld(sound, pos).SetVolume(volume);
            }
        }
    }
}
