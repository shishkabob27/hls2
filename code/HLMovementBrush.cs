using Sandbox;
using System;
using System.Net;
using static Sandbox.Package;

public class HLMovementBrush: BrushEntity, IUse
{

    private Vector3 mins;
    private Vector3 maxs;
    public float frictionmv = 80;

    // Config
    public float bGirth = 1 * 0.8f;
    public float bHeight = 1;


    [ConVar.Replicated] public static float sv_gravity { get; set; } = 800;
    [ConVar.Replicated] public static float sv_friction { get; set; } = 4;
    [ConVar.Replicated] public static float sv_stopspeed { get; set; } = 100;
    public float Friction { get; set; } = 1.0f;
    public float GroundBounce { get; set; } = 0.1f;
    public float WallBounce { get; set; } = 0.1f;
    public float GroundAngle { get; set; } = 46.0f;

    public float SurfaceFriction;
    public override void Spawn()
    {

        base.Spawn();
        //SetupPhysicsFromOBB(PhysicsMotionType.Static, CollisionBounds.Mins, CollisionBounds.Maxs);
        EnableAllCollisions = true;
        EnableTouch = true;
        Tags.Add("funcpush", "solid");
    }
    [Event.Tick.Server]
     void Tick()
    {
        Simulate();
    }

    public void Simulate()
    {
            FindTouching();
            CalcGroundEnt();
            ApplyFriction(sv_friction * SurfaceFriction);
            ApplyGravity();
            Move();
    }

    public void Move()
    {
        
        var mib = CollisionBounds.Mins;
        var mab = CollisionBounds.Maxs;
        mins = new Vector3(mib.x, mib.y, mib.z);
        maxs = new Vector3(mab.x, mab.y, mab.z);
        NewMoveHelper mover = new NewMoveHelper(Position, Velocity);
        mover.Trace = mover.Trace
            .Size(mins, maxs)
            .Ignore(this);
        mover.GroundBounce = GroundBounce;
        mover.WallBounce = WallBounce;
        mover.TryMoveWithStep(Time.Delta, 18);
        //mover.TryUnstuck();

        //lastTouch = mover.HitEntity;
        Position = mover.Position;
        Velocity = mover.Velocity;
    }
    public void ApplyGravity()
    {
        Velocity -= new Vector3(0, 0, sv_gravity * 0.5f) * Time.Delta;
        Velocity += new Vector3(0, 0, BaseVelocity.z) * Time.Delta;


        BaseVelocity = BaseVelocity.WithZ(0);
    }
    public void FindTouching()
    {

        var mib = CollisionBounds.Mins;
        var mab = CollisionBounds.Maxs;
        float sizeAdd = 0.064f;
        var emins = new Vector3(mib.x - sizeAdd, mib.y - sizeAdd, mib.z - sizeAdd);
        var emaxs = new Vector3(mab.x + sizeAdd, mab.y + sizeAdd, mab.z + sizeAdd);
        var tr = Trace.Ray(Position, Position)
                    .Size(emins, emaxs)
                    .EntitiesOnly()
                    .Ignore(this)
                    .Run();

        if (tr.Entity is not null)
        {
            Touch(tr.Entity);

        }
    }
    public void ApplyPush(Entity lastTouch, bool push)
    {
        if (IsClient) return;
        var temp = Velocity;
        bool playerTouch = false;
        if (lastTouch is HLPlayer && lastTouch.GroundEntity != this)
        {
            var IN_USE = (lastTouch as HLPlayer).IN_USE;
            var IN_FORWARD = (lastTouch as HLPlayer).IN_FORWARD;
            var factor = 2.0f;
            if (push && !(IN_FORWARD|IN_USE))
            {
                return;
            }
            playerTouch = true;
            //if (push) factor = 0.25f;
            temp.x = temp.x + (lastTouch as HLPlayer).WishVelocity.x * factor;
            temp.y = temp.y + (lastTouch as HLPlayer).WishVelocity.y * factor;


            float length = (float)Math.Sqrt(temp.x * temp.x + temp.y * temp.y); //(lastTouch as HLPlayer).WishVelocity.Length; //magnitude
            if (push && (length > (400 - frictionmv)))
            {
                temp.x = (temp.x * (400 - frictionmv) / length);
                temp.y = (temp.y * (400 - frictionmv) / length);
            }

            Velocity = temp;

            if (playerTouch)
            {

                var temp2 = (lastTouch as HLPlayer).Velocity;
                temp2.x = temp.x;
                temp2.y = temp.y;
                (lastTouch as HLPlayer).Velocity = temp2;
            }


        }
    }

