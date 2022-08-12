// https://github.com/ValveSoftware/halflife/blob/master/dlls/combat.cpp

public static class HLCombat
{
    public static void CreateGibs(Vector3 Position, DamageInfo dmgInfo)
    {
        Vector3 attackDir = (dmgInfo.Position - new Vector3(0, 0, 10) - Position).Normal;
        
        var skullGib = new HLGib();
        skullGib.AngularVelocity = new Angles(Rand.Float(100, 300), 0, Rand.Float(100, 200));

        skullGib.Velocity = attackDir * -1;
        skullGib.Velocity += new Vector3(Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f));
        skullGib.Velocity = skullGib.Velocity * Rand.Float(300f, 400f);
        
        skullGib.Position = Position;
        skullGib.Rotation = Rotation.LookAt(Vector3.Random.Normal);
        skullGib.Spawn("models/hl1/gib/hgib/hgib_skull1.vmdl");
        for (int i = 0; i < 4; i++)
        {
            var gib = new HLGib();
            gib.AngularVelocity = new Angles(Rand.Float(100, 300), 0, Rand.Float(100, 200));
            
            gib.Velocity = attackDir * -1;
            gib.Velocity += new Vector3(Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f), Rand.Float(-0.25f, 0.25f));
            gib.Velocity = gib.Velocity * Rand.Float(300f, 400f);
            
            gib.Position = Position;
            gib.Rotation = Rotation.LookAt(Vector3.Random.Normal);
            gib.Spawn();
        }
    }
}
