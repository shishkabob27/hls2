
public class VRHandLeft : VRHand
{
    public override void Spawn()
    {
        Predictable = true;
        SetModel("models/vr/hand_left.vmdl");
		Transmit = TransmitType.Always;
    }
    public override void Simulate(Client cl)
    {
        base.Simulate(cl);
        Transform = Input.VR.LeftHand.Transform;
    }
}