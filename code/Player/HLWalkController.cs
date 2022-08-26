
using System;

namespace Sandbox
{
    [Library]
    public partial class HLWalkController : BasePlayerController
    {
        [ConVar.Replicated] public static float sv_gravity { get; set; } = 800;
        [ConVar.Replicated] public static float sv_stopspeed { get; set; } = 100;
        [ConVar.Replicated] public static float sv_noclip_accelerate { get; set; } = 5;
        [ConVar.Replicated] public static float sv_noclip_speed { get; set; } = 5;
        [ConVar.Replicated] public static float sv_spectator_accelerate { get; set; } = 5;
        [ConVar.Replicated] public static float sv_spectator_speed { get; set; } = 3;
        [ConVar.Replicated] public static bool sv_spectator_noclip { get; set; } = true;

        [ConVar.Replicated] public static float sv_maxspeed { get; set; } = 320;
        [ConVar.Replicated] public static float sv_accelerate { get; set; } = 10;

        [ConVar.Replicated] public static float sv_airaccelerate { get; set; } = 10;
        [ConVar.Replicated] public static float sv_aircontrol { get; set; } = 100;
        [ConVar.Replicated] public static float sv_airspeedcap { get; set; } = 30;
        [ConVar.Replicated] public static float sv_wateraccelerate { get; set; } = 10;
        [ConVar.Replicated] public static float sv_waterfriction { get; set; } = 1;
        [ConVar.Replicated] public static float sv_rollspeed { get; set; } = 200;
        [ConVar.Replicated] public static float sv_rollangle { get; set; } = 0;
        [ConVar.Replicated] public static float sv_maxnonjumpvelocity { get; set; } = 250;
        [ConVar.Replicated] public static float sv_maxstandableangle { get; set; } = 50;

        [ConVar.Replicated] public static float sv_friction { get; set; } = 4;

        [ConVar.Replicated] public static float sv_bounce { get; set; } = 0;
        [ConVar.Replicated] public static float sv_maxvelocity { get; set; } = 3500;
        [ConVar.Replicated] public static float sv_stepsize { get; set; } = 18;
        [ConVar.Replicated] public static float sv_backspeed { get; set; } = 0.6f;
        [ConVar.Replicated] public static float sv_waterdist { get; set; } = 12;


        [ConVar.Replicated] public static float sv_sprintspeed { get; set; } = 320.0f;
        [ConVar.Replicated] public static float sv_walkspeed { get; set; } = 150.0f;
        [ConVar.Replicated] public static float sv_defaultspeed { get; set; } = 320.0f;
        [ConVar.Replicated] public static bool sv_use_sbox_movehelper { get; set; } = true;
        [ConVar.Replicated] public static bool sv_enablebunnyhopping { get; set; } = true;
        [ConVar.Replicated, Net] public static bool sv_autojump { get; set; } = false;

        /*
        [Net] public float SprintSpeed { get; set; } = 320.0f;
        [Net] public float WalkSpeed { get; set; } = 150.0f;
        [Net] public float DefaultSpeed { get; set; } = 190.0f;
        [Net] public float Acceleration { get; set; } = 10.0f;
        [Net] public float AirAcceleration { get; set; } = 10.0f;
        [Net] public float FallSoundZ { get; set; } = -30.0f;
        [Net] public float GroundFriction { get; set; } = 4.0f;
        [Net] public float StopSpeed { get; set; } = 100.0f;
        [Net] public float Size { get; set; } = 20.0f;
        [Net] public float DistEpsilon { get; set; } = 0.03125f;
        [Net] public float GroundAngle { get; set; } = 46.0f;
        [Net] public float Bounce { get; set; } = 0.0f;
        [Net] public float MoveFriction { get; set; } = 1.0f;
        [Net] public float StepSize { get; set; } = 18.0f;
        [Net] public float MaxNonJumpVelocity { get; set; } = 250.0f;
        [Net] public float Gravity { get; set; } = 800.0f;
        [Net] public float AirControl { get; set; } = 100.0f;
         */
        [Net] public float BodyGirth { get; set; } = 32.0f;
        [Net] public float BodyHeight { get; set; } = 72.0f;
        [Net] public float EyeHeight { get; set; } = 64.0f;

        public Vector3 aVelocity;

        public Vector3 aPosition;
        
        public bool Swimming { get; set; } = false;

        public HLDuck Duck;
        public Unstuck Unstuck;


        public HLWalkController()
        {
            Duck = new HLDuck(this);
            Unstuck = new Unstuck(this);
        }

        /// <summary>
        /// This is temporary, get the hull size for the player's collision
        /// </summary>
        public override BBox GetHull()
        {
            var girth = BodyGirth * 0.5f;
            var mins = new Vector3(-girth, -girth, 0);
            var maxs = new Vector3(+girth, +girth, BodyHeight);

            return new BBox(mins, maxs);
        }


        // Duck body height 32
        // Eye Height 64
        // Duck Eye Height 28

        protected Vector3 mins;
        protected Vector3 maxs;

        public virtual void SetBBox(Vector3 mins, Vector3 maxs)
        {
            if (this.mins == mins && this.maxs == maxs)
                return;

            this.mins = mins;
            this.maxs = maxs;
        }

        /// <summary>
        /// Update the size of the bbox. We should really trigger some shit if this changes.
        /// </summary>
        public virtual void UpdateBBox()
        {
            var girth = BodyGirth * 0.5f;

            var mins = new Vector3(-girth, -girth, 0) * Pawn.Scale;
            var maxs = new Vector3(+girth, +girth, BodyHeight) * Pawn.Scale;

            Duck.UpdateBBox(ref mins, ref maxs, Pawn.Scale);

            SetBBox(mins, maxs);
        }

