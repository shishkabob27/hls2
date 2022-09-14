using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Sandbox
{
	/// <summary>
	/// A model break command, defined in ModelDoc and ran after spawning model gibs. The inheriting class must have a LibraryAttribute.
	/// </summary>
	///
	public interface IModelBreakCommand
	{
		/// <summary>
		/// This will be called after an entity with this model breaks via <see cref="Breakables">Breakables</see> class.
		/// </summary>
		/// <param name="result">This class contains the break event data, including the source entity and the list of gibs.</param>
		public abstract void OnBreak( Sandbox.Breakables.Result result );
	}

	/// <summary>
	/// Handle breaking a prop into bits
	/// </summary>
	public static partial class Breakables
	{

		private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
		{
			ReadCommentHandling = JsonCommentHandling.Skip,
			PropertyNameCaseInsensitive = true,
			IncludeFields = true,
			Converters =
			{
				new JsonStringEnumConverter()
			}
		};

		public static void ApplyBreakCommands( Result result )
		{
			ModelEntity ent = result.Source as ModelEntity;
			if ( ent == null || ent.Model == null ) return;

			var data = ent.Model.GetBreakCommands();
			foreach ( var kv in data )
			{
				var type = TypeLibrary.GetDescription( kv.Key )?.TargetType;
				if ( type == null ) continue;

				// For each break command of this type..
				foreach ( var cmdData in kv.Value )
				{
					try
					{
						object instance = JsonSerializer.Deserialize( cmdData, type, jsonOptions );
						if ( instance is not IModelBreakCommand bcInst ) continue;

						bcInst.OnBreak( result );
					}
					catch ( Exception e )
					{
						Log.Error( e );
					}
				}
			}
		}
	}
}


namespace Sandbox.Internal
{
	/// <summary>
	/// Spawn a particle system when this model breaks.
	/// </summary>
	[Library( "break_create_particle" )]
	class ModelBreakParticle : IModelBreakCommand
	{
		// Only execute this command if given named break piece has spawned..
		/*[JsonPropertyName( "part_name" ), FGDType( "model_breakpiece" )]
		public string LimitToPart { get; set; }*/

		/// <summary>
		/// The particle to spawn when the model breaks.
		/// </summary>
		[JsonPropertyName( "name" ), ResourceType( "vpcf" )]
		public string Particle { get; set; }

		/// <summary>
		/// (Optional) Set the particle control point #0 to the specified model.
		/// </summary>
		[JsonPropertyName( "cp0_model" ), ResourceType( "vmdl" )]
		public string Model { get; set; }

		/// <summary>
		/// (Optional) Set the particle control point #0 to the specified snapshot.
		/// </summary>
		[JsonPropertyName( "cp0_snapshot" ), ResourceType( "vsnap" )]
		public string Snapshot { get; set; }

		/// <summary>
		/// (Optional) Set this control point to the position of the break damage.
		/// </summary>
		[JsonPropertyName( "damage_position_cp" ), DefaultValue( -1 )]
		public int? DamagePositionCP { get; set; }

		/// <summary>
		/// (Optional) Set this control point to the direction of the break damage.
		/// </summary>
		[JsonPropertyName( "damage_direction_cp" ), DefaultValue( -1 )]
		public int? DamageDirectionCP { get; set; }

		/// <summary>
		/// (Optional) Set this control point to the velocity of the original prop.
		/// </summary>
		[JsonPropertyName( "velocity_cp" ), DefaultValue( -1 )]
		public int? VelocityCP { get; set; }

		/// <summary>
		/// (Optional) Set this control point to the angular velocity of the original prop.
		/// </summary>
		[JsonPropertyName( "angular_velocity_cp" ), DefaultValue( -1 )]
		public int? AngularVelocityCP { get; set; }

		/// <summary>
		/// (Optional) Set this control point to global world gravity at the moment the model broke.
		/// </summary>
		[JsonPropertyName( "local_gravity_cp" ), DefaultValue( -1 )]
		public int? LocalGravityCP { get; set; }

		/// <summary>
		/// (Optional) Set this control point to the tint color of the original prop as a vector with values 0 to 1.
		/// </summary>
		[JsonPropertyName( "tint_cp" ), DefaultValue( -1 )]
		public int? TintCP { get; set; }

		// I think this is a Source 1 thing
		/*/// <summary>
		/// (Optional) Set this control point to the parent model's active materialgroup name.
		/// </summary>
		[JsonPropertyName( "skin_cp" ), DefaultValue( -1 )]
		public int? SkinCP { get; set; }*/

		public void OnBreak( Sandbox.Breakables.Result res )
		{
			if ( res.Source is not ModelEntity ent ) return;

			var part = Particles.Create( Particle, ent.Position );
			part.SetOrientation( 0, ent.Rotation );
			if ( !string.IsNullOrEmpty( Model ) ) part.SetModel( 0, Sandbox.Model.Load( Model ) );
			if ( !string.IsNullOrEmpty( Snapshot ) ) part.SetSnapshot( 0, Snapshot );

			if ( DamagePositionCP.HasValue )
			{
				part.SetPosition( DamagePositionCP.Value, res.Params.DamagePositon );
			}
			if ( DamageDirectionCP.HasValue )
			{
				part.SetForward( DamageDirectionCP.Value, (res.Params.DamagePositon - ent.Position).Normal );
			}
			if ( AngularVelocityCP.HasValue )
			{
				part.SetOrientation( AngularVelocityCP.Value, ent.AngularVelocity );
			}
			if ( LocalGravityCP.HasValue )
			{
				part.SetPosition( LocalGravityCP.Value, Map.Physics.Gravity );
				part.SetForward( LocalGravityCP.Value, Map.Physics.Gravity.Normal );
			}
			if ( VelocityCP.HasValue )
			{
				part.SetPosition( VelocityCP.Value, ent.Velocity );
				part.SetForward( VelocityCP.Value, ent.Velocity.Normal );
			}
			if ( TintCP.HasValue )
			{
				var clr = ent.RenderColor.ToColor32();
				part.SetPosition( TintCP.Value, new Vector3( clr.r, clr.g, clr.b ) );
			}
		}
	}

	/// <summary>
	/// Creates a revolute (hinge) joint between two spawned breakpieces.
	/// </summary>
	[Library( "break_create_joint_revolute" )]
	[ModelDoc.Axis( Origin = "anchor_position", Angles = "anchor_angles" )]
	[ModelDoc.HingeJoint( Origin = "anchor_position", Angles = "anchor_angles", EnableLimit = "enable_limit", MinAngle = "min_angle", MaxAngle = "max_angle" )]
	class ModelBreakPieceRevoluteJoint : IModelBreakCommand
	{
		[JsonPropertyName( "parent_piece" ), FGDType( "model_breakpiece" )]
		public string ParentPiece { get; set; }

		[JsonPropertyName( "child_piece" ), FGDType( "model_breakpiece" )]
		public string ChildPiece { get; set; }

		[JsonPropertyName( "anchor_position" ), Title( "Anchor Position (relative to model)" )]
		public Vector3 Position { get; set; }

		/// <summary>
		/// Axis around which the revolute/hinge joint rotates.
		/// </summary>
		[JsonPropertyName( "anchor_angles" ), Title( "Anchor Axis (relative to model)" )]
		public Angles Angles { get; set; }

		/// <summary>
		/// Hinge friction.
		/// </summary>
		public float Friction { get; set; }

		/// <summary>
		/// Whether the angle limit should be enabled or not.
		/// </summary>
		[JsonPropertyName( "enable_limit" )]
		public bool LimitAngles { get; set; }

		[JsonPropertyName( "min_angle" ), MinMax( -179, 179 )]
		public float MinimumAngle { get; set; }

		[JsonPropertyName( "max_angle" ), MinMax( -179, 179 )]
		public float MaximumAngle { get; set; }

