[Library("weapon_egon"), HammerEntity]
[EditorModel("models/hl1/weapons/world/egon.vmdl")]
[Title("Egon"), Category("Weapons")]
partial class Egon : HLWeapon
{
    //stub

    public override string ViewModelPath => "models/hl1/weapons/view/v_egon.vmdl";

    public override int Bucket => 3;
    public override int BucketWeight => 3;
    public override AmmoType AmmoType => AmmoType.Uranium;
    public override string AmmoIcon => "ui/ammo7.png";

    public override bool CanPrimaryAttack()
    {
        return base.CanPrimaryAttack();//Input.Pressed(InputButton.PrimaryAttack);
    }
    Particles Beam;
    public override void Simulate(Client owner)
    {
        if (!Input.Down(InputButton.PrimaryAttack))
        {
            if (Beam != null)
            {
                Beam.Destroy();
                Beam = null;
            }
        }
        if (Beam != null)
        {

            var owner2 = Owner as HLPlayer;
            var startPos = GetFiringPos();
            var dir = GetFiringRotation().Forward;
            var tr = Trace.Ray(startPos, startPos + dir * 800)
            .UseHitboxes()
                .Ignore(owner2, false)
                .WithAllTags("solid")
                .Run();
            Beam.SetPosition(1, tr.EndPosition);
            Beam.SetPosition(1, tr.EndPosition);
        }

        base.Simulate(owner);
    }
    public override void AttackPrimary()
    {

        var owner = Owner as HLPlayer;
        var startPos = GetFiringPos();
        var dir = GetFiringRotation().Forward;
        var tr = Trace.Ray(startPos, startPos + dir * 800)
        .UseHitboxes()
            .Ignore(owner, false)
            .WithAllTags("solid")
            .Run();
        if (Beam == null)
        {
            Beam = Particles.Create("particles/egon_beam.vpcf", tr.EndPosition);
        }

        Beam.SetPosition(1, tr.EndPosition);
        Beam.SetEntityAttachment(0, EffectEntity, "muzzle", true);
        if (Client.IsUsingVr) Beam.SetEntityAttachment(0, VRWeaponModel, "muzzle", true);

        Beam.SetPosition(0, (Vector3)GetAttachment("muzzle")?.Position);
        Beam.SetForward(0, GetFiringRotation().Forward); 
        //var pos = tr.StartPosition;
        //var a = GetAttachment("muzzle");
        //if (a != null)
        //pos = (a ?? default).Position;
        //Beam.SetPosition(0, pos);
        Beam.SetPosition(1, tr.EndPosition);
        Particles.Create("particles/gauss_impact.vpcf", tr.EndPosition);

        base.AttackPrimary();
    }

}