        protected float SurfaceFriction;


        public override void FrameSimulate()
        {
            base.FrameSimulate();

            EyeRotation = Input.Rotation;
        }
        public virtual void StartGravity()
        {
            float ent_gravity = -1;
            if (ent_gravity <= 0)
                ent_gravity = 1;
            var a = Velocity;
            a.z -= (ent_gravity * sv_gravity * 0.5f * Time.Delta);
            a.z += BaseVelocity.z * Time.Delta;
            Velocity = a;
            var temp = BaseVelocity;
            temp.z = 0;
            BaseVelocity = temp;

            CheckVelocity();
        }

        public virtual void FinishGravity()
        {
            if (Swimming)
                return;

            var ent_gravity = -1;
            if (ent_gravity <= 0)
                ent_gravity = 1;
            var a = Velocity;
            a[2] -= (ent_gravity * sv_gravity * Time.Delta * 0.5f);
            Velocity = a;
            CheckVelocity();
        }
        public override void Simulate()
        {
            fwishspd();
            EyeLocalPosition = Vector3.Up * (EyeHeight * Pawn.Scale);
            UpdateBBox();

            EyeLocalPosition += TraceOffset;
            EyeRotation = Input.Rotation;

            RestoreGroundPos();

            //Velocity += BaseVelocity * ( 1 + Time.Delta * 0.5f );
            //BaseVelocity = Vector3.Zero;

            //Rot = Rotation.LookAt( Input.Rotation.Forward.WithZ( 0 ), Vector3.Up );

            if (Unstuck.TestAndFix())
                return;

            // Check Stuck
            // Unstuck - or return if stuck

            // Set Ground Entity to null if  falling faster then 250

            // store water level to compare later

            // if not on ground, store fall velocity

            // player->UpdateStepSound( player->m_pSurfaceData, mv->GetAbsOrigin(), mv->m_vecVelocity )


            // RunLadderMode

            CheckLadder();
            Swimming = Pawn.WaterLevel > 0.6f;

            //
            // Start Gravity
            //
            StartGravity();


            /*
             if (player->m_flWaterJumpTime)
	            {
		            WaterJump();
		            TryPlayerMove();
		            // See if we are still in water?
		            CheckWater();
		            return;
	            }
            */

            // if ( underwater ) do underwater movement

            if (sv_autojump ? Input.Down(InputButton.Jump) : Input.Pressed(InputButton.Jump))
            {
                CheckJumpButton();
            }

            // Fricion is handled before we add in any base velocity. That way, if we are on a conveyor,
            //  we don't slow when standing still, relative to the conveyor.
            bool bStartOnGround = GroundEntity != null;
            //bool bDropSound = false;
            if (bStartOnGround)
            {
                //if ( Velocity.z < FallSoundZ ) bDropSound = true;

                Velocity = Velocity.WithZ(0);
                //player->m_Local.m_flFallVelocity = 0.0f;

                if (GroundEntity != null)
                {
                    ApplyFriction(sv_friction * SurfaceFriction);
                }
            }

            
            /*
            var inSpeed = WishVelocity.Length.Clamp(0, 1);
            WishVelocity *= Input.Rotation.Angles().WithPitch(0).ToRotation();

           

            WishVelocity = WishVelocity.Normal * inSpeed;
            WishVelocity *= GetWishSpeed();
            */
            Duck.PreTick();

            bool bStayOnGround = false;
            if (Swimming)
            {
                ApplyFriction(1);
                WaterMove();
            }
            else if (IsTouchingLadder)
            {
                SetTag("climbing");
                LadderMove();
            }
            else if (GroundEntity != null)
            {
                bStayOnGround = true;
                WalkMove();
            }
            else
            {
                AirMove();
                
            }

            CategorizePosition(bStayOnGround);

            // FinishGravity
            FinishGravity();


            if (GroundEntity != null)
            {
                Velocity = Velocity.WithZ(0);
            }
            CheckVelocity();
            // CheckFalling(); // fall damage etc

            // Land Sound
            // Swim Sounds

            SaveGroundPos();

            if (Debug)
            {
                DebugOverlay.Box(Position + TraceOffset, mins, maxs, Color.Red);
                DebugOverlay.Box(Position, mins, maxs, Color.Blue);

                var lineOffset = 0;
                if (Host.IsServer) lineOffset = 10;

                DebugOverlay.ScreenText($"        Position: {Position}", lineOffset + 0);
                DebugOverlay.ScreenText($"        Velocity: {Velocity}", lineOffset + 1);
                DebugOverlay.ScreenText($"    BaseVelocity: {BaseVelocity}", lineOffset + 2);
                DebugOverlay.ScreenText($"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]", lineOffset + 3);
                DebugOverlay.ScreenText($" SurfaceFriction: {SurfaceFriction}", lineOffset + 4);
                DebugOverlay.ScreenText($"    WishVelocity: {WishVelocity}", lineOffset + 5);
                DebugOverlay.ScreenText($"    Speed: {Velocity.Length}", lineOffset + 6);
            }

        }


