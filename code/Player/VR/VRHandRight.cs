public class VRHandRight : VRHand
{
    public override Input.VrHand InputHand => Input.VR.RightHand;
    public override void Spawn()
    {
        SetModel( "models/vr/v_hand_hevsuit/v_hand_hevsuit_right.vmdl" );
		Transmit = TransmitType.Always;
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
        Transform = Input.VR.RightHand.Transform;
    }
    public override void FrameSimulate(IClient cl)
    {
        base.FrameSimulate(cl);
        Transform = Input.VR.RightHand.Transform;
    }
}
