using Sandbox.Component;

[Library( "weapon_physgun" )]
partial class PhysGun : HLWeapon
{
    public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
    public override string InventoryIcon => "/ui/weapons/weapon_gauss.png";
    public override string InventoryIconSelected => "/ui/weapons/weapon_gauss_selected.png";
    public override int Bucket => 0;
    public override int BucketWeight => 1000;

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
    protected virtual float TargetDistanceSpeed => 25.0f;
    protected virtual float RotateSpeed => 0.125f;
    protected virtual float RotateSnapAt => 45.0f;

    public const string GrabbedTag = "grabbed";

    [Net] public bool BeamActive { get; set; }
    [Net] public Entity GrabbedEntity { get; set; }
    [Net] public int GrabbedBone { get; set; }
    [Net] public Vector3 GrabbedPos { get; set; }

    public PhysicsBody HeldBody => heldBody;

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
        // Don't move keyframed, unless it's a player
        //
        //if ( body.BodyType == PhysicsBodyType.Keyframed && rootEnt is not Player )
        //return;

        //
        // Unfreeze
        //
        if ( body.BodyType == PhysicsBodyType.Static )
        {
            body.BodyType = PhysicsBodyType.Dynamic;
        }

        if ( rootEnt.Tags.Has( GrabbedTag ) )
            return;

        GrabInit( body, eyePos, tr.EndPosition, eyeRot );

        GrabbedEntity = rootEnt;
        GrabbedEntity.Tags.Add( GrabbedTag );
        GrabbedEntity.Tags.Add( $"{GrabbedTag}{Client.PlayerId}" );

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
            DoRotate( eyeRot, Input.MouseDelta * RotateSpeed );
            snapping = Input.Down( InputButton.Run );
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

        KillEffects();
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
            GrabbedEntity.Tags.Remove( GrabbedTag );
            GrabbedEntity.Tags.Remove( $"{GrabbedTag}{Client.PlayerId}" );
            GrabbedEntity = null;
        }

        heldBody = null;
        grabbing = false;
    }

    [Event.Physics.PreStep]
    public void OnPrePhysicsStep()
    {
        if ( !IsServer )
            return;

        if ( !heldBody.IsValid() )
            return;

        if ( GrabbedEntity is Player || GrabbedEntity is NPC )
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


        holdPos = startPos - heldPos * heldBody.Rotation + dir * holdDistance;

        holdRot = rot * heldRot;

        if ( !heldBody.IsValid() )
        {
            GrabbedEntity.Position = holdPos;
            GrabbedEntity.Rotation = holdRot;
            return;
        }
        if ( GrabbedEntity is DoorEntity || GrabbedEntity is DoorRotatingEntity || GrabbedEntity is DoorEntity )
        {
            GrabbedEntity.Position = holdPos;
            GrabbedEntity.Rotation = holdRot;
            return;

        }
        if ( GrabbedEntity is Player || GrabbedEntity is NPC || GrabbedEntity is func_train || GrabbedEntity is func_wall )
        {
            var velocity = GrabbedEntity.Velocity;
            Vector3.SmoothDamp( GrabbedEntity.Position, holdPos, ref velocity, 0.075f, Time.Delta );
            GrabbedEntity.Velocity = velocity;
            GrabbedEntity.GroundEntity = null;

            return;
        }



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
            return;
        }

        //
        // Lock view angles
        //
        owner.ViewAngles = owner.OriginalViewAngles;
    }

    //public override bool IsUsable( Entity user )
    //{
    //return Owner == null || HeldBody.IsValid();
    //}

    Particles Beam;
    Particles EndNoHit;

    Vector3 lastBeamPos;
    ModelEntity lastGrabbedEntity;

    [Event.Frame]
    public void OnFrame()
    {
        UpdateEffects();
    }

    protected virtual void KillEffects()
    {
        Beam?.Destroy( true );
        Beam = null;
        BeamActive = false;

        EndNoHit?.Destroy( false );
        EndNoHit = null;

        if ( lastGrabbedEntity.IsValid() )
        {
            foreach ( var child in lastGrabbedEntity.Children.OfType<ModelEntity>() )
            {
                if ( child is Player )
                    continue;

                if ( child.Components.TryGet<Glow>( out var childglow ) )
                {
                    childglow.Enabled = false;
                }
            }

            if ( lastGrabbedEntity.Components.TryGet<Glow>( out var glow ) )
            {
                glow.Enabled = false;
            }

            lastGrabbedEntity = null;
        }
    }

    protected virtual void UpdateEffects()
    {
        var owner = Owner as Player;

        if ( owner == null || !BeamActive || owner.ActiveChild != this )
        {
            KillEffects();
            return;
        }

        var startPos = owner.EyePosition;
        var dir = owner.EyeRotation.Forward;

        var tr = Trace.Ray( startPos, startPos + dir * MaxTargetDistance )
            .UseHitboxes()
            .Ignore( owner, false )
            .WithAllTags( "solid" )
            .Run();

        if ( Beam == null )
        {
            Beam = Particles.Create( "particles/physgun_beam.vpcf", tr.EndPosition );
        }

        Beam.SetEntityAttachment( 0, EffectEntity, "muzzle", true );

        if ( GrabbedEntity.IsValid() && !GrabbedEntity.IsWorld )
        {
            var physGroup = GrabbedEntity.PhysicsGroup;

            if ( physGroup != null && GrabbedBone >= 0 )
            {
                var physBody = physGroup.GetBody( GrabbedBone );
                if ( physBody != null )
                {
                    Beam.SetPosition( 1, physBody.Transform.PointToWorld( GrabbedPos ) );
                }
            }
            else
            {
                Beam.SetEntity( 1, GrabbedEntity, GrabbedPos, true );
            }

            lastBeamPos = GrabbedEntity.Position + GrabbedEntity.Rotation * GrabbedPos;

            EndNoHit?.Destroy( false );
            EndNoHit = null;

            if ( GrabbedEntity is ModelEntity modelEnt )
            {
                lastGrabbedEntity = modelEnt;

                var glow = modelEnt.Components.GetOrCreate<Glow>();
                glow.Enabled = true;
                glow.RangeMin = 0;
                glow.RangeMax = 1000;
                glow.Color = new Color( 4f, 50.0f, 70.0f, 1.0f );

                foreach ( var child in lastGrabbedEntity.Children.OfType<ModelEntity>() )
                {
                    if ( child is Player )
                        continue;

                    glow = child.Components.GetOrCreate<Glow>();
                    glow.Enabled = true;
                    glow.RangeMin = 0;
                    glow.RangeMax = 1000;
                    glow.Color = new Color( 0.1f, 1.0f, 1.0f, 1.0f );
                }
            }
        }
        else
        {
            lastBeamPos = tr.EndPosition;// Vector3.Lerp( lastBeamPos, tr.EndPosition, Time.Delta * 10 );
            Beam.SetPosition( 1, lastBeamPos );

            if ( EndNoHit == null )
                EndNoHit = Particles.Create( "particles/physgun_end_nohit.vpcf", lastBeamPos );

            EndNoHit.SetPosition( 0, lastBeamPos );
        }
    }
}