        public virtual float GetWishSpeed()
        {
            QAngle a = Input.Rotation; //.AngleVectors(out var forward, out var right, out var up);
            a.AngleVectors(out var forward, out var right, out var up);
            Vector3 wishvel = 0;
            var oldGround = GroundEntity;
            var mvspeed = sv_defaultspeed; //0.0f;
            var ws = Duck.GetWishSpeed();
            if (ws >= 0) mvspeed = ws;

            if (Input.Down(InputButton.Walk)) mvspeed = sv_sprintspeed;
            if (Input.Down(InputButton.Run)) mvspeed = sv_walkspeed;
            var ForwardMove = Input.Forward * mvspeed;
            var SideMove = -Input.Left * mvspeed;
            var UpMove = Input.Up * mvspeed;

            var fmove = ForwardMove;
            var smove = SideMove;

            if (forward[2] != 0)
            {
                forward[2] = 0;
                forward = forward.Normal;
            }

            if (right[2] != 0)
            {
                right[2] = 0;
                right = right.Normal;
            }

            for (int i = 0; i < 2; i++)
                wishvel[i] = forward[i] * fmove + right[i] * smove;

            wishvel[2] = 0;
            var wishdir = wishvel.Normal;
            var wishspeed = wishvel.Length;

            if (wishspeed != 0 && wishspeed > sv_maxspeed)
            {
                wishvel *= sv_maxspeed / wishspeed;
                wishspeed = sv_maxspeed;
            }
            return wishspeed;
            /*
            var ws = Duck.GetWishSpeed();
            if (ws >= 0) return ws;

            if (Input.Down(InputButton.Run)) return SprintSpeed;
            if (Input.Down(InputButton.Walk)) return WalkSpeed;

            return DefaultSpeed;

            //
            // Work out wish velocity.. just take input, rotate it to view, clamp to -1, 1
            //
            QAngle a = Input.Rotation; //.AngleVectors(out var forward, out var right, out var up);
            a.AngleVectors(out var forward, out var right, out var up);
            Vector3 wishvel = 0;
            var oldGround = GroundEntity;
            var mvspeed = sv_defaultspeed; //0.0f;
            var ws = Duck.GetWishSpeed();
            if (ws >= 0) mvspeed = ws;

            if (Input.Down(InputButton.Walk)) mvspeed = sv_sprintspeed;
            if (Input.Down(InputButton.Run)) mvspeed = sv_walkspeed;
            var ForwardMove = Input.Forward * mvspeed;
            var SideMove = -Input.Left * mvspeed;
            var UpMove = Input.Up * mvspeed;

            float spd = (ForwardMove * ForwardMove) +
            (SideMove * SideMove) +
            (UpMove * UpMove);

            if ((spd != 0) && (spd > sv_maxspeed * sv_maxspeed))
            {
                var ratio = sv_maxspeed / MathF.Sqrt(spd);
                //ForwardMove *= ratio;
                //SideMove *= ratio;
                //UpMove *= ratio;
            }
            // if we are going upwards with this speed, we can't be standing on anything.
            if (Velocity.z > 250)
                GroundEntity = null;
            var fmove = ForwardMove;
            var smove = SideMove;

            if (forward[2] != 0)
            {
                forward[2] = 0;
                forward = forward.Normal;
            }

            if (right[2] != 0)
            {
                right[2] = 0;
                right = right.Normal;
            }

            for (int i = 0; i < 2; i++)
                wishvel[i] = forward[i] * fmove + right[i] * smove;

            wishvel[2] = 0;
            var wishdir = wishvel.Normal;
            var wishspeed = wishvel.Length;

            if (wishspeed != 0 && wishspeed > sv_maxspeed)
            {
                wishvel *= sv_maxspeed / wishspeed;
                wishspeed = sv_maxspeed;
            }

            WishVelocity = wishvel;
            if (!Swimming && !IsTouchingLadder)
            {
                WishVelocity = WishVelocity.WithZ(0);
            }
            */
        }

