

[Library( "weapon_physgun" )]
partial class PhysGun : HLWeapon
{
    public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
    public override int Bucket => 0;
    public override int BucketWeight => 300;

    protected PhysicsBody heldBody;
    protected Vector3 heldPos;
    protected Rotation heldRot;
    protected Vector3 holdPos;
    protected Rotation holdRot;
    protected float holdDistance;
    protected bool grabbing;

    protected virtual float MinTargetDistance => 0.0f;
    protected virtual float MaxTargetDistance => 10000.0f;
    protected virtual float LinearFrequency => 20.0f;
    protected virtual float LinearDampingRatio => 1.0f;
    protected virtual float AngularFrequency => 20.0f;
    protected virtual float AngularDampingRatio => 1.0f;
    protected virtual float TargetDistanceSpeed => 50.0f;
    protected virtual float RotateSpeed => 1.0f;
    protected virtual float RotateSnapAt => 45.0f;

    private const string grabbedTag = "grabbed";

    public override string InventoryIcon => "/ui/weapons/weapon_gauss.png";
    [Net] public bool BeamActive { get; set; }
    [Net] public Entity GrabbedEntity { get; set; }
    [Net] public int GrabbedBone { get; set; }
    [Net] public Vector3 GrabbedPos { get; set; }

    public PhysicsBody HeldBody => heldBody;

    private Vector3 rotateInput;
    private Vector3 lastRotateInput;

    public override void Spawn()
    {
        base.Spawn();

        Tags.Add( "weapon" );
        SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
    }

    public override void Simulate( Client client )
    {
        if ( Owner is not Player owner ) return;

        var eyePos = owner.EyePosition;
        var eyeDir = owner.EyeRotation.Forward;
        var eyeRot = Rotation.From( new Angles( 0.0f, owner.EyeRotation.Yaw(), 0.0f ) );

        if ( Input.Pressed( InputButton.PrimaryAttack ) )
        {
            ( Owner as AnimatedEntity )?.SetAnimParameter( "b_attack", true );

            if ( !grabbing )
                grabbing = true;
        }

        bool grabEnabled = grabbing && Input.Down( InputButton.PrimaryAttack );
        bool wantsToFreeze = Input.Pressed( InputButton.SecondaryAttack );

        if ( GrabbedEntity.IsValid() && wantsToFreeze )
        {
            ( Owner as AnimatedEntity )?.SetAnimParameter( "b_attack", true );
        }

        BeamActive = grabEnabled;

        if ( IsServer )
        {
            using ( Prediction.Off() )
            {
                if ( grabEnabled )
                {
                    if ( heldBody.IsValid() )
                    {
                        UpdateGrab( eyePos, eyeRot, eyeDir, wantsToFreeze );
                    }
                    else
                    {
                        TryStartGrab( eyePos, eyeRot, eyeDir );
                    }
                }
                else if ( grabbing )
                {
                    GrabEnd();
                }

                if ( !grabbing && Input.Pressed( InputButton.Reload ) )
                {
                    TryUnfreezeAll( eyePos, eyeRot, eyeDir );
                }
            }
        }

        if ( BeamActive )
        {
            Input.MouseWheel = 0;
        }
    }

    private void TryUnfreezeAll( Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir )
    {
        var tr = Trace.Ray( eyePos, eyePos + eyeDir * MaxTargetDistance )
            .UseHitboxes()
            .Ignore( this )
            .Run();

        if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld ) return;

        var rootEnt = tr.Entity.Root;
        if ( !rootEnt.IsValid() ) return;

        var physicsGroup = rootEnt.PhysicsGroup;
        if ( physicsGroup == null ) return;

        bool unfrozen = false;

        for ( int i = 0; i < physicsGroup.BodyCount; ++i )
        {
            var body = physicsGroup.GetBody( i );
            if ( !body.IsValid() ) continue;

            if ( body.BodyType == PhysicsBodyType.Static )
            {
                body.BodyType = PhysicsBodyType.Dynamic;
                unfrozen = true;
            }
        }

