[Library( "monster_houndeye" ), HammerEntity]
[EditorModel( "models/hl1/monster/houndeye.vmdl" )]
[Title( "Houndeye" ), Category( "Monsters" ), Icon( "person" )]
public class Houndeye : NPC
{
    public override void Spawn()
    {
        base.Spawn();
        Health = 20;
        SetModel( "models/hl1/monster/houndeye.vmdl" );
        SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 36 ) );
        entFOV = 0.5f;
        EnableHitboxes = true;
        Tags.Add( "npc", "playerclip" );

    }
    public override int Classify()
    {
        return (int)HLCombat.Class.CLASS_ALIEN_MONSTER;
    }
}