		public void OnBreak( Sandbox.Breakables.Result res )
		{
			var ParentEnt = res.Props.Find( prop => prop is PropGib gib && gib.BreakpieceName == ParentPiece );
			var ChildEnt = res.Props.Find( prop => prop is PropGib gib && gib.BreakpieceName == ChildPiece );
			if ( ParentEnt == null || ChildPiece == null ) return;

			var WorldPos = res.Source.Transform.PointToWorld( Position );
			var WorldAngle = res.Source.Transform.RotationToWorld( Rotation.From( Angles ) );

			var hinge = PhysicsJoint.CreateHinge( ParentEnt.PhysicsBody, ChildEnt.PhysicsBody, WorldPos, WorldAngle.Up );
			hinge.EnableAngularConstraint = true;
			hinge.EnableLinearConstraint = true;
			hinge.Friction = Friction;

			if ( LimitAngles )
			{
				hinge.MinAngle = MinimumAngle;
				hinge.MaxAngle = MaximumAngle;
			}
		}
	}

	/// <summary>
	/// Overrides the materialgroup on spawned breakpieces. (By default the active material group of the parent model is propagated to the breakpieces.)
	/// </summary>
	[Library( "break_change_material_group" )]
	class ModelBreakPieceMaterialGroup : IModelBreakCommand
	{
		/// <summary>
		/// Material group name to switch to.
		/// </summary>
		[JsonPropertyName( "material_group_name" ), FGDType( "materialgroup" )]
		public string MaterialGroup { get; set; }

		/// <summary>
		/// If set, only apply this command to a particular piece.
		/// </summary>
		[JsonPropertyName( "limit_to_piece" ), FGDType( "model_breakpiece" )]
		public string LimitToPiece { get; set; }

		/*/// <summary>
		/// If set, only apply this command if the parent model had a particular materialgroup active.
		/// </summary>
		[JsonPropertyName( "limit_to_material_group" ), FGDType( "materialgroup" ), Title( "Limit to instances using material group" )]
		public string LimitToMatGroup { get; set; }*/

		public void OnBreak( Sandbox.Breakables.Result res )
		{
			// TODO: Cannot get current entity's material group
			/*if ( !string.IsNullOrEmpty( LimitToMatGroup ) )
			{
				if ( res.Source is ModelEntity entity && entity.IsMaterialGroup( LimitToMatGroup ) ) return;
			}*/

			foreach ( var gib in res.Props )
			{
				if ( !string.IsNullOrEmpty( LimitToPiece ) && gib is PropGib propGib && propGib.BreakpieceName != LimitToPiece ) continue;

				gib.SetMaterialGroup( MaterialGroup );
			}
		}
	}

	/*/// <summary>
	/// Disables impact (physics) damage on spawned breakpiece(s)
	/// </summary>
	[Library( "break_disable_impact_damage" )]
	class ModelBreakPieceNoImpactDamage : IModelBreakCommand
	{
		/// <summary>
		/// If set, only apply this command to a particular piece.
		/// </summary>
		[JsonPropertyName( "limit_to_piece" ), FGDType( "model_breakpiece" )]
		public string LimitToPiece { get; set; }

		public void OnBreak( Sandbox.Breakables.Result res )
		{
			foreach ( var gib in res.Props )
			{
				if ( !string.IsNullOrEmpty( LimitToPiece ) && gib is PropGib propGib && propGib.BreakpieceName != LimitToPiece ) continue;
			}
		}
	}*/

	/// <summary>
	/// Applies extra velocity to breakpieces outwards from the influence point (default is the origin of the model)
	/// </summary>
	[Library( "break_apply_force" )]
	[ModelDoc.Axis( Origin = "offset", Attachment = "attachment_point" )]
	class ModelBreakPieceApplyForce : IModelBreakCommand
	{
		/// <summary>
		/// Offset for the influence point (in the space of the model or attachment)
		/// </summary>
		public Vector3 Offset { get; set; }

		/// <summary>
		/// Offset the influence point from the named attachment rather than the root of the model
		/// </summary>
		[JsonPropertyName( "attachment_point" ), FGDType( "model_attachment" )]
		public string AttachmentPoint { get; set; }

		/// <summary>
		/// Center the influence point (will ignore cause attachment to be ignored, but still honor offset)
		/// </summary>
		[JsonPropertyName( "center_on_damage_point" )]
		public bool CenterOnDamagePoint { get; set; } = false;

		/// <summary>
		/// If set, only apply this command to a particular piece.
		/// </summary>
		[JsonPropertyName( "limit_to_piece" ), FGDType( "model_breakpiece" )]
		public string LimitToPiece { get; set; } = "";

