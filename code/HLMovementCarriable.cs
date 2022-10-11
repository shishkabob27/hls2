class HLMovementCarriable : BaseCarriable
{

    private Vector3 mins = Vector3.Zero;
    private Vector3 maxs = Vector3.Zero;

    public Vector3 minsOverride;
    public Vector3 maxsOverride;

    public new Vector3 AngularVelocity;
    protected float SurfaceFriction;

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
    public float Gravity { get; set; } = 1.0f;
    public bool DontSleep = false;

    Entity lastTouch;
    Vector3 lastHitNormal;

    [Event.Tick]
    public void Tick()
    {
        Simulate();
    }

    public void Simulate()
    {
        if ( Owner is HLPlayer ) return; // Don't do physics if we are being carried
        if ( HLUtils.PlayerInRangeOf( Position, 2048 ) == false && !DontSleep )
            return;
        try
        {
            CalcGroundEnt();
            ApplyGravity();
            ApplyFriction( sv_friction * SurfaceFriction );
            ApplyAngularFriction( sv_friction * SurfaceFriction );
            Move();
        }
        catch
        {
        }
    }

    public void Move()
    {
        mins = new Vector3( -bGirth, -bGirth, 0 );
        maxs = new Vector3( +bGirth, +bGirth, bHeight );
        if ( minsOverride != Vector3.Zero )
        {
            mins = minsOverride;
        }
        if ( maxsOverride != Vector3.Zero )
        {
            maxs = maxsOverride;
        }
        NewMoveHelper mover = new( Position, Velocity );

        mover.Trace = mover.Trace
            .Size( mins, maxs )
            .Ignore( this )
            .WithoutTags( "player" );
        mover.GroundBounce = GroundBounce;
        mover.WallBounce = WallBounce;
        mover.TryMove( Time.Delta );
        if ( mover.HitWall || mover.HitFloor )
        {
            if ( mover.TraceResult.Normal != lastHitNormal )
            {
                this.Touch( this );
            }
            lastHitNormal = mover.TraceResult.Normal;
        }

        lastTouch = mover.TraceResult.Entity;
        Position = mover.Position;
        Velocity = mover.Velocity;
        Rotation = ( Rotation.Angles() + ( new Angles( AngularVelocity.x, AngularVelocity.y, AngularVelocity.z ) * Time.Delta ) ).ToRotation();
    }
    public void ApplyGravity()
    {
        Velocity -= new Vector3( 0, 0, ( sv_gravity * Gravity ) * 0.5f ) * Time.Delta;
        Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;

        BaseVelocity = BaseVelocity.WithZ( 0 );
    }
    public void CalcGroundEnt()
    {


        mins = new Vector3( -bGirth, -bGirth, 0 );
        maxs = new Vector3( +bGirth, +bGirth, bHeight );
        if ( minsOverride != Vector3.Zero )
        {
            mins = minsOverride;
        }
        if ( maxsOverride != Vector3.Zero )
        {
            maxs = maxsOverride;
        }
        SurfaceFriction = 1.0f;
        var point = Position - Vector3.Up * 2;
        var vBumpOrigin = Position;
        //if ( GroundEntity != null ) // and not underwater
        //{
        //bMoveToEndPos = true;
        //point.z -= 18;
        //}


        var pm = TraceBBox( vBumpOrigin, point, mins, maxs, 4.0f );

        if ( pm.Entity == null || Vector3.GetAngle( Vector3.Up, pm.Normal ) > GroundAngle )
        {
            ClearGroundEntity();
            if ( Velocity.z > 0 )
            {
                SurfaceFriction = 0.25f;
            }
        }
        else
        {
            UpdateGroundEntity( pm );
        }

    }
    public Vector3 TraceOffset = 0;

    /// <summary>
    /// Traces the bbox and returns the trace result.
    /// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
    /// position. This is good when tracing down because you won't be tracing through the ceiling above.
    /// </summary>
    public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
    {
        if ( liftFeet > 0 )
        {
            start += Vector3.Up * liftFeet;
            maxs = maxs.WithZ( maxs.z - liftFeet );
        }

        var tr = Trace.Ray( start + TraceOffset, end + TraceOffset )
                    .Size( mins, maxs )
                    .WithAnyTags( "solid" )
                    .Ignore( this )
                    .Run();

        tr.EndPosition -= TraceOffset;
        return tr;
    }
    /// <summary>
    /// We have a new ground entity
    /// </summary>
    public void UpdateGroundEntity( TraceResult tr )
    {
        var GroundNormal = tr.Normal;

        // VALVE HACKHACK: Scale this to fudge the relationship between vphysics friction values and player friction values.
        // A value of 0.8f feels pretty normal for vphysics, whereas 1.0f is normal for players.
        // This scaling trivially makes them equivalent.  REVISIT if this affects low friction surfaces too much.
        SurfaceFriction = tr.Surface.Friction * 1.25f;
        if ( SurfaceFriction > 1 ) SurfaceFriction = 1;

        //if ( tr.Entity == GroundEntity ) return;

        Vector3 oldGroundVelocity = default;
        if ( GroundEntity != null ) oldGroundVelocity = GroundEntity.Velocity;

        bool wasOffGround = GroundEntity == null;

        GroundEntity = tr.Entity;

        if ( GroundEntity != null )
        {
            BaseVelocity = GroundEntity.Velocity;
        }
        if ( wasOffGround )
        {

            //this.StartTouch(this);
        }

    }

    /// <summary>
    /// We're no longer on the ground, remove it
    /// </summary>
    public void ClearGroundEntity()
    {

        if ( GroundEntity == null ) return;
        this.EndTouch( this );
        GroundEntity = null;
        var GroundNormal = Vector3.Up;
        SurfaceFriction = 1.0f;
    }

    public void ApplyFriction( float frictionAmount = 1.0f )
    {
        // If we are in water jump cycle, don't apply friction
        //if ( player->m_flWaterJumpTime )
        //   return;

        // Not on ground - no friction
        if ( GroundEntity == null )
            return;
        frictionAmount += ( Friction - 1 );

        // Calculate speed
        var speed = Velocity.Length;
        if ( speed < 0.1f ) return;

        // Bleed off some speed, but if we have less than the bleed
        //  threshold, bleed the threshold amount.
        float control = ( speed < sv_stopspeed ) ? sv_stopspeed : speed;

        // Add the amount to the drop amount.
        var drop = control * Time.Delta * frictionAmount;

        // scale the velocity
        float newspeed = speed - drop;
        if ( newspeed < 0 ) newspeed = 0;

        if ( newspeed != speed )
        {
            newspeed /= speed;
            Velocity *= newspeed;
        }

        // mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
    }
    public void ApplyAngularFriction( float frictionAmount = 1.0f )
    {
        // If we are in water jump cycle, don't apply friction
        //if ( player->m_flWaterJumpTime )
        //   return;

        // Not on ground - no friction
        if ( GroundEntity == null )
            return;
        frictionAmount += ( Friction - 1 );

        // Calculate speed
        var speed = AngularVelocity.Length;
        if ( speed < 0.1f ) return;

        // Bleed off some speed, but if we have less than the bleed
        //  threshold, bleed the threshold amount.
        float control = ( speed < sv_stopspeed ) ? sv_stopspeed : speed;

        // Add the amount to the drop amount.
        var drop = control * Time.Delta * frictionAmount;

        // scale the velocity
        float newspeed = speed - drop;
        if ( newspeed < 0 ) newspeed = 0;

        if ( newspeed != speed )
        {
            newspeed /= speed;
            AngularVelocity *= newspeed;
        }

        // mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity;
    }
}
