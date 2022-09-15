using System;

namespace Sandbox
{
	/// <summary>
	/// Handle breaking a prop into bits
	/// </summary>
	public static partial class Breakables
	{
		// TODO - Debris collision group

		// TODO - Probably not ideal to use List here
		static internal List<Entity> CurrentGibs = new();

		[ConVar.Server( "gibs_max" )]
		static public int MaxGibs { get; set; } = 256;

		public static void Break( Entity ent, Result result = null )
        { 
            if ( ent is ModelEntity modelEnt && modelEnt.IsValid )
			{
				if ( result != null )
				{
					result.Source = ent;

					// Let's make sure we have SOME position.
					if ( result.Params.DamagePositon.IsNearlyZero() )
					{
						result.Params.DamagePositon = modelEnt.CollisionWorldSpaceCenter;
					}
				}
				Break( modelEnt.Model, modelEnt.Position, modelEnt.Rotation, modelEnt.Scale, modelEnt.RenderColor, result, modelEnt.PhysicsBody );
				if ( result != null ) ApplyBreakCommands( result );
			}
		}

		public static void Break( Model model, Vector3 pos, Rotation rot, float scale, Color color, Result result = null, PhysicsBody sourcePhysics = null )
		{
			if ( model == null || model.IsError )
				return;

			var genericGibsSpawned = false;
			var breakList = model.GetData<ModelBreakPiece[]>();
			string surfName = sourcePhysics?.GetDominantSurface();
			var surface = Surface.FindByName( surfName );
			if ( surface == null ) surface = Surface.FindByName( "default" );
			// If model has particles to spawn on break, do not do generic gibs
			var hasAnyBreakParticles = model.GetBreakCommands().ContainsKey( "break_create_particle" );

			// If model has no gibs of it own, try to replace them with something.
			// This is mostly intended for map models, not vmdls.
			if ( (breakList == null || breakList.Length <= 0) && !hasAnyBreakParticles )
			{
				genericGibsSpawned = true;

				List<ModelBreakPiece> pieces = new();



				if ( surface != null )
				{
                    HLSurface.GetBustSound(surface, pos);
                    /*var breakSnd = surface.Breakables.BreakSound;

					var surf = surface.GetBaseSurface();
					while ( string.IsNullOrWhiteSpace( breakSnd ) && surf != null )
					{
						breakSnd = surf.Breakables.BreakSound;
						surf = surf.GetBaseSurface();
					}

					if ( !string.IsNullOrEmpty( breakSnd ) )
					{
						Sound.FromWorld( breakSnd, pos );
					}*/
                }

				//int num = ( model.Bounds.Volume / 20000.0f ).CeilToInt();
				int num = (model.Bounds.Size.x * model.Bounds.Size.y + model.Bounds.Size.y * model.Bounds.Size.z + model.Bounds.Size.z * model.Bounds.Size.x).CeilToInt() / (432 * 1);

				//DebugOverlay.Box( 10, pos, rot, model.Bounds.Mins, model.Bounds.Maxs, Color.Green );
				for ( int i = 0; i < num; i++ )
				{
					ModelBreakPiece piece = new();
					piece.Model = HLSurface.GetRandomGib(surface);
					piece.Offset = model.Bounds.RandomPointInside;
					piece.CollisionTags = "debris";
					//piece.FadeTime = 3.0f;
					pieces.Add( piece );

					//DebugOverlay.Axis( pos + rot * piece.Offset, Rotation.Identity, 10, 10 );
				}

				breakList = pieces.ToArray();
			}

			if ( breakList == null || breakList.Length <= 0 ) return;

			// Remove all invalid gibs
			CurrentGibs.RemoveAll( x => !x.IsValid() );

			// Remove enough old gibs to fit the new ones...
			if ( MaxGibs > 0 && CurrentGibs.Count + breakList.Length >= MaxGibs )
			{
				int toRemove = (CurrentGibs.Count + breakList.Length) - MaxGibs;
				toRemove = Math.Min( toRemove, CurrentGibs.Count );
				for ( int i = 0; i < toRemove; i++ )
				{
					CurrentGibs[i].Delete();
					// Not fading out because that still causes too much lag with large amounts of gibs
					//_ = FadeAsync( CurrentGibs[ i ] as Prop, 0.1f );
				}
				CurrentGibs.RemoveRange( 0, toRemove );
			}
			
			foreach ( var piece in breakList )
			{
				if ( MaxGibs >= 0 && CurrentGibs.Count >= MaxGibs ) return;

				var mdl = Model.Load( piece.Model );
				var offset = mdl.GetAttachment( "placementOrigin" ) ?? Transform.Zero;

				var gib = new HLGib
				{
					Position = pos + rot * ((piece.Offset - offset.Position) * scale),
					//Rotation = rot,
					Scale = scale,
					RenderColor = color,
					Invulnerable = 0.2f,
					BreakpieceName = piece.PieceName,
					AlternateLandingRotation = true,
					BounceSound = true
                };

				gib.SurfaceType = surface;
				gib.Tags.Add( "gib" );
				gib.Model = mdl;
                gib.AngularVelocity = new Angles(Rand.Float(100, 300), 0, Rand.Float(100, 200));
				 
                gib.Velocity += new Vector3(Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f));
                gib.Velocity = gib.Velocity * Rand.Float(40f, 60f);

                if ( result != null && result.Source != null && result.Source is ModelEntity mdlEnt )
				{
					gib.CopyMaterialGroup( mdlEnt );
				}

				if ( !string.IsNullOrWhiteSpace( piece.CollisionTags ) )
				{
					var parts = piece.CollisionTags.Split( ' ' );
					foreach( var p in parts )
					{
						gib.Tags.Add( p );
					}
				}

				var phys = gib.PhysicsBody;
				if ( phys != null )
				{
					// Apply the velocity at the parent object's position
					if ( sourcePhysics != null )
					{
						phys.Velocity = sourcePhysics.GetVelocityAtPoint( phys.Position );
						phys.AngularVelocity = sourcePhysics.AngularVelocity;
					}
				}

				if ( piece.FadeTime > 0 )
				{
					_ = FadeAsync( gib, piece.FadeTime );
				}

				result?.AddProp( gib );

				if ( MaxGibs > 0 ) CurrentGibs.Add( gib );
			}

