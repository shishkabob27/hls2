namespace Sandbox.UI
{
	[Library( "logo" )]
	public class Logo : Panel
	{
		public string logost = "/ui/menu/logo/";
		int curframe = 1;
		int maxframes = 110;
		float FPS = 24;
		TimeSince lastFrame;
		public Logo()
		{
		}
		public override void Tick()
		{
			base.Tick();
			frame();
		}
		void frame()
		{
			if ( lastFrame <= 1f / FPS ) return;
			lastFrame = 0;
			Style.SetBackgroundImage( $"{logost}{curframe}.png" );
			curframe++;
			if ( curframe > maxframes ) curframe = 1;
		}
	}
}
