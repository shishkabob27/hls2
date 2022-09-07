using System;

namespace Sandbox
{
	/// <summary>
	/// A struct to help you set up your citizen based animations
	/// </summary>
	public struct HLAnimationHelper
	{
		AnimatedEntity Owner;

		public HLAnimationHelper( AnimatedEntity entity )
		{
			Owner = entity;
		}

		/// <summary>
		/// Have the player look at this point in the world
		/// </summary>
		public void WithLookAt( Vector3 look, float eyesWeight = 1.0f, float headWeight = 1.0f, float bodyWeight = 1.0f )
		{
			Owner.SetAnimLookAt( "aim_eyes", look );
			Owner.SetAnimLookAt( "aim_head", look );
			Owner.SetAnimLookAt( "aim_body", look );

			Owner.SetAnimParameter( "aim_eyes_weight", eyesWeight );
			Owner.SetAnimParameter( "aim_head_weight", headWeight );
			Owner.SetAnimParameter( "aim_body_weight", bodyWeight );
		}

		public void WithVelocity( Vector3 Velocity )
		{
			var dir = Velocity;
			var forward = Owner.Rotation.Forward.Dot( dir );
			var sideward = Owner.Rotation.Right.Dot( dir );

			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

			Owner.SetAnimParameter( "move_direction", angle );
			Owner.SetAnimParameter( "move_speed", Velocity.Length );
			Owner.SetAnimParameter( "move_groundspeed", Velocity.WithZ( 0 ).Length );
			Owner.SetAnimParameter( "move_y", sideward );
			Owner.SetAnimParameter( "move_x", forward );
			Owner.SetAnimParameter( "move_z", Velocity.z );
		}

		public void WithWishVelocity( Vector3 Velocity )
		{
			var dir = Velocity;
			var forward = Owner.Rotation.Forward.Dot( dir );
			var sideward = Owner.Rotation.Right.Dot( dir );

			var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

			Owner.SetAnimParameter( "wish_direction", angle );
			Owner.SetAnimParameter( "wish_speed", Velocity.Length );
			Owner.SetAnimParameter( "wish_groundspeed", Velocity.WithZ( 0 ).Length );
			Owner.SetAnimParameter( "wish_y", sideward );
			Owner.SetAnimParameter( "wish_x", forward );
			Owner.SetAnimParameter( "wish_z", Velocity.z );
		}

		public Rotation AimAngle
		{
			set
			{
				value = Owner.Rotation.Inverse * value;
				var ang = value.Angles();

				Owner.SetAnimParameter( "aim_body_pitch", ang.pitch );
				Owner.SetAnimParameter( "aim_body_yaw", ang.yaw );
			}
		}
        public float Neck
        {
            get => Owner.GetAnimParameterFloat("neck");
            set => Owner.SetAnimParameter("neck", value);
        }

        public float AimEyesWeight
		{
			get => Owner.GetAnimParameterFloat( "aim_eyes_weight" );
			set => Owner.SetAnimParameter( "aim_eyes_weight", value );
		}

		public float AimHeadWeight
		{
			get => Owner.GetAnimParameterFloat( "aim_head_weight" );
			set => Owner.SetAnimParameter( "aim_head_weight", value );
		}

		public float AimBodyWeight
		{
			get => Owner.GetAnimParameterFloat( "aim_body_weight" );
			set => Owner.SetAnimParameter( "aim_headaim_body_weight_weight", value );
		}


		public float FootShuffle
		{
			get => Owner.GetAnimParameterFloat( "move_shuffle" );
			set => Owner.SetAnimParameter( "move_shuffle", value );
		}

		public float DuckLevel
		{
			get => Owner.GetAnimParameterFloat( "duck" );
			set => Owner.SetAnimParameter( "duck", value );
		}

		public float VoiceLevel
		{
			get => Owner.GetAnimParameterFloat( "voice" );
			set => Owner.SetAnimParameter( "voice", value );
		}

		public float HealthLevel
		{
			get => Owner.GetAnimParameterFloat("health");
			set => Owner.SetAnimParameter("health", value);
		}

        public bool Attack
        {
            get => Owner.GetAnimParameterBool("attack");
            set => Owner.SetAnimParameter("attack", value);
        }

        public bool IsScared
		{
			get => Owner.GetAnimParameterBool("scared");
			set => Owner.SetAnimParameter("scared", value);
		}

		public bool IsSitting
		{
			get => Owner.GetAnimParameterBool( "b_sit" );
			set => Owner.SetAnimParameter( "b_sit", value );
		}

		public bool IsGrounded
		{
			get => Owner.GetAnimParameterBool( "b_grounded" );
			set => Owner.SetAnimParameter( "b_grounded", value );
		}

		public bool IsSwimming
		{
			get => Owner.GetAnimParameterBool( "b_swim" );
			set => Owner.SetAnimParameter( "b_swim", value );
		}

		public bool IsClimbing
		{
			get => Owner.GetAnimParameterBool( "b_climbing" );
			set => Owner.SetAnimParameter( "b_climbing", value );
		}

		public bool IsNoclipping
		{
			get => Owner.GetAnimParameterBool( "b_noclip" );
			set => Owner.SetAnimParameter( "b_noclip", value );
		}

		public bool IsWeaponLowered
		{
			get => Owner.GetAnimParameterBool( "b_weapon_lower" );
			set => Owner.SetAnimParameter( "b_weapon_lower", value );
		}

        public HLCombat.HoldTypes HoldType
		{
			get => (HLCombat.HoldTypes)Owner.GetAnimParameterInt( "holdtype" );
			set => Owner.SetAnimParameter( "holdtype", (int)value );
		}

		public enum Hand
		{
			Both,
			Right,
			Left
		}

		public Hand Handedness
		{
			get => (Hand)Owner.GetAnimParameterInt( "holdtype_handedness" );
			set => Owner.SetAnimParameter( "holdtype_handedness", (int)value );
		}

		public void TriggerJump()
		{
			Owner.SetAnimParameter( "b_jump", true );
		}

		public void TriggerDeploy()
		{
			Owner.SetAnimParameter( "b_deploy", true );
		}
	}
}
