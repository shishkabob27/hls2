// https://github.com/ValveSoftware/halflife/blob/master/dlls/combat.cpp

public partial class HLCombat
{
    public static void CreateGibs(Vector3 Position, Vector3 DMGPos, float Health, BBox bbox)
    {
        Sound.FromWorld("bodysplat", Position);
        
        Vector3 attackDir = (DMGPos - new Vector3(0, 0, 10) - Position).Normal;
        CreateHeadGib(Position, DMGPos, Health);
       
        for (int i = 0; i < 4; i++)
        {
            var gib = new HLGib();
            gib.AngularVelocity = new Angles(Rand.Float(100, 300), 0, Rand.Float(100, 200));
            
            gib.Velocity = attackDir * -1;
            gib.Velocity += new Vector3(Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f));
            gib.Velocity = gib.Velocity * Rand.Float(300f, 400f);
            
            if (Health > -50)
            {
                gib.Velocity = gib.Velocity * 0.7f;
            }
            else if (Health > -200)
            {
                gib.Velocity = gib.Velocity * 2;
            }
            else
            {
                gib.Velocity = gib.Velocity * 4;
            }
            
            gib.Position = bbox.RandomPointInside + Position -bbox.Mins;
            gib.Rotation = Rotation.LookAt(Vector3.Random.Normal);
            gib.Spawn();
        }
    }

    static void CreateHeadGib(Vector3 Position, Vector3 DMGPos, float Health)
    {
        Vector3 attackDir = (DMGPos - new Vector3(0, 0, 10) - Position).Normal;
        var skullGib = new HLGib();

        skullGib.Position = Position;
        skullGib.Rotation = Rotation.LookAt(Vector3.Random.Normal);

        var player = HLUtils.FindPlayerInBox(Position, 2048);
        
        // 5% chance head will be thrown at player's face.
        if (player is HLPlayer && Rand.Float(0, 100) <= 5)
        {
            skullGib.Velocity = (((player as HLPlayer).CollisionWorldSpaceCenter + new Vector3(0, 0, 72)) - skullGib.CollisionWorldSpaceCenter).Normal * 500;
            skullGib.Velocity = skullGib.Velocity.WithZ(skullGib.Velocity.z + 100);
        }
        else
        {

            //skullGib.Velocity = attackDir * -1;
            //skullGib.Velocity += new Vector3(Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f));
            //skullGib.Velocity = skullGib.Velocity * Rand.Float(300f, 400f);
            skullGib.Velocity = new Vector3(Rand.Float(-100, 100), Rand.Float(-100, 100), Rand.Float(200, 300));
        }
        if (Health > -50)
        {
            skullGib.Velocity = skullGib.Velocity * 0.7f;
        }
        else if (Health > -200)
        {
            skullGib.Velocity = skullGib.Velocity * 2;
        }
        else
        {
            skullGib.Velocity = skullGib.Velocity * 4;
        }
        
        skullGib.AngularVelocity = new Angles(Rand.Float(100, 300), 0, Rand.Float(100, 200));

        skullGib.Spawn("models/hl1/gib/hgib/hgib_skull1.vmdl");
    }
}