    public override void Touch(Entity other)
    {
        base.Touch(other);
        ApplyPush(other, true);
    }
    public void CalcGroundEnt()
    {
        //mins = this.Model.Bounds.Mins;//new Vector3(this.PhysicsBody.GetBounds().Mins.x, this.PhysicsBody.GetBounds().Mins.y, this.PhysicsBody.GetBounds().Mins.z);
        //mins = this.Model.Bounds.Maxs;
        SurfaceFriction = 1.0f;
        var point = Position - Vector3.Up * 2;
        var vBumpOrigin = Position;
        //if ( GroundEntity != null ) // and not underwater
        //{
        //bMoveToEndPos = true;
        //point.z -= 18;
        //}


        var pm = TraceBBox(vBumpOrigin, point, mins, maxs, 4.0f);

        if (pm.Entity == null || Vector3.GetAngle(Vector3.Up, pm.Normal) > GroundAngle)
        {
            ClearGroundEntity();
            if (Velocity.z > 0)
            {
                //SurfaceFriction = 0.25f;
            }
        }
        else
        {
            UpdateGroundEntity(pm);
        }

    }
    public Vector3 TraceOffset;

    /// <summary>
    /// Traces the bbox and returns the trace result.
    /// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
    /// position. This is good when tracing down because you won't be tracing through the ceiling above.
    /// </summary>
    public virtual TraceResult TraceBBox(Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f)
    {
        if (liftFeet > 0)
        {
            start += Vector3.Up * liftFeet;
            maxs = maxs.WithZ(maxs.z - liftFeet);
        }

        var tr = Trace.Ray(start + TraceOffset, end + TraceOffset)
                    .Size(mins, maxs)
                    .WithAnyTags("solid")
                    .Ignore(this)
                    .Run();
        
        tr.EndPosition -= TraceOffset;
        return tr;
    }
    /// <summary>
    /// We have a new ground entity
    /// </summary>
    public void UpdateGroundEntity(TraceResult tr)
    {

        var GroundNormal = tr.Normal;

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
        if (wasOffGround)
        {

            this.StartTouch(this);
        }

    }

    /// <summary>
    /// We're no longer on the ground, remove it
    /// </summary>
    public void ClearGroundEntity()
    {

        if (GroundEntity == null) return;
        this.EndTouch(this);
        GroundEntity = null;
        var GroundNormal = Vector3.Up;
        SurfaceFriction = 1.0f;
    }
    public virtual void ApplyFriction(float frictionAmount = 1.0f)
    {

        var speed = Velocity.Length;
        if (speed < 0.1f)
            return;

        var drop = 0f;

        if (GroundEntity != null)
        {
            var friction = sv_friction * SurfaceFriction;
            var control = (speed < sv_stopspeed) ? sv_stopspeed : speed;

            // Add the amount to the drop amount.
            drop += control * friction * Time.Delta;
        }

        // scale the velocity
        float newspeed = speed - drop;
        if (newspeed < 0)
            newspeed = 0;

        if (newspeed != speed)
        {
            newspeed /= speed;
            Velocity *= newspeed;
        }
    }
    public void ApplyFrictionold(float frictionAmount = 1.0f)
    {
        // If we are in water jump cycle, don't apply friction
        //if ( player->m_flWaterJumpTime )
        //   return;

        // Not on ground - no friction
        if (GroundEntity == null)
            return;
        //frictionAmount = frictionAmount + (Friction - 1);

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

    public bool OnUse(Entity user)
    {
        if(user.Velocity != Vector3.Zero)
            ApplyPush(user, false);
        return true;
    }

    public bool IsUsable(Entity user)
    {
        return true;
    }
}