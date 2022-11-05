﻿using System;

namespace Sandbox
{
	public class HLPlayerAnimator : PawnAnimator
	{
		TimeSince TimeSinceFootShuffle = 60;


		float duck;

		public override void Simulate()
		{
			var player = Pawn as Player;


			var dir = Velocity;
			var forward = Rotation.Forward.Dot(dir);
			var sideward = Rotation.Right.Dot(dir);
			var angle = MathF.Atan2(sideward, forward).RadianToDegree().NormalizeDegrees();
			var idealRotation = Rotation.LookAt(Input.Rotation.Forward.WithZ(0), Vector3.Up);//Rotation.LookAt( Input.Rotation.Forward.WithZ(Input.Rotation.Forward.z.Clamp(-0.01f, 0.01f)), Vector3.Up );
																					  //(Pawn.EyeRotation.Pitch().Clamp(-10, 10))
		
			
			DoRotation( idealRotation );
			DoWalk();

			//
			// Let the animation graph know some shit
			//
			bool sitting = HasTag( "sitting" );
			bool noclip = HasTag( "noclip" ) && !sitting;

			SetAnimParameter( "b_grounded", GroundEntity != null || noclip || sitting );
			SetAnimParameter( "b_noclip", noclip );
			SetAnimParameter( "b_sit", sitting );
			SetAnimParameter( "b_swim", Pawn.WaterLevel > 0.5f && !sitting );
			

			if ( Host.IsClient && Client.IsValid() )
			{
				SetAnimParameter( "voice", Client.TimeSinceLastVoice < 0.5f ? Client.VoiceLevel : 0.0f );
			}

			Vector3 aimPos = Pawn.EyePosition + Input.Rotation.Forward * 200;
			Vector3 lookPos = aimPos;
			SetAnimParameter("aim_eyes_pitch", Pawn.EyeRotation.Pitch());
			SetAnimParameter("aim_eyes_yaw", Pawn.EyeRotation.Yaw());
			SetAnimParameter("aim_eyes_roll", Pawn.EyeRotation.Roll());
			//
			// Look in the direction what the player's input is facing
			//
			SetLookAt( "aim_eyes", lookPos );
			SetLookAt( "aim_head", lookPos );
			SetLookAt( "aim_body", aimPos );

			if ( HasTag( "ducked" ) ) duck = duck.LerpTo( 1.0f, Time.Delta * 10.0f );
			else duck = duck.LerpTo( 0.0f, Time.Delta * 5.0f );

			SetAnimParameter( "duck", duck );

			if ( player != null && player.ActiveChild is BaseCarriable carry )
			{
				carry.SimulateAnimator( this ); 
				
			}
			else
			{
				SetAnimParameter( "holdtype", 0 );
				SetAnimParameter( "aim_body_weight", 0.5f );
			}

		}

		public virtual void DoRotation( Rotation idealRotation )
		{
			var player = Pawn as Player;

			//
			// Our ideal player model rotation is the way we're facing
			//
			var allowYawDiff = player?.ActiveChild == null ? 90 : 50;

			float turnSpeed = 0.01f;
			if ( HasTag( "ducked" ) ) turnSpeed = 0.1f;

			//
			// If we're moving, rotate to our ideal rotation
			//
			Rotation = Rotation.Slerp( Rotation, idealRotation, 10 * turnSpeed );

			//
			// Clamp the foot rotation to within 120 degrees of the ideal rotation
			//
			Rotation = Rotation.Clamp( idealRotation, allowYawDiff, out var change );

			//
			// If we did restrict, and are standing still, add a foot shuffle
			//
			if ( change > 1 && WishVelocity.Length <= 1 ) TimeSinceFootShuffle = 0;

			SetAnimParameter( "b_shuffle", TimeSinceFootShuffle < 0.1 );
		}

		void DoWalk()
		{
			// Move Speed
			{
				var dir = Velocity;
				var forward = Rotation.Forward.Dot( dir );
				var sideward = Rotation.Right.Dot( dir );

				var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

				SetAnimParameter( "move_direction", angle );
				SetAnimParameter( "movedir", new Vector3(1, ((sideward / 36)) * -1, 1).EulerAngles.Direction );
				SetAnimParameter( "move_speed", Velocity.Length );
				SetAnimParameter( "move_groundspeed", Velocity.WithZ( 0 ).Length );
				SetAnimParameter( "move_y", sideward );
				SetAnimParameter( "move_x", forward );
				SetAnimParameter( "move_z", Velocity.z );
			}

			// Wish Speed
			{
				var dir = WishVelocity;
				var forward = Rotation.Forward.Dot( dir );
				var sideward = Rotation.Right.Dot( dir );

				var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

				SetAnimParameter( "wish_direction", angle );
				SetAnimParameter( "wish_speed", WishVelocity.Length );
				SetAnimParameter( "wish_groundspeed", WishVelocity.WithZ( 0 ).Length );
				SetAnimParameter( "wish_y", sideward );
				SetAnimParameter( "wish_x", forward );
				SetAnimParameter( "wish_z", WishVelocity.z );
			}
		}

		public override void OnEvent(string name)
		{
			// DebugOverlay.Text( Pos + Vector3.Up * 100, name, 5.0f );
			if ( name == "jump" )
			{
				Trigger( "b_jump" );
			}

            var player = Pawn as HLPlayer;
			player.OnJump(Position);
            base.OnEvent( name );
		}
	}
}
