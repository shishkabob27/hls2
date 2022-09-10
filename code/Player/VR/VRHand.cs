
public class VRHand : AnimatedEntity
{

    public bool HandType = false; // false for left, true for right.
    public override void Spawn()
    {
        SetModel(HandType ? "models/hands/alyx_hand_left.vmdl" : "models/hands/alyx_hand_right.vmdl");
    }
}