			// Give some randomness to generic gibs
			if ( genericGibsSpawned && result != null )
			{
				foreach ( var gib in result.Props )
				{
					gib.AngularVelocity = Angles.Random * 256;
					gib.Velocity = Vector3.Random * 100;
					gib.Rotation = Rotation.Random;
				}
			}
		}

		static async Task FadeAsync( ModelEntity gib, float fadeTime )
		{
			fadeTime += Rand.Float( -1, 1 );

			if ( fadeTime < 0.5f )
				fadeTime = 0.5f;

			await gib.Task.DelaySeconds( fadeTime );

			var fadePerFrame = 5 / 255.0f;

			while ( gib.RenderColor.a > 0 )
			{
				var c = gib.RenderColor;
				c.a -= fadePerFrame;
				gib.RenderColor = c;
				await gib.Task.Delay( 20 );
			}

			gib.Delete();
		}

		public class Result
		{
			public struct BreakableParams
			{
				public Vector3 DamagePositon;
				// TODO: Move more data here?
			}

			/// <summary>
			/// The entity that is breaking.
			/// </summary>
			public Entity Source;

			/// <summary>
			/// Various break piece related parameters
			/// </summary>
			public BreakableParams Params;

			/// <summary>
			/// List of gibs generated. Amount of gibs may not match model's break piece count depending on value of <see cref="Breakables.MaxGibs"/>.
			/// </summary>
			public List<ModelEntity> Props = new List<ModelEntity>();

			public void AddProp( ModelEntity prop )
			{
				Props.Add( prop );
			}

			public void CopyParamsFrom( DamageInfo dmgInfo )
			{
				Params.DamagePositon = dmgInfo.Position;
			}
		}
	}
}