        public void fwishspd()
        {
            //
            // Work out wish velocity.. just take input, rotate it to view, clamp to -1, 1
            //
            QAngle a = Input.Rotation; //.AngleVectors(out var forward, out var right, out var up);
            a.AngleVectors(out var forward, out var right, out var up);
            Vector3 wishvel = 0;
            var oldGround = GroundEntity;
            var mvspeed = sv_defaultspeed; //0.0f;
            var ws = Duck.GetWishSpeed();
            if (ws >= 0) mvspeed = ws;

            if (Input.Down(InputButton.Walk)) mvspeed = sv_sprintspeed;
            if (Input.Down(InputButton.Run)) mvspeed = sv_walkspeed;
            var ForwardMove = Input.Forward * mvspeed;
            var SideMove = -Input.Left * mvspeed;
            var UpMove = Input.Up * mvspeed;

            float spd = (ForwardMove * ForwardMove) +
            (SideMove * SideMove) +
            (UpMove * UpMove);

            if ((spd != 0) && (spd > sv_maxspeed * sv_maxspeed))
            {
                var ratio = sv_maxspeed / MathF.Sqrt(spd);
                //ForwardMove *= ratio;
                //SideMove *= ratio;
                //UpMove *= ratio;
            }
            // if we are going upwards with this speed, we can't be standing on anything.
            if (Velocity.z > 250)
                GroundEntity = null;
            var fmove = ForwardMove;
            var smove = SideMove;

            if (forward[2] != 0)
            {
                forward[2] = 0;
                forward = forward.Normal;
            }

            if (right[2] != 0)
            {
                right[2] = 0;
                right = right.Normal;
            }

            for (int i = 0; i < 2; i++)
                wishvel[i] = forward[i] * fmove + right[i] * smove;

            wishvel[2] = 0;
            var wishdir = wishvel.Normal;
            var wishspeed = wishvel.Length;

            if (wishspeed != 0 && wishspeed > sv_maxspeed)
            {
                wishvel *= sv_maxspeed / wishspeed;
                wishspeed = sv_maxspeed;
            }

            WishVelocity = wishvel;
            if (!Swimming && !IsTouchingLadder)
            {
                WishVelocity = WishVelocity.WithZ(0);
            }
        }
        public virtual void WalkMove()
        {
            var wishdir = WishVelocity.Normal;
            var wishspeed = WishVelocity.Length;

            WishVelocity = WishVelocity.WithZ(0);
            WishVelocity = WishVelocity.Normal * wishspeed;

            Velocity = Velocity.WithZ(0);
            Accelerate(wishdir, wishspeed, 0, sv_accelerate);
            Velocity = Velocity.WithZ(0);

            //   Player.SetAnimParam( "forward", Input.Forward );
            //   Player.SetAnimParam( "sideward", Input.Right );
            //   Player.SetAnimParam( "wishspeed", wishspeed );
            //    Player.SetAnimParam( "walkspeed_scale", 2.0f / 190.0f );
            //   Player.SetAnimParam( "runspeed_scale", 2.0f / 320.0f );

            //  DebugOverlay.Text( 0, Pos + Vector3.Up * 100, $"forward: {Input.Forward}\nsideward: {Input.Right}" );

            // Add in any base velocity to the current velocity.
            Velocity += BaseVelocity;

            try
            {
                if (Velocity.Length < 1.0f)
                {
                    Velocity = Vector3.Zero;
                    return;
                }

                // first try just moving to the destination
                var dest = (Position + Velocity * Time.Delta).WithZ(Position.z);

                var pm = TraceBBox(Position, dest);

                if (pm.Fraction == 1)
                {
                    Position = pm.EndPosition;
                    StayOnGround();
                    return;
                }

                StepMove();
            }
            finally
            {

                // Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
                Velocity -= BaseVelocity;
            }

			StayOnGround();

			Velocity = Velocity.Normal * MathF.Min( Velocity.Length, GetWishSpeed() );
		}

        public virtual void StepMove()
        {
            if (!sv_use_sbox_movehelper)
            {
                StepMove2();
                return;
            }
            MoveHelper mover = new MoveHelper(Position, Velocity);
            mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Pawn);
            mover.MaxStandableAngle = sv_maxstandableangle;

            mover.TryMoveWithStep(Time.Delta, sv_stepsize);

            Position = mover.Position;
            Velocity = mover.Velocity;
        }

        public virtual void Move()
        {
            if (!sv_use_sbox_movehelper)
            {
                Move2();
                return;
            }
            MoveHelper mover = new MoveHelper(Position, Velocity);
            mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Pawn);
            mover.MaxStandableAngle = sv_maxstandableangle;

            mover.TryMove(Time.Delta);

