using Sandbox.UI;

[UseTemplate( "/UI/Menu/SubMenus/Config/VR.html" )]
class VR : BaseMenuScreen
{
	public VR()
	{
		getCvars();
	}
	public void Done( Panel p )
	{
		updateCvars();
		Delete();
		var a = Parent.AddChild<Config>();
		BaseButtonClickDown( p, a, true, "Virtual Reality" );
	}

	public bool vrpointer { get; set; } = false;
	public int vrcrouchheight { get; set; } = 45;

	void getCvars()
	{
		vrpointer = HLGame.hl_vr_pointer;
		vrcrouchheight = HLGame.hl_vr_crouch_height;
	}

	public void updateCvars()
	{
		HLGame.hl_vr_pointer = vrpointer;
		HLGame.hl_vr_crouch_height = vrcrouchheight;

		ConsoleSystem.Run( "hl_vr_pointer " + (vrpointer ? 1 : 0) );
		ConsoleSystem.Run( "hl_vr_crouch_height " + vrcrouchheight );
		updtasync();
	}
	public async void updtasync()
	{
		await GameTask.DelaySeconds( 0.1f );
		ConsoleSystem.Run( "hl_savecvar" );
	}
}
