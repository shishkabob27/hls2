[Library( "weapon_snark" ), HammerEntity]
[EditorModel( "models/hl1/weapons/world/sqknest.vmdl" )]
[Title( "Snark" ), Category( "Weapons" )]
partial class SnarkWeapon : HLWeapon
{
    //stub
    public static readonly Model WorldModel = Model.Load( "models/hl1/weapons/world/sqknest.vmdl" );
    public override string ViewModelPath => "models/hl1/weapons/view/v_squeak.vmdl";
	public override bool HasHDModel => true;
    public override float PrimaryRate => 0.3f;
    public override int Bucket => 4;
    public override int BucketWeight => 4;
    public override AmmoType AmmoType => AmmoType.Snark;
    public override string AmmoIcon => "ui/ammo11.png";
    public override string InventoryIcon => "/ui/weapons/weapon_snark.png";
    public override string InventoryIconSelected => "/ui/weapons/weapon_snark_selected.png";
    public override int ClipSize => -1;

    public override void Spawn()
    {
        base.Spawn();

        Model = WorldModel;
        AmmoClip = 0;
        WeaponIsAmmoAmount = 5;
        WeaponIsAmmo = true;
    }
    public override void AttackPrimary()
    {
        TimeSincePrimaryAttack = 0;
        TimeSinceSecondaryAttack = 0;


        if ( Owner is not HLPlayer player ) return;

        var owner = Owner as HLPlayer;

        if ( owner.TakeAmmo( AmmoType, 1 ) == 0 )
        {
            return;
        }

        // woosh sound
        // screen shake

        PlaySound( "dm.grenade_throw" );

        Rand.SetSeed( Time.Tick );


        if ( IsServer )
            using ( Prediction.Off() )
            {
                var snark = new Snark
                {
                    Position = GetFiringPos() + GetFiringRotation().Forward * 53.0f,
                    Owner = Owner
                };

                snark.Velocity = GetFiringRotation().Forward * 300.0f + GetFiringRotation().Up * 200.0f + Owner.Velocity;

                // This is fucked in the head, lets sort this this year
                Tags.Add( "debris" );

                //grenade.CollisionGroup = CollisionGroup.Debris;
                //grenade.SetInteractsExclude( CollisionLayer.Player );
                //grenade.SetInteractsAs( CollisionLayer.Debris );

            }

        player.SetAnimParameter( "b_attack", true );

        player.SetAnimParameter( "attack", true );

        if ( IsClient )
            ViewModelEntity.SetAnimParameter( "attack", true );

        if ( IsServer && player.AmmoCount( AmmoType.Snark ) == 0 )
        {

            player.SwitchToBestWeapon();
        }

    }

    public override void SimulateAnimator( PawnAnimator anim )
    {
        anim.SetAnimParameter( "holdtype", (int)HLCombat.HoldTypes.Squeak ); // TODO this is shit
        anim.SetAnimParameter( "aim_body_weight", 1.0f );
    }
}