            Position = mover.Position;
            Velocity = mover.Velocity;
        }
        protected string DescribeAxis(int axis)
        {
            switch (axis)
            {
                case 0: return "X";
                case 1: return "Y";
                case 2: default: return "Z";
            }
        }
        public void CheckVelocity()
        {
            for (int i = 0; i < 3; i++)
            {
                if (float.IsNaN(Velocity[i]))
                {
                    Log.Info($"Got a NaN velocity {DescribeAxis(i)}");
                    aVelocity = Velocity;
                    aVelocity[i] = 0;
                    Velocity = aVelocity;
                }

                if (float.IsNaN(Position[i]))
                {
                    Log.Info($"Got a NaN position {DescribeAxis(i)}");
                    aPosition = Position;
                    aPosition[i] = 0;
                    Position = aPosition;
                }

                if (Velocity[i] > sv_maxvelocity)
                {
                    Log.Info($"Got a velocity too high on {DescribeAxis(i)}");
                    aVelocity = Velocity;
                    aVelocity[i] = sv_maxvelocity;
                    Velocity = aVelocity;
                }

                if (Velocity[i] < -sv_maxvelocity)
                {
                    Log.Info($"Got a velocity too low on {DescribeAxis(i)}");
                    aVelocity = Velocity;
                    aVelocity[i] = -sv_maxvelocity;
                    Velocity = aVelocity;
                }
            }
        }
        /// <summary>
        /// Add our wish direction and speed onto our velocity
        /// </summary>
        public virtual void Accelerate(Vector3 wishdir, float wishspeed, float speedLimit, float acceleration)
        {
            // This gets overridden because some games (CSPort) want to allow dead (observer) players
            // to be able to move around.
            // if ( !CanAccelerate() )
            //     return;

            // See if we are changing direction a bit
            var currentspeed = Velocity.Dot(wishdir);

            var addspeed = wishspeed - currentspeed;
            if (addspeed <= 0)
                return;

            // Determine amount of acceleration.
            var accelspeed = acceleration * Time.Delta * wishspeed * SurfaceFriction;

            // Cap at addspeed
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            Velocity += accelspeed * wishdir;
        }

        public virtual void AirAccelerate(Vector3 wishdir, float wishspeed, float accel)
        {

            var wishspd = wishspeed;
            
            if (wishspd > sv_airspeedcap)
                wishspd = sv_airspeedcap;

            // See if we are changing direction a bit
            var currentspeed = Velocity.Dot(wishdir);

            // Reduce wishspeed by the amount of veer.
            var addspeed = wishspd - currentspeed;

            // If not going to add any speed, done.
            if (addspeed <= 0)
                return;

            // Determine amount of acceleration.
            var accelspeed = accel * wishspeed * Time.Delta * SurfaceFriction;

            // Cap at addspeed
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            //Velocity = Velocity.WithX(Velocity.x + accelspeed * wishdir.x);
            //Velocity = Velocity.WithY(Velocity.y + accelspeed * wishdir.y);
            //Velocity = Velocity.WithZ(Velocity.z + accelspeed * wishdir.z);
            Velocity += accelspeed * wishdir;
        }

        /// <summary>
        /// Remove ground friction from velocity
        /// </summary>
        public virtual void ApplyFriction(float frictionAmount = 1.0f)
        {
            // If we are in water jump cycle, don't apply friction
            //if ( player->m_flWaterJumpTime )
            //   return;

            // Not on ground - no friction


            // Calculate speed
            var speed = Velocity.Length;
            if (speed < 0.1f) return;

            // Bleed off some speed, but if we have less than the bleed
            //  threshold, bleed the threshold amount.
            float control = (speed < sv_stopspeed) ? sv_stopspeed : speed;

            // Add the amount to the drop amount.
            var drop = control * Time.Delta * frictionAmount;

            // scale the velocity
            float newspeed = speed - drop;
            if (newspeed < 0) newspeed = 0;

            if (newspeed != speed)
            {
                newspeed /= speed;
                Velocity *= newspeed;
            }

            // mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
        }

        public virtual void CheckJumpButton()
        {
            //if ( !player->CanJump() )
            //    return false;


            /*
            if ( player->m_flWaterJumpTime )
            {
                player->m_flWaterJumpTime -= gpGlobals->frametime();
                if ( player->m_flWaterJumpTime < 0 )
                    player->m_flWaterJumpTime = 0;

                return false;
            }*/



            // If we are in the water most of the way...
            if (Swimming)
            {
                // swimming, not jumping
                ClearGroundEntity();

                Velocity = Velocity.WithZ(100);

                // play swimming sound
                //  if ( player->m_flSwimSoundTime <= 0 )
                {
                    // Don't play sound again for 1 second
                    //   player->m_flSwimSoundTime = 1000;
                    //   PlaySwimSound();
                }

                return;
            }

            if (GroundEntity == null)
                return;
            if (!sv_enablebunnyhopping)
                PreventBunnyJumping();
            /*
            if ( player->m_Local.m_bDucking && (player->GetFlags() & FL_DUCKING) )
                return false;
            */

            /*
            // Still updating the eye position.
            if ( player->m_Local.m_nDuckJumpTimeMsecs > 0u )
                return false;
            */

            ClearGroundEntity();

            // player->PlayStepSound( (Vector &)mv->GetAbsOrigin(), player->m_pSurfaceData, 1.0, true );

            // MoveHelper()->PlayerSetAnimation( PLAYER_JUMP );

            //if ( player->m_pSurfaceData )
            {
                //   flGroundFactor = g_pPhysicsQuery->GetGameSurfaceproperties( player->m_pSurfaceData )->m_flJumpFactor;
            }

            //PreventBunnyJumping();
            //float flMul = 268.3281572999747f * 1.2f;
            float JumpImpulse = 268;
            float startz = Velocity.z;
            if (Duck.IsActive)
            {
                var a = Velocity;
                a.z = JumpImpulse;
                Velocity = a;
            }
            else
            {
                var a = Velocity;
                a.z += JumpImpulse;
                Velocity = a;
            }
            FinishGravity();
            /*
            if (Duck.IsActive)
                flMul *= 0.8f;

            Velocity = Velocity.WithZ(startz + flMul * flGroundFactor);

            Velocity -= new Vector3(0, 0, sv_gravity * 0.5f) * Time.Delta;
            */
            // mv->m_outJumpVel.z += mv->m_vecVelocity[2] - startz;
            // mv->m_outStepHeight += 0.15f;

            // don't jump again until released
            //mv->m_nOldButtons |= IN_JUMP;

            AddEvent("jump");

        }
        public virtual void PreventBunnyJumping()
        {
            // Speed at which bunny jumping is limited
            float maxscaledspeed = sv_maxspeed;
            if (maxscaledspeed <= 0.0f)
                return;

            // Current player speed
            float spd = Velocity.Length;
            if (spd <= maxscaledspeed)
                return;

            // Apply this cropping fraction to velocity
            float fraction = (maxscaledspeed / spd);

            Velocity *= fraction;
        }
        public virtual void AirMove()
        {
            /*
            ViewAngles.AngleVectors(out var forward, out var right, out var up);

            var fmove = ForwardMove;
            var smove = SideMove;
            Vector3 wishvel = 0;
            forward[2] = 0;
            right[2] = 0;
            forward = forward.Normal;
            right = right.Normal;

            Vector3 wishvel = 0;
            for (var i = 0; i < 2; i++)
                wishvel[i] = forward[i] * fmove + right[i] * smove;
            wishvel[2] = 0;
            
            
            QAngle a = Input.Rotation; //.AngleVectors(out var forward, out var right, out var up);
            a.AngleVectors(out var forward, out var right, out var up);

            var oldGround = GroundEntity;
            var MaxSpeed = sv_defaultspeed; //0.0f;
            var ws = Duck.GetWishSpeed();
            if (ws >= 0) MaxSpeed = ws;

            if (Input.Down(InputButton.Walk)) MaxSpeed = sv_sprintspeed;
            if (Input.Down(InputButton.Run)) MaxSpeed = sv_walkspeed;
            var ForwardMove = Input.Forward * MaxSpeed;
            var SideMove = -Input.Left * MaxSpeed;
            var UpMove = Input.Up * MaxSpeed;

            var fmove = ForwardMove;
            var smove = SideMove;

            if (forward[2] != 0)
            {
                forward[2] = 0;
                forward = forward.Normal;
            }

            if (right[2] != 0)
            {
                right[2] = 0;
                right = right.Normal;
            }

            for (int i = 0; i < 2; i++)
                wishvel[i] = forward[i] * fmove + right[i] * smove;

            wishvel[2] = 0;
            */
            var mvspeed = sv_defaultspeed; //0.0f;
            var ws = Duck.GetWishSpeed();
            if (ws >= 0) mvspeed = ws;

            if (Input.Down(InputButton.Walk)) mvspeed = sv_sprintspeed;
            if (Input.Down(InputButton.Run)) mvspeed = sv_walkspeed;
            
            var wishdir = WishVelocity.Normal;
            var wishspeed = WishVelocity.Length;

            if (wishspeed != 0 && wishspeed > sv_maxspeed)
            {
                WishVelocity *= sv_maxspeed / wishspeed;
                wishspeed = sv_maxspeed;
            }
            
            AirAccelerate(wishdir, wishspeed, sv_airaccelerate);

            Velocity += BaseVelocity;
            Move();
            Velocity -= BaseVelocity;
            
            //var wishdir = WishVelocity.Normal;
            //var wishspeed = WishVelocity.Length;

            //Accelerate(wishdir, wishspeed, AirControl, AirAcceleration);

            //Velocity += BaseVelocity;

            //Move();

            //Velocity -= BaseVelocity;
        }

        public virtual void WaterMove()
        {
            var wishdir = WishVelocity.Normal;
            var wishspeed = WishVelocity.Length;

            wishspeed *= 0.8f;

            Accelerate(wishdir, wishspeed, 100, sv_accelerate);

            Velocity += BaseVelocity;

            Move();

            Velocity -= BaseVelocity;
        }

        bool IsTouchingLadder = false;
        Vector3 LadderNormal;

        public virtual void CheckLadder()
        {
            var wishvel = new Vector3(Input.Forward, Input.Left, 0);
            wishvel *= Input.Rotation.Angles().WithPitch(0).ToRotation();
            wishvel = wishvel.Normal;

            if (IsTouchingLadder)
            {
                if (Input.Pressed(InputButton.Jump))
                {
                    Velocity = LadderNormal * 100.0f;
                    IsTouchingLadder = false;

                    return;

                }
                else if (GroundEntity != null && LadderNormal.Dot(wishvel) > 0)
                {
                    IsTouchingLadder = false;

                    return;
                }
            }

            const float ladderDistance = 1.0f;
            var start = Position;
            Vector3 end = start + (IsTouchingLadder ? (LadderNormal * -1.0f) : wishvel) * ladderDistance;

            var pm = Trace.Ray(start, end)
                        .Size(mins, maxs)
                        .WithTag("ladder")
                        .Ignore(Pawn)
                        .Run();

            IsTouchingLadder = false;

            if (pm.Hit)
            {
                IsTouchingLadder = true;
                LadderNormal = pm.Normal;
            }
        }

        public virtual void LadderMove()
        {
            var velocity = WishVelocity;
            float normalDot = velocity.Dot(LadderNormal);
            var cross = LadderNormal * normalDot;
            Velocity = (velocity - cross) + (-normalDot * LadderNormal.Cross(Vector3.Up.Cross(LadderNormal).Normal));

            Move();
        }


        public virtual void CategorizePosition(bool bStayOnGround)
        {
            SurfaceFriction = 1.0f;

            // Doing this before we move may introduce a potential latency in water detection, but
            // doing it after can get us stuck on the bottom in water if the amount we move up
            // is less than the 1 pixel 'threshold' we're about to snap to.	Also, we'll call
            // this several times per frame, so we really need to avoid sticking to the bottom of
            // water on each call, and the converse case will correct itself if called twice.
            //CheckWater();

            var point = Position - Vector3.Up * 2;
            var vBumpOrigin = Position;

            //
            //  Shooting up really fast.  Definitely not on ground trimed until ladder shit
            //
            bool bMovingUpRapidly = Velocity.z > sv_maxnonjumpvelocity;
            bool bMovingUp = Velocity.z > 0;

            bool bMoveToEndPos = false;

            if (GroundEntity != null) // and not underwater
            {
                bMoveToEndPos = true;
                point.z -= sv_stepsize;
            }
            else if (bStayOnGround)
            {
                bMoveToEndPos = true;
                point.z -= sv_stepsize;
            }

            if (bMovingUpRapidly || Swimming) // or ladder and moving up
            {
                ClearGroundEntity();
                return;
            }

            var pm = TraceBBox(vBumpOrigin, point, 4.0f);

            if (pm.Entity == null || Vector3.GetAngle(Vector3.Up, pm.Normal) > sv_maxstandableangle)
            {
                ClearGroundEntity();
                bMoveToEndPos = false;

                if (Velocity.z > 0)
                    SurfaceFriction = 0.25f;
            }
            else
            {
                UpdateGroundEntity(pm);
            }

            if (bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f)
            {
                Position = pm.EndPosition;
            }

        }

        /// <summary>
        /// We have a new ground entity
        /// </summary>
        public virtual void UpdateGroundEntity(TraceResult tr)
        {
            GroundNormal = tr.Normal;

            // VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
            // A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
            // This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
            SurfaceFriction = tr.Surface.Friction * 1.25f;
            if (SurfaceFriction > 1) SurfaceFriction = 1;

            //if ( tr.Entity == GroundEntity ) return;

            Vector3 oldGroundVelocity = default;
            if (GroundEntity != null) oldGroundVelocity = GroundEntity.Velocity;

            bool wasOffGround = GroundEntity == null;

            GroundEntity = tr.Entity;

            if (GroundEntity != null)
            {
                BaseVelocity = GroundEntity.Velocity;
            }
        }

        /// <summary>
        /// We're no longer on the ground, remove it
        /// </summary>
        public virtual void ClearGroundEntity()
        {
            if (GroundEntity == null) return;

            GroundEntity = null;
            GroundNormal = Vector3.Up;
            SurfaceFriction = 1.0f;
        }

        /// <summary>
        /// Traces the current bbox and returns the result.
        /// liftFeet will move the start position up by this amount, while keeping the top of the bbox at the same
        /// position. This is good when tracing down because you won't be tracing through the ceiling above.
        /// </summary>
        public override TraceResult TraceBBox(Vector3 start, Vector3 end, float liftFeet = 0.0f)
        {
            return TraceBBox(start, end, mins, maxs, liftFeet);
        }

        /// <summary>
        /// Try to keep a walking player on the ground when running down slopes etc
        /// </summary>
        public virtual void StayOnGround()
        {
            var start = Position + Vector3.Up * 2;
            var end = Position + Vector3.Down * sv_stepsize;

            // See how far up we can go without getting stuck
            var trace = TraceBBox(Position, start);
            start = trace.EndPosition;

            // Now trace down from a known safe position
            trace = TraceBBox(start, end);

            if (trace.Fraction <= 0) return;
            if (trace.Fraction >= 1) return;
            if (trace.StartedSolid) return;
            if (Vector3.GetAngle(Vector3.Up, trace.Normal) > sv_maxstandableangle) return;

            // This is incredibly hacky. The real problem is that trace returning that strange value we can't network over.
            // float flDelta = fabs( mv->GetAbsOrigin().z - trace.m_vEndPos.z );
            // if ( flDelta > 0.5f * DIST_EPSILON )

            Position = trace.EndPosition;
        }

        void RestoreGroundPos()
        {
            if (GroundEntity == null || GroundEntity.IsWorld)
                return;

            //var Position = GroundEntity.Transform.ToWorld( GroundTransform );
            //Pos = Position.Position;
        }

        void SaveGroundPos()
        {
            if (GroundEntity == null || GroundEntity.IsWorld)
                return;

            //GroundTransform = GroundEntity.Transform.ToLocal( new Transform( Pos, Rot ) );
        }
        public const float PLAYER_MAX_SAFE_FALL_SPEED = 580;
        public const float PLAYER_MIN_BOUNCE_SPEED = 200;

        public const float NON_JUMP_VELOCITY = 140;
        public const int MAX_CLIP_PLANES = 5;
        public const float DIST_EPSILON = 0.3125f;
        public virtual void StepMove2()
        {
            var vecEndPos = Position + Velocity * Time.Delta;

            // Try sliding forward both on ground and up 16 pixels
            //  take the move that goes farthest
            var vecPos = Position;
            var vecVel = Velocity;

            // Slide move down.
            Move();

            // Down results.
            var vecDownPos = Position;
            var vecDownVel = Velocity;

            // Reset original values.
            Position = vecPos;
            Velocity = vecVel;

            // Move up a stair height.
            vecEndPos = Position;
            vecEndPos.z += sv_stepsize + DIST_EPSILON;

            var trace = TraceBBox(Position, vecEndPos);
            Position = trace.EndPosition;

            // Slide move up.
            Move();

            // Move down a stair (attempt to).
            vecEndPos = Position;
            vecEndPos.z -= sv_stepsize + DIST_EPSILON;

            trace = TraceBBox(Position, vecEndPos);

            // If we are not on the ground any more then use the original movement attempt.
            if (trace.Normal.z < 0.7f)
            {
                Position = vecDownPos;
                Velocity = vecDownVel;
                return;
            }

            // If the trace ended up in empty space, copy the end over to the origin.
            if (!trace.StartedSolid /* && !trace.allsolid */)
            {
                Position = trace.EndPosition;
            }

            // Copy this origin to up.
            var vecUpPos = Position;

            // decide which one went farther
            float flDownDist = (vecDownPos.x - vecPos.x) * (vecDownPos.x - vecPos.x) + (vecDownPos.y - vecPos.y) * (vecDownPos.y - vecPos.y);
            float flUpDist = (vecUpPos.x - vecPos.x) * (vecUpPos.x - vecPos.x) + (vecUpPos.y - vecPos.y) * (vecUpPos.y - vecPos.y);
            if (flDownDist > flUpDist)
            {
                Position = vecDownPos;
                Velocity = vecDownVel;
            }
            else
            {
                // copy z value from slide move
                Velocity = Velocity.WithZ(vecDownVel.z);
            }
        }
        public int Move2()
        {
            int bumpcount, numbumps;
            Vector3 dir;
            float d;
            int numplanes;
            var planes = new Vector3[MAX_CLIP_PLANES];
            Vector3 primal_velocity, original_velocity;
            Vector3 new_velocity;
            int i, j;
            TraceResult pm;
            Vector3 end;
            float time_left, allFraction;
            int blocked;

            numbumps = 4;

            blocked = 0;
            numplanes = 0;

            original_velocity = Velocity;
            primal_velocity = Velocity;

            allFraction = 0;
            time_left = Time.Delta;

            new_velocity = 0;

            for (bumpcount = 0; bumpcount < numbumps; bumpcount++)
            {
                if (Velocity.Length == 0)
                    break;

                // Assume we can move all the way from the current origin to the
                // end point.
                end = Position + Velocity * time_left;
                pm = TraceBBox(Position, end);

                allFraction += pm.Fraction;

                // If we started in a solid object, or we were in solid space
                //  the whole way, zero out our velocity and return that we
                //  are blocked by floor and wall.
                if (pm.StartedSolid)
                {
                    Velocity = 0;
                    // entity is trapped in another solid
                    return 4;
                }

                // If we moved some portion of the total distance, then
                //  copy the end position into the pmove.origin and 
                //  zero the plane counter.
                if (pm.Fraction > 0)
                {
                    if (numbumps > 0 && pm.Fraction == 1)
                    {
                        // There's a precision issue with terrain tracing that can cause a swept box to successfully trace
                        // when the end position is stuck in the triangle.  Re-run the test with an uswept box to catch that
                        // case until the bug is fixed.
                        // If we detect getting stuck, don't allow the movement
                        var stuck = TraceBBox(pm.EndPosition, pm.EndPosition);
                        if (stuck.StartedSolid || stuck.Fraction != 1.0f)
                        {
                            // Log.Info( "Player will become stuck!!!\n" );
                            Velocity = 0;
                            break;
                        }

                        // actually covered some distance
                        Position = pm.EndPosition;
                        original_velocity = Velocity;
                        numplanes = 0;
                    }
                }

                // If we covered the entire distance, we are done
                //  and can return.
                if (pm.Fraction == 1)
                    break; // moved the entire distance

                if (pm.Normal.z > 0.7f)
                    blocked |= 1;   // floor

                if (pm.Normal.z == 0)
                    blocked |= 2;   // step / wall

                time_left -= time_left * pm.Fraction;

                // Did we run out of planes to clip against?
                if (numplanes >= MAX_CLIP_PLANES)
                {
                    // this shouldn't really happen
                    // Stop our movement if so.
                    Velocity = 0;
                    break;
                }

                planes[numplanes] = pm.Normal;
                numplanes++;

                // reflect player velocity 
                // Only give this a try for first impact plane because you can get yourself stuck in an acute corner by jumping in place
                //  and pressing forward and nobody was really using this bounce/reflection feature anyway...
                if (numplanes == 1 &&
                    GroundEntity == null)
                {
                    for (i = 0; i < numplanes; i++)
                    {
                        if (planes[i][2] > 0.7f)
                        {
                            ClipVelocity(original_velocity, planes[i], out new_velocity, 1);
                            original_velocity = new_velocity;
                        }
                        else
                        {
                            ClipVelocity(original_velocity, planes[i], out new_velocity, 1 + sv_bounce * (1 - SurfaceFriction));
                        }
                    }

                    Velocity = new_velocity;
                    original_velocity = new_velocity;
                }
                else
                {
                    for (i = 0; i < numplanes; i++)
                    {
                        ClipVelocity(
                            original_velocity,
                            planes[i],
                            out var a,
                            1);
                        Velocity = a;
                        for (j = 0; j < numplanes; j++)
                        {
                            if (j != i)
                            {
                                // Are we now moving against this plane?
                                if (Vector3.Dot(Velocity, planes[j]) < 0)
                                    break;  // not ok
                            }
                        }

                        if (j == numplanes) // Didn't have to clip, so we're ok
                            break;
                    }

                    if (i == numplanes)
                    {
                        // go along the crease
                        if (numplanes != 2)
                        {
                            Velocity = 0;
                            break;
                        }

                        dir = Vector3.Cross(planes[0], planes[1]);
                        dir = dir.Normal;
                        d = Vector3.Dot(dir, Velocity);
                        Velocity = d * dir;
                    }

                    //
                    // if original velocity is against the original velocity, stop dead
                    // to avoid tiny occilations in sloping corners
                    //
                    d = Vector3.Dot(Velocity, primal_velocity);
                    if (d <= 0)
                    {
                        //Con_DPrintf("Back\n");
                        Velocity = 0;
                        break;
                    }
                }
            }

            if (allFraction == 0)
                Velocity = 0;

            /*
            // Check if they slammed into a wall
            float fSlamVol = 0.0f;
            float fLateralStoppingAmount = primal_velocity.Length2D() - mv->m_vecVelocity.Length2D();
            if ( fLateralStoppingAmount > PLAYER_MAX_SAFE_FALL_SPEED * 2.0f )
            {
                fSlamVol = 1.0f;
            }
            else if ( fLateralStoppingAmount > PLAYER_MAX_SAFE_FALL_SPEED )
            {
                fSlamVol = 0.85f;
            }
            PlayerRoughLandingEffects( fSlamVol );
            */

            return blocked;
        }
        public int ClipVelocity(Vector3 vIn, Vector3 normal, out Vector3 vOut, float overbounce)
        {
            var angle = normal.z;

            var blocked = 0x00;
            if (angle > 0)
                blocked |= 0x01;
            if (angle == 0)
                blocked |= 0x02;

            var backoff = Vector3.Dot(vIn, normal) * overbounce;

            vOut = 0;
            for (var i = 0; i < 3; i++)
            {
                var change = normal[i] * backoff;
                vOut[i] = vIn[i] - change;
            }

            var adjust = Vector3.Dot(vOut, normal);
            if (adjust < 0)
            {
                vOut -= normal * adjust;
            }

            return blocked;
        }
    }
}
