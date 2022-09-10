
public class VRHandRight : VRHand
{
    public override void Spawn()
    {
        Predictable = true;
        SetModel("models/vr/hand_right.vmdl");
		Transmit = TransmitType.Always;
    }
    public override void Simulate(Client cl)
    {
        base.Simulate(cl);
        Transform = Input.VR.RightHand.Transform;
    }
}