        if ( unfrozen )
        {
            var freezeEffect = Particles.Create( "particles/physgun_freeze.vpcf" );
            freezeEffect.SetPosition( 0, tr.EndPosition );
        }
    }

    private void TryStartGrab( Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir )
    {
        var tr = Trace.Ray( eyePos, eyePos + eyeDir * MaxTargetDistance )
            .UseHitboxes()
            .WithAnyTags( "solid", "player" )
            .Ignore( this )
            .Run();

        if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld || tr.StartedSolid ) return;

        var rootEnt = tr.Entity.Root;
        var body = tr.Body;

        if ( !body.IsValid() || tr.Entity.Parent.IsValid() )
        {
            if ( rootEnt.IsValid() && rootEnt.PhysicsGroup != null )
            {
                body = ( rootEnt.PhysicsGroup.BodyCount > 0 ? rootEnt.PhysicsGroup.GetBody( 0 ) : null );
            }
        }

        if ( !body.IsValid() )
            return;

        //
        // Don't move keyframed, unless it's a player, NPC or Door
        //
        if ( body.BodyType == PhysicsBodyType.Keyframed && rootEnt is not Player or NPC or DoorEntity )
            return;

        //
        // Unfreeze
        //
        if ( body.BodyType == PhysicsBodyType.Static )
        {
            body.BodyType = PhysicsBodyType.Dynamic;
        }

        if ( rootEnt.Tags.Has( grabbedTag ) )
            return;

        GrabInit( body, eyePos, tr.EndPosition, eyeRot );

        GrabbedEntity = rootEnt;
        GrabbedEntity.Tags.Add( grabbedTag );

        GrabbedPos = body.Transform.PointToLocal( tr.EndPosition );
        GrabbedBone = body.GroupIndex;

        Client?.Pvs.Add( GrabbedEntity );
    }

    private void UpdateGrab( Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir, bool wantsToFreeze )
    {
        if ( wantsToFreeze )
        {
            if ( heldBody.BodyType == PhysicsBodyType.Dynamic )
            {
                heldBody.BodyType = PhysicsBodyType.Static;
            }

            if ( GrabbedEntity.IsValid() )
            {
                var freezeEffect = Particles.Create( "particles/physgun_freeze.vpcf" );
                freezeEffect.SetPosition( 0, heldBody.Transform.PointToWorld( GrabbedPos ) );
            }

            GrabEnd();
            return;
        }

        MoveTargetDistance( Input.MouseWheel * TargetDistanceSpeed );

        bool rotating = Input.Down( InputButton.Use );
        bool snapping = false;

        if ( rotating )
        {
            DoRotate( eyeRot, Input.Cursor.Direction - lastRotateInput );
            lastRotateInput = Input.Cursor.Direction;

            snapping = Input.Down( InputButton.Run );
        }
        else
        {
            lastRotateInput = 0;
        }

        GrabMove( eyePos, eyeDir, eyeRot, snapping );
    }

    private void Activate()
    {
        if ( !IsServer )
        {
            return;
        }
    }

    private void Deactivate()
    {
        if ( IsServer )
        {
            GrabEnd();
        }

        rotateInput = 0;
    }

    public override void ActiveStart( Entity ent )
    {
        base.ActiveStart( ent );

        Activate();
    }

    public override void ActiveEnd( Entity ent, bool dropped )
    {
        base.ActiveEnd( ent, dropped );

        Deactivate();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Deactivate();
    }

    public override void OnCarryDrop( Entity dropper )
    {
    }

    private void GrabInit( PhysicsBody body, Vector3 startPos, Vector3 grabPos, Rotation rot )
    {
        if ( !body.IsValid() )
            return;

        GrabEnd();

        grabbing = true;
        heldBody = body;
        holdDistance = Vector3.DistanceBetween( startPos, grabPos );
        holdDistance = holdDistance.Clamp( MinTargetDistance, MaxTargetDistance );

        heldRot = rot.Inverse * heldBody.Rotation;
        heldPos = heldBody.Transform.PointToLocal( grabPos );

        holdPos = heldBody.Position;
        holdRot = heldBody.Rotation;

        heldBody.Sleeping = false;
        heldBody.AutoSleep = false;
    }

    private void GrabEnd()
    {
        if ( heldBody.IsValid() )
        {
            heldBody.AutoSleep = true;
        }

        Client?.Pvs.Remove( GrabbedEntity );

        if ( GrabbedEntity.IsValid() )
        {
            GrabbedEntity.Tags.Remove( grabbedTag );
            GrabbedEntity = null;
        }

        heldBody = null;
        grabbing = false;

        lastRotateInput = 0;
    }

    [Event.Physics.PreStep]
    public void OnPrePhysicsStep()
    {
        if ( !IsServer )
            return;

        if ( !heldBody.IsValid() )
            return;

        if ( GrabbedEntity is Player )
            return;

        var velocity = heldBody.Velocity;
        Vector3.SmoothDamp( heldBody.Position, holdPos, ref velocity, 0.075f, Time.Delta );
        heldBody.Velocity = velocity;

        var angularVelocity = heldBody.AngularVelocity;
        Rotation.SmoothDamp( heldBody.Rotation, holdRot, ref angularVelocity, 0.075f, Time.Delta );
        heldBody.AngularVelocity = angularVelocity;
    }

    private void GrabMove( Vector3 startPos, Vector3 dir, Rotation rot, bool snapAngles )
    {
        if ( !heldBody.IsValid() )
            return;

        holdPos = startPos - heldPos * heldBody.Rotation + dir * holdDistance;

        if ( GrabbedEntity is Player player )
        {
            var velocity = player.Velocity;
            Vector3.SmoothDamp( player.Position, holdPos, ref velocity, 0.075f, Time.Delta );
            player.Velocity = velocity;
            player.GroundEntity = null;

            return;
        }

        holdRot = rot * heldRot;

        if ( snapAngles )
        {
            var angles = holdRot.Angles();

            holdRot = Rotation.From(
                MathF.Round( angles.pitch / RotateSnapAt ) * RotateSnapAt,
                MathF.Round( angles.yaw / RotateSnapAt ) * RotateSnapAt,
                MathF.Round( angles.roll / RotateSnapAt ) * RotateSnapAt
            );
        }
    }

    private void MoveTargetDistance( float distance )
    {
        holdDistance += distance;
        holdDistance = holdDistance.Clamp( MinTargetDistance, MaxTargetDistance );
    }

    protected virtual void DoRotate( Rotation eye, Vector3 input )
    {
        var localRot = eye;
        localRot *= Rotation.FromAxis( Vector3.Up, input.x * RotateSpeed );
        localRot *= Rotation.FromAxis( Vector3.Right, input.y * RotateSpeed );
        localRot = eye.Inverse * localRot;

        heldRot = localRot * heldRot;
    }

    public override void BuildInput( InputBuilder owner )
    {
        if ( !owner.Down( InputButton.Use ) ||
             !owner.Down( InputButton.PrimaryAttack ) ||
             !GrabbedEntity.IsValid() )
        {
            rotateInput = Vector3.Zero;

            return;
        }

        rotateInput.x += -owner.AnalogLook.yaw;
        rotateInput.y += owner.AnalogLook.pitch;

        //
        // We need to get accumulated look delta to the server so put it into something we don't use
        //
        owner.Cursor.Direction = rotateInput;

        //
        // Lock view angles
        //
        owner.ViewAngles = owner.OriginalViewAngles;
    }
}