		/// <summary>
		/// Velocity added to each piece (radially, away from the influence point)
		/// </summary>
		[JsonPropertyName( "burst_scale" ), MinMax( -500, 500 )]
		public float BurstScale { get; set; } = 0;

		/// <summary>
		/// Magnitude of random vector that will be added to the burst velocity
		/// </summary>
		[JsonPropertyName( "burst_randomize" ), MinMax( 0, 500 )]
		public float BurstRandomize { get; set; } = 0;

		public enum BreakForceType
		{
			RadialPush,
			AngularFlip,
			AngularTwist
		}

		/// <summary>
		/// What kind of force to apply?<br/>
		/// <b>Radial Push</b> - Applies a radial burst to breakpieces outwards from the influence point<br/>
		/// <b>Angular Flip</b> - Applies an angular 'flip' to breakpieces (like objects tipping over from an explosion or flower petals opening) causing them to tip outwards from the influence point<br/>
		/// <b>Angular Twist</b> - Applies an angular 'twist' to breakpieces, causing them to roll around the radial axis outward from the influence point<br/>
		/// </summary>
		public BreakForceType ForceType { get; set; } = BreakForceType.RadialPush;

		protected Vector3 ComputeBreakForcePoint( Sandbox.Breakables.Result res )
		{
			if ( res.Source == null ) return Vector3.Zero;

			if ( CenterOnDamagePoint )
			{
				return res.Params.DamagePositon + res.Source.Transform.Rotation * Offset;
			}

			if ( res.Source is ModelEntity mdlEnt && !string.IsNullOrEmpty( AttachmentPoint ) )
			{
				var mTemp = mdlEnt.GetAttachment( AttachmentPoint );
				if ( mTemp.HasValue )
				{
					return mTemp.Value.TransformVector( Offset );
				}
			}

			return res.Source.Transform.TransformVector( Offset );
		}

		public void OnBreak( Sandbox.Breakables.Result res )
		{
			var offset = ComputeBreakForcePoint( res );
			if ( ForceType == BreakForceType.AngularFlip || ForceType == BreakForceType.AngularTwist )
			{
				foreach ( var gib in res.Props )
				{
					if ( !string.IsNullOrEmpty( LimitToPiece ) && gib is PropGib propGib && propGib.BreakpieceName != LimitToPiece ) continue;

					var vRelativePos = gib.PhysicsBody.MassCenter - offset;

					if ( BurstRandomize > 1e-6f )
					{
						vRelativePos += Vector3.Random * BurstRandomize;
					}

					vRelativePos = vRelativePos.Normal;//NormalizeInPlaceSafe( Vector(1,0,0) );

					Vector3 vFlipAxis = vRelativePos;
					if ( ForceType == BreakForceType.AngularFlip )
					{
						// use -z so that positive flip force = 'falling over' away from the force origin
						vFlipAxis = vRelativePos.Cross( Vector3.Down );
					}

					float flFinalScale = BurstScale;
					if ( BurstRandomize > 1e-6f )
					{
						flFinalScale += Rand.Float( -BurstRandomize, BurstRandomize );
					}

					const float ANGULAR_VELOCITY_SCALE = 0.1f; // this is an arbitrary number so that linear and angular numbers are in rougly the same scale (10 isn't much, 500 is too much)

					gib.PhysicsBody.AngularVelocity += vFlipAxis * flFinalScale * ANGULAR_VELOCITY_SCALE;
				}
			}
			else // BreakForceType.RadialPush and invalid options
			{
				foreach ( var gib in res.Props )
				{
					if ( !string.IsNullOrEmpty( LimitToPiece ) && gib is PropGib propGib && propGib.BreakpieceName != LimitToPiece ) continue;

					var vecBurst = gib.PhysicsBody.MassCenter - offset;

					float flBurstLen = vecBurst.Length;
					if ( flBurstLen > 1e-6f )
					{
						vecBurst *= Math.Abs( BurstScale ) / flBurstLen;
					}
					if ( BurstRandomize > 1e-6f )
					{
						vecBurst += Vector3.Random * BurstRandomize;
					}

					gib.PhysicsBody.Velocity += vecBurst;
				}
			}
		}
	}
}
