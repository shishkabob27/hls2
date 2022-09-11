public class VRHand : AnimatedEntity
{

    public bool HandType = false; // false for left, true for right.

    public override void Spawn()
    {
        SetModel(HandType ? "models/vr/hand_left.vmdl" : "models/vr/hand_right.vmdl");
    }
}