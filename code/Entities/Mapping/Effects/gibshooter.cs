[Library("gibshooter")]
[HammerEntity]
[EditorSprite("editor/gibshooter.vmat")]
[Title("gibshooter"), Category("Legacy"), Icon("dirty_lens")]
public partial class gibshooter : Entity
{
    // stub

    [Property(Title = "Number of Gibs")]
    public int m_iGibs { get; set; } = 1;

    [Property(Title = "Gib Velocity")]
    public int m_flVelocity { get; set; } = 1;

    [Property(Title = "Gib Angular Velocity")]
    public string gibanglevelocity { get; set; } = "";

    [Input]
    public void Shoot()
    {
		for ( int i = 0; i < m_iGibs; i++ )
		{
			var gib = new HLGib();
			gib.AngularVelocity = new Angles( Rand.Float( 100, 300 ), 0, Rand.Float( 100, 200 ) );

			gib.Velocity = Rotation.Forward * -1;
			gib.Velocity += new Vector3( Rand.Float( -0.25f, 0.25f ), Rand.Float( -0.25f, 0.25f ), Rand.Float( -0.25f, 0.25f ) );
			gib.Velocity = gib.Velocity * Rand.Float( 300f, 400f );

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
