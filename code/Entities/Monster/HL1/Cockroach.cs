[Library( "monster_cockroach" ), HammerEntity]
[EditorModel( "models/hl1/monster/roach.vmdl" )]
[Title( "Cockroach" ), Category( "Monsters" ), Icon( "person" )]
public class Cockroach : NPC
{

    //ROACH_IDLE
    //ROACH_BORED
    //ROACH_SCARED_BY_ENT
    //ROACH_SCARED_BY_LIGHT
    //ROACH_SMELL_FOOD
    //ROACH_EAT	
    string MODE = "ROACH_IDLE";

    public override void Spawn(){
        base.Spawn();
        Health = 1;
        SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3 ( -1, -1, 0 ), new Vector3( 1, 1, 2 ));
        SetModel( "models/hl1/monster/roach.vmdl" );
        entFOV = 0.2f;
        EnableHitboxes = true;
        Tags.Add( "npc", "playerclip" );
    }

    public override int Classify()
    {
        return (int)HLCombat.Class.CLASS_INSECT;
    }

    public override void Touch( Entity other ){
        base.Touch( other );
        if ( other is HLPlayer && IsServer )
        {
            Health = 0;
        }
    }

    public override void Think()
	{

	}
}