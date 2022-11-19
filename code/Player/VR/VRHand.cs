public class VRHand : AnimatedEntity
{

    public bool HandType = false; // false for left, true for right.

    public virtual Input.VrHand InputHand { get; }

    public override void Spawn()
    {
        SetModel(HandType ? "models/vr/v_hand_hevsuit/v_hand_hevsuit_left.vmdl" : "models/vr/v_hand_hevsuit/v_hand_hevsuit_right.vmdl" );
    }

    public override void Simulate(Client cl)
    {
        base.Simulate(cl);

        Animate();
    }

    private void Animate()
    {
        SetAnimParameter("Thumb", InputHand.GetFingerCurl(0));
        SetAnimParameter("Index", InputHand.GetFingerCurl(1));
        SetAnimParameter("Middle", InputHand.GetFingerCurl(2));
        SetAnimParameter("Ring", InputHand.GetFingerCurl(3));
        SetAnimParameter("Pinky", InputHand.GetFingerCurl(4));
    }
}
