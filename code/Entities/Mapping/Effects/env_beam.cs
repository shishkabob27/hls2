[Library( "env_beam" )]
[HammerEntity]
[Title( "env_beam" ), Category( "Legacy" ), Icon( "volume_up" )]
public partial class env_beam : Entity
{
    [Flags]
    public enum Flags
    {
        StartOn = 1,
        Toggle = 2,
        RandomStrike = 4,
        Ring = 8,
        StartSparks = 16,
        EndSparks = 32,
        DecalEnd = 64,
        ShadeStart = 128,
        ShadeEnd = 256,
        TaperOut = 512,
    }

    /// <summary>
    /// Settings that are only applicable when the entity spawns
    /// </summary>
    [Property( "spawnflags", Title = "Spawn Settings" )]
    public Flags spawnflags { get; set; } = Flags.StartOn;



    [Property( "LightningStart" ), FGDType( "target_destination" )]
    public string LightningStart { get; set; } = "";
    [Property( "LightningEnd" ), FGDType( "target_destination" )]
    public string LightningEnd { get; set; } = "";
    [Property]
    public string texture { get; set; } = "";
    [Property]
    public float NoiseAmplitude { get; set; } = 0;
    [Property]
    public float BoltWidth { get; set; } = 2;
    [Property]
    Vector3 rendercolor { get; set; }
    [Property]
    Vector3 targetpoint { get; set; }
    Particles Beam;
    // stub

    public override void Spawn()
    {
        base.Spawn();
        if ( spawnflags.HasFlag( Flags.StartOn ) )
        {
            TurnOn();
        }
    }

    [Input]
    void TurnOn()
    {
        if ( true )//Beam == null)
        {
            Beam = Particles.Create( "particles/env_beam.vpcf", Position );
        }
        UpdateBeam();
        Beam.SetPosition( 2, rendercolor );
        Beam.SetPosition( 3, new Vector3( BoltWidth, 1, 0 ) );
        Beam.SetPosition( 4, new Vector3( NoiseAmplitude, 0, 0 ) );
    }
    [Input]
    void TurnOff()
    {
        rpcremove();
    }
    [ClientRpc]
    void rpcremove()
    {
        if ( Beam != null )
        {
            Beam.Destroy();
            Beam = null;
        }
    }
    [Input]
    void Toggle()
    {
        if ( Beam == null )
        {
            TurnOn();
        }
        else
        {
            Beam.Destroy();
        }
    }
    void UpdateBeam()
    {
        if ( Beam != null )
        {
            try
            {
                var c = Entity.FindAllByName( LightningStart ).First();
                var d = Entity.FindAllByName( LightningEnd ).First();
                Beam.SetEntity( 0, c );
                Beam.SetEntity( 1, d );
            }
            catch
            {

            }
            var a = Position;
            var b = targetpoint;

            try
            {
                a = Entity.FindAllByName( LightningStart ).First().Position;
                b = Entity.FindAllByName( LightningEnd ).First().Position;
            }
            catch
            {

            }
            Beam.SetPosition( 0, a );
            Beam.SetPosition( 1, b );
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        rpcremove();
    }
}