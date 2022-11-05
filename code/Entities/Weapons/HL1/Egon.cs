[Library( "weapon_egon" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/egon.vmdl" )]
[Title( "Egon" ), Category( "Weapons" ), MenuCategory( "Half-Life" )]
partial class Egon : Weapon
{
    //stub
    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/egon.vmdl" );
    public override string ViewModelPath => "models/hl1/weapons/view/v_egon.vmdl";
	public override string WorldModelPath => "models/hl1/weapons/world/egon.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 3;
    public override AmmoType AmmoType => AmmoType.Uranium;
    public override int ClipSize => -1;
    public override string AmmoIcon => "ui/ammo7.png";
    float AmmoUseTime;
    float dmgtime;
    Sound currentsound;
    float rundelay;
    bool hasStartedrun;
	public override bool HasHDModel => true;
	public override string CrosshairIcon => "/ui/crosshairs/crosshair11.png";
    public override string InventoryIcon => "/ui/weapons/weapon_egon.png";
    public override string InventoryIconSelected => "/ui/weapons/weapon_egon_selected.png";

    public override void Spawn()
    {
        base.Spawn();

        Model = WorldModel;
        AmmoClip = 0;
        WeaponIsAmmoAmount = 15;
    }
    public override bool CanPrimaryAttack()
    {
        return base.CanPrimaryAttack();//Input.Pressed(InputButton.PrimaryAttack);
    }
    Particles Beam;
    public override void Simulate( Client owner )
    {
        if ( !Input.Down( InputButton.PrimaryAttack ) )
        {
            if ( Beam != null )
            {
                currentsound.Stop();
                currentsound = PlaySound( "egon_off" );
                Beam.Destroy();
                Beam = null;
            }
        }
        if ( Beam != null )
        {
            if ( Time.Now > rundelay && !hasStartedrun )
            {
                hasStartedrun = true;
                currentsound.Stop();
                currentsound = PlaySound( "egon_run" );
            }
            var owner2 = Owner as HLPlayer;
            var startPos = GetFiringPos();
            var dir = GetFiringRotation().Forward;
            var tr = Trace.Ray( startPos, startPos + dir * 4096 )
            .UseHitboxes()
                .Ignore( owner2, false )
                .WithAllTags( "solid" )
                .Run();
            Beam.SetPosition( 1, tr.EndPosition );
            Beam.SetPosition( 1, tr.EndPosition );
        }

        base.Simulate( owner );
    }
    public override void AttackPrimary()
    {
        var owner = Owner as HLPlayer;
        var startPos = GetFiringPos();
        var dir = GetFiringRotation().Forward;
        var tr = Trace.Ray( startPos, startPos + dir * 4096 )
        .UseHitboxes()
            .Ignore( owner, false )
            .WithAnyTags( "solid", "npc", "player" )
            .Run();
        if ( Beam == null )
        {
            AmmoUseTime = Time.Now;
            currentsound.Stop();
            currentsound = PlaySound( "egon_windup" );
            hasStartedrun = false;
            rundelay = Time.Now + 3.935f;
            Beam = Particles.Create( "particles/egon_beam.vpcf", tr.EndPosition );
        }
        if ( Time.Now > dmgtime )
        {
            dmgtime = Time.Now + 0.1f;
            if ( tr.Entity != null && IsServer )
            {
                tr.Entity.TakeDamage( DamageInfo.Generic( 14 ).WithFlag( DamageFlags.AlwaysGib ).WithAttacker( this ) );
            }
            if ( HLGame.GameIsMultiplayer() )
            {
                // multiplayer uses 1 ammo every 1/10th second
                if ( Time.Now >= AmmoUseTime )
                {
                    owner.TakeAmmo( AmmoType, 1 );
                    AmmoUseTime = Time.Now + 0.2f;
                }
            }
            else
            {
                // single player, use 3 ammo/second
                if ( Time.Now >= AmmoUseTime )
                {
                    owner.TakeAmmo( AmmoType, 1 );
                    AmmoUseTime = Time.Now + 0.1f;
                }
            }
        }
        Beam.SetPosition( 1, tr.EndPosition );
        Beam.SetEntityAttachment( 0, EffectEntity, "muzzle", true );
        if ( Client.IsUsingVr ) Beam.SetEntityAttachment( 0, VRWeaponModel, "muzzle", true );

        Beam.SetPosition( 0, (Vector3)GetAttachment( "muzzle" )?.Position );
        Beam.SetForward( 0, GetFiringRotation().Forward );
        //var pos = tr.StartPosition;
        //var a = GetAttachment("muzzle");
        //if (a != null)
        //pos = (a ?? default).Position;
        //Beam.SetPosition(0, pos);
        Beam.SetPosition( 1, tr.EndPosition );

        base.AttackPrimary();
    }
    public override void SimulateAnimator( PawnAnimator anim )
	{
		SetHoldType( HLCombat.HoldTypes.Egon, anim );
		anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }
}
