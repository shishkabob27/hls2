namespace Sandbox
{
	public partial class HLGame : Game
	{
		[ConCmd.Server( "spawnScientist", Help = "Kills the calling player with generic damage" )]
		public static void SpawnScientistCommand()
		{
			var sci = new Scientist();
            sci.Position = ConsoleSystem.Caller.Pawn.Position;
			sci.Spawn();
        }
	}
}
