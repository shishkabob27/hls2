﻿[Library("env_explosion")]
[HammerEntity, Solid]
[Title("env_explosion"), Category("Effects"), Icon("explosion")]
public partial class env_explosion : Entity
{
    [Input]
    public void Explode()
    {
        HLExplosion.Explosion(this, Owner, Position, 250, 100, 24.0f, "grenade");
    }
}