public class VRHandRight : VRHand
{
    public override Input.VrHand InputHand => Input.VR.RightHand;
    public override void Spawn()
    {
        SetModel("models/vr/hand_right.vmdl");
		Transmit = TransmitType.Always;
    }

    public override void Simulate(Client cl)
    {
        base.Simulate(cl);
        Transform = Input.VR.RightHand.Transform;
    }
    public override void FrameSimulate(Client cl)
    {
        base.FrameSimulate(cl);
        Transform = Input.VR.RightHand.Transform;
    }
}