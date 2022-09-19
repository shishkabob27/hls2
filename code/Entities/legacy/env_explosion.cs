[Library("env_explosion")]
[HammerEntity]
[EditorSprite("editor/env_explosion.vmat")]
[Title("env_explosion"), Category("Effects"), Icon("mode_heat")]
public partial class env_explosion : Entity
{
	[Input]
    public void Explode()
    {
        HLExplosion.Explosion(this, Owner, Position, 250, 100, 24.0f, "grenade");
    }
}