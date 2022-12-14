partial class HLViewModel : BaseViewModel
{

	public override void PlaceViewmodel()
	{

	}

	public void UpdateCamera()
	{
		Position = Camera.Position;
		Rotation = Camera.Rotation;

		Camera.ZNear = 4;
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 90f );
	}
}
