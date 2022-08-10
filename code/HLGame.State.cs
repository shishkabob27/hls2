


partial class HLGame : Game
{
	public static GameStates CurrentState => (Current as HLGame)?.GameState ?? GameStates.Live;

	[Net]
	public RealTimeUntil StateTimer { get; set; } = 0f;

	[Net]
	public GameStates GameState { get; set; } = GameStates.Live;
	[Net]
	public string NextMap { get; set; } = "facepunch.datacore";

	[ConCmd.Admin]
	public static void SkipStage()
	{
		if ( Current is not HLGame dmg ) return;

		dmg.StateTimer = 2;
	}

	private async Task WaitStateTimer()
	{
		while ( StateTimer > 0 )
		{
			await Task.DelayRealtimeSeconds( 1.0f );
		}

		// extra second for fun
		await Task.DelayRealtimeSeconds( 1.0f );
	}

	private async Task GameLoopAsync()
	{
		GameState = GameStates.Warmup;
		StateTimer = 0;
		await WaitStateTimer();

		GameState = GameStates.Live;
		StateTimer = 99999 * 9999;
		FreshStart();
		await WaitStateTimer();

		GameState = GameStates.GameEnd;
		StateTimer = 10.0f;
		await WaitStateTimer();

		GameState = GameStates.MapVote;
		await WaitStateTimer();

	}

	private bool HasEnoughPlayers()
	{
		if ( All.OfType<HLPlayer>().Count() < 2 )
			return false;

		return true;
	}

	private void FreshStart()
	{
		foreach ( var cl in Client.All )
		{
			cl.SetInt( "kills", 0 );
			cl.SetInt( "deaths", 0 );
		}

		All.OfType<HLPlayer>().ToList().ForEach( x =>
		{
			x.Respawn();
		} );
	}

	public enum GameStates
	{
		Warmup,
		Live,
		GameEnd,
		MapVote
	}

}
