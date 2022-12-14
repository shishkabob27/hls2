partial class HLViewModel : BaseViewModel
{

	public override void PlaceViewmodel()
	{

	}

	public void UpdateCamera()
	{
		var rotationDistance = Rotation.Distance( Camera.Rotation );

		Position = Camera.Position;
		Rotation = Rotation.Lerp( Rotation, Camera.Rotation, RealTime.Delta * rotationDistance * 1.1f );

		Camera.ZNear = 4;
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( 90f );
	}
}
