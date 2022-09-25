[Library( "monster_headcrab" ), HammerEntity]
[EditorModel( "models/hl1/monster/headcrab.vmdl" )]
[Title( "Headcrab" ), Category( "Monsters" ), Icon( "person" )]
public class Headcrab : NPC
{
    public override void Spawn()
    {
        base.Spawn();
        Health = 20;
        SetModel( "models/hl1/monster/headcrab.vmdl" );
        SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -12, -12, 0 ), new Vector3( 12, 12, 24 ) );
        entFOV = 0.5f;
        EnableHitboxes = true;
        Tags.Add( "npc", "playerclip" );

    }
    public override int Classify()
    {
        return (int)HLCombat.Class.CLASS_ALIEN_PREY;
    }
}