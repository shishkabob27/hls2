[Library( "monster_bullchicken" ), HammerEntity]
[EditorModel( "models/hl1/monster/bullchicken.vmdl" )]
[Title( "Bullsquid" ), Category( "Monsters" ), Icon( "person" ), MenuCategory( "Aliens" )]
public class Bullsquid : NPC
{
    public override void Spawn()
    {
        base.Spawn();
        Health = 20;
        SetModel( "models/hl1/monster/bullsquid.vmdl" );
        SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -32, -32, 0 ), new Vector3( 32, 32, 64 ) );
        entFOV = 0.2f;
        EnableHitboxes = true;
        Tags.Add( "npc", "playerclip" );

    }
    public override int Classify()
    {
        return (int)HLCombat.Class.CLASS_ALIEN_PREDATOR;
    }
}
