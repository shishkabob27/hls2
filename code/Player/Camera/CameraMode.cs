public class CameraMode : BaseNetworkable
{
	public Vector3 Position
	{
		get => Camera.Position;
		set => Camera.Position = value;
	}
	public Rotation Rotation
	{
		get => Camera.Rotation;
		set => Camera.Rotation = value;
	}
	public virtual void Update()
	{

	}
}
