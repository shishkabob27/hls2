[Library("env_shooter")]
[HammerEntity]
[EditorSprite("editor/env_shooter.vmat")]
[Title("env_shooter"), Category( "Effects" ), Icon("dirty_lens")]
public partial class env_shooter : Entity
{
    // stub

    [Property(Title = "Number of Gibs")]
    public int m_iGibs { get; set; } = 1;

    [Property(Title = "Gib Velocity")]
    public int m_flVelocity { get; set; } = 1;

    [Property(Title = "Gib Variance")]
    public int m_flVariance { get; set; } = 0;

    [Input]
    public void Shoot()
    {
		for ( int i = 0; i < m_iGibs; i++ )
		{
			var gib = new HLGib();
			gib.AngularVelocity = new Angles( Game.Random.Float( 100, 300 ), 0, Game.Random.Float( 100, 200 ) );
			 
			gib.Velocity = (Rotation.Forward + (Vector3.Random * m_flVariance)) * m_flVelocity;

			if ( Health > -50 )
			{
				gib.Velocity = gib.Velocity * 0.7f;
			}
			else if ( Health > -200 )
			{
				gib.Velocity = gib.Velocity * 2;
			}
			else
			{
				gib.Velocity = gib.Velocity * 4;
			}

			gib.Position = Position;
			gib.Rotation = Rotation.LookAt( Vector3.Random.Normal );

			gib.Spawn( 0 );
		}
	}
}
