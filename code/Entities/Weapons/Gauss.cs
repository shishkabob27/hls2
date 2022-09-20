[Library("weapon_gauss"), HammerEntity]
[EditorModel("models/hl1/weapons/world/gauss.vmdl")]
[Title("Gauss"), Category("Weapons")]
partial class Gauss : HLWeapon
{
    //stub

    public static readonly Model WorldModel = Model.Load("models/hl1/weapons/world/gauss.vmdl");
    public override string ViewModelPath => "models/hl1/weapons/view/v_gauss.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 2;
    public override AmmoType AmmoType => AmmoType.Uranium;
    public override string AmmoIcon => "ui/ammo7.png";
    public override string InventoryIcon => "/ui/weapons/weapon_gauss.png";
    public override float ReloadTime => 0.1f;
    public override int ClipSize => 1;
    static Vector3 orangeCOLOUR = new Vector3(255, 128, 0);
    static Vector3 whiteCOLOUR = new Vector3(255, 255, 255);
    Nullable<Sound> spinSound;
    bool spinning = false;
    float spintime = 0.0f;

    public override void Spawn()
    {
        base.Spawn();

        Model = WorldModel;
        AmmoClip = 0;
    }

    Particles Beam;
    public override void Simulate(Client owner)
    {
        if (spinSound == null)
        {
            spinSound = Sound.FromEntity("pulsemachine", this).SetVolume(0);
        } else
        {
            spinSound?.SetVolume(spinning ? 1 : 0);
            spinSound?.SetPitch(((spintime + 1.05f)/2).Clamp(1.1f,2.50f));
        }
        base.Simulate(owner);
        if (Owner is not HLPlayer player) return;

        var owner2 = Owner as HLPlayer;

        if ((!(Input.Down(InputButton.SecondaryAttack)) && spinning))
        {
            ViewModelEntity?.SetAnimParameter("spinning", false);
            ShootEffects(whiteCOLOUR);
            var x = 85 + Rand.Float(0, 31);
            PlaySound("gauss").SetPitch(HLUtils.CorrectPitch(x));
            ShootBullet(0, 1, 15 * spintime, 2.0f);
            ViewModelEntity?.SetAnimParameter("fire", true);
            spinning = false;
        }
    }
    public override void AttackPrimary()
    {
        TimeSincePrimaryAttack = 0;

        if (Owner is not HLPlayer player) return;

        var owner = Owner as HLPlayer;
        if (owner.TakeAmmo(AmmoType, 2) == 0)
        {
            return;
        }
        var x = 85 + Rand.Float(0, 31);
        PlaySound("gauss").SetPitch(HLUtils.CorrectPitch(x));

        ShootEffects(orangeCOLOUR);
        ShootBullet(0, 1, 15, 2.0f);

    }
    protected void ShootEffects(Vector3 beamcolour)
    {
        if (Owner is not HLPlayer player) return;

        var owner = Owner as HLPlayer;
        var startPos = GetFiringPos();
        var dir = GetFiringRotation().Forward;

        var tr = Trace.Ray(startPos, startPos + dir * 800)
        .UseHitboxes()
            .Ignore(owner, false)
            .WithAllTags("solid")
            .Run();

        if (true)//Beam == null)
        {
            Beam = Particles.Create("particles/generic_beam.vpcf", tr.EndPosition);
        }

        ViewModelEntity?.SetAnimParameter("fire", true);
        VRWeaponModel?.SetAnimParameter("fire", true);
        ViewModelEntity?.SetAnimParameter("holdtype_attack", false ? 2 : 1);
        VRWeaponModel?.SetAnimParameter("holdtype_attack", false ? 2 : 1);

        Beam.SetEntityAttachment(0, EffectEntity, "muzzle", true);
        if (Client.IsUsingVr) Beam.SetEntityAttachment(0, VRWeaponModel, "muzzle", true);
        Beam.SetPosition(2, beamcolour);
        Beam.SetPosition(3, new Vector3(2, 1, 0));

        //Beam.SetPosition(0, Position);
        //var pos = tr.StartPosition;
        //var a = GetAttachment("muzzle");
        //if (a != null)
        //pos = (a ?? default).Position;
        //Beam.SetPosition(0, pos);
        Beam.SetPosition(1, tr.EndPosition);
        Beam.Destroy();

        Particles.Create("particles/gauss_impact.vpcf", tr.EndPosition);
    }

    int tickammouse = 0;
    public override void AttackSecondary()
    {

        if (!spinning)
        {
            tickammouse = 5;
            spintime = 0;
        }

        base.AttackSecondary();
        TimeSinceSecondaryAttack = 0;
        if (Owner is not HLPlayer player) return;

        var owner = Owner as HLPlayer;
        tickammouse += 1;

        if (tickammouse >= 5 && spintime < 10)
        {
            tickammouse = 0;
            if (owner.TakeAmmo(AmmoType, 1) == 0)
            {
                return;
            }  
        }

        spinning = true;

        ViewModelEntity?.SetAnimParameter("spinning", true);
        if (spintime >= 10)
            return;

        spintime += 0.1f;
        //charge attack here!
        
    }
}