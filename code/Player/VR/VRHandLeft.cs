public class VRHandLeft : VRHand
{
    public override Input.VrHand InputHand => Input.VR.LeftHand;
    public override void Spawn()
    {
        SetModel("models/vr/hand_left.vmdl");
		Transmit = TransmitType.Always;
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
        Transform = Input.VR.LeftHand.Transform;
    }
    public override void FrameSimulate(IClient cl)
    {
        base.FrameSimulate(cl);
        Transform = Input.VR.LeftHand.Transform;
    }
}
