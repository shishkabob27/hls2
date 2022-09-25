
public partial class HLGame : Game
{
    // TODO: CheatCmd
    [ConCmd.Admin]
    public static void setpos( float posX, float posY, float posZ, float pitch = 0, float yaw = 0, float roll = 0 )
    {
        if ( ConsoleSystem.Caller == null )
        {
            Log.Warning( "setpos: This command can only be ran by a player" );
            return;
        }

        var ent = ConsoleSystem.Caller.Pawn;
        if ( ent == null )
        {
            Log.Warning( "setpos: Player is missing a Pawn entity" );
            return;
        }

        ent.Velocity = Vector3.Zero;
        ent.Position = new Vector3( posX, posY, posZ );
        ent.EyeRotation = Rotation.From( pitch, yaw, roll );
        // TODO: Leave any vehicles? Check if stuck in world?
    }


    [ConCmd.Admin]
    public static void setang( float pitch, float yaw, float roll )
    {
        if ( ConsoleSystem.Caller == null )
        {
            Log.Warning( "setang: This command can only be ran by a player" );
            return;
        }

        if ( ConsoleSystem.Caller.Pawn == null )
        {
            Log.Warning( "setang: Player is missing a Pawn entity" );
            return;
        }

        ConsoleSystem.Caller.Pawn.EyeRotation = Rotation.From( pitch, yaw, roll );
    }

    [ConCmd.Server]
    public static void getpos()
    {
        if ( ConsoleSystem.Caller == null )
        {
            Log.Warning( "getpos: This command can only be ran by a player" );
            return;
        }

        var ent = ConsoleSystem.Caller.Pawn;
        if ( ent == null )
        {
            Log.Warning( "getpos: Player is missing a Pawn entity" );
            return;
        }

        var ang = ent.EyeRotation.Angles();
        var pos = ent.Position;

        Log.Info( $"setpos {pos.x:F2} {pos.y:F2} {pos.z:F2} {ang.pitch:F2} {ang.yaw:F2} {ang.roll:F2}" );
    }

    [ConVar.Client]
    public static int cl_showpos { get; set; } = 0;

    [Event.Frame]
    static void DrawPos()
    {
        if ( cl_showpos == 0 ) return;
        if ( !Local.Pawn.IsValid() ) return;

        DebugOverlay.ScreenText( $"pos: {Local.Pawn.Position}", Vector2.Zero.WithX( 2 ), 6, Color.White );
        DebugOverlay.ScreenText( $"ang: {Local.Pawn.Rotation.Angles()}", Vector2.Zero.WithX( 2 ), 7, Color.White );
        DebugOverlay.ScreenText( $"vel: {Local.Pawn.Velocity.Length:0.##}", Vector2.Zero.WithX( 2 ), 8, Color.White );
    }

    [ConCmd.Admin]
    private static void ent_fire( string targetName, string input, string value = null )
    {
        var ply = ConsoleSystem.Caller.Pawn;

        List<Entity> targets = new();
        if ( targetName == "!picker" )
        {
            var tr = Trace.Ray( ply.EyePosition, ply.EyePosition + ply.EyeRotation.Forward * 2500 )
            .UseHitboxes()
            .Ignore( ply )
            .Run();

            targets.Add( tr.Entity );
        }
        else
        {
            targets = EntityTarget.Default( targetName ).GetTargets( null ).ToList();
        }

        if ( !targets.Any() ) Log.Warning( $"No target entity '{targetName}' found!" );

        foreach ( var target in targets )
        {
            // TODO: debug_mapio output
            if ( !target.FireInput( input, ply, value ) )
            {
                Log.Warning( $"Failed to fire input {input}" );
            }
        }
    }

    [ConCmd.Admin]
    private static void ent_remove( string targetName = "" )
    {
        var ply = ConsoleSystem.Caller.Pawn;

        List<Entity> targets = new();
        if ( targetName == "" )
        {
            var tr = Trace.Ray( ply.EyePosition, ply.EyePosition + ply.EyeRotation.Forward * 2500 )
            .WithAnyTags( "solid", "debris" )
            .UseHitboxes()
            .Ignore( ply )
            .Run();

            if ( tr.Entity != null ) targets.Add( tr.Entity );
        }
        else
        {
            targets.Add( EntityTarget.Default( targetName ).GetTargets( null ).First() );
        }

        if ( !targets.Any() ) Log.Warning( $"No target entity '{targetName}' found!" );

        foreach ( var target in targets )
        {
            if ( target.IsWorld || target is Client ) continue;

            Log.Info( $"Removing {target.ToString()}..." );
            target.Delete();
        }
    }

    [ConCmd.Admin]
    private static void ent_remove_all( string targetName )
    {
        List<Entity> targets = EntityTarget.Default( targetName ).GetTargets( null ).ToList();

        if ( !targets.Any() ) Log.Warning( $"No target entity '{targetName}' found!" );

        foreach ( var target in targets )
        {
            if ( target.IsWorld || target is Player ) continue;

            Log.Info( $"Removing {target.ToString()}..." );
            target.Delete();
        }
    }

    private static void ToggleDebugFlag( string targetName = "", EntityDebugFlags flag = EntityDebugFlags.Text )
    {
        var ply = ConsoleSystem.Caller.Pawn;
        List<Entity> targets = new();
        if ( targetName == "" )
        {
            var tr = Trace.Ray( ply.EyePosition, ply.EyePosition + ply.EyeRotation.Forward * 2500 )
            .UseHitboxes()
            .Ignore( ply )
            .Run();

            if ( tr.Entity != null && tr.Entity is not WorldEntity ) targets.Add( tr.Entity );

            if ( !targets.Any() )
            {
                var ents = Entity.FindInSphere( tr.HitPosition, 64 );
                if ( ents.Any() ) targets.Add( ents.First() );
            }
        }
        else
        {
            targets.AddRange( EntityTarget.Default( targetName ).GetTargets( null ) );
        }

        foreach ( var target in targets )
        {
            if ( target.IsWorld ) continue;

            target.DebugFlags ^= flag;
        }
    }

    [ConCmd.Admin]
    private static void ent_text( string targetName = "" )
    {
        ToggleDebugFlag( targetName, EntityDebugFlags.Text );
    }

    [ConCmd.Admin]
    private static void ent_bbox( string targetName = "" )
    {
        ToggleDebugFlag( targetName, EntityDebugFlags.OVERLAY_BBOX_BIT );
    }

    [ConCmd.Server( "noclip", Help = "Turns on noclip mode, which makes you non solid and lets you fly around" )]
    public static void HLNoclipCommand()
    {
        if ( ConsoleSystem.Caller == null ) return;

        ( ConsoleSystem.Caller.Pawn as HLPlayer ).DoHLPlayerNoclip( ConsoleSystem.Caller );
    }
    [ConCmd.Server( "spawnScientist", Help = "Kills the calling player with generic damage" )]
    public static void SpawnScientistCommand()
    {
        var sci = new Scientist();
        sci.Position = ConsoleSystem.Caller.Pawn.Position;
        sci.Spawn();
    }

    [ConCmd.Admin( "respawn_entities" )]
    public static void RespawnEntities()
    {
        Map.Reset( DefaultCleanupFilter );
    }

    [ConCmd.Server( "ent_create" )]
    public static void SpawnEntity( string entName )
    {
        var owner = ConsoleSystem.Caller.Pawn as Player;

        if ( owner == null )
            return;

        var entityType = TypeLibrary.GetTypeByName<Entity>( entName );
        if ( entityType == null )

            if ( !TypeLibrary.Has<SpawnableAttribute>( entityType ) )
                return;

        var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 200 )
            .UseHitboxes()
            .Ignore( owner )
            .Size( 2 )
            .Run();

        var ent = TypeLibrary.Create<Entity>( entityType );
        if ( ent is BaseCarriable && owner.Inventory != null )
        {
            if ( owner.Inventory.Add( ent, true ) )
                return;
        }

        ent.Position = tr.EndPosition;
        ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) );

        //Log.Info( $"ent: {ent}" );
    }

    [ConCmd.Server( "Gib" )]
    public static void Gib()
    {
        HLCombat.CreateGibs( ConsoleSystem.Caller.Pawn.Position, ConsoleSystem.Caller.Pawn.Position, 0, new BBox( new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) ) );
    }

    [ConCmd.Server( "ply_closestto" )]
    public static void Gib( Vector3 pos )
    {
        var ply = HLUtils.ClosestPlayerTo( pos );

        Log.Info( $"Player: {ply} named {ply.Client.Name}" );
    }

    [ConCmd.Admin( "impulse" )]
    public static void Impulse( int impulse )
    {
        var caller = ConsoleSystem.Caller.Pawn as HLPlayer;

        if ( caller == null )
            return;

        if ( impulse == 101 )
        {
            caller.Inventory.Add( new Crowbar() );
            caller.Inventory.Add( new Pistol() );
            caller.Inventory.Add( new Python() );
            caller.Inventory.Add( new Shotgun() );
            caller.Inventory.Add( new SMG() );
            caller.Inventory.Add( new RPG() );
            caller.Inventory.Add( new Crossbow() );
            caller.Inventory.Add( new GrenadeWeapon() );
            caller.Inventory.Add( new TripmineWeapon() );
            caller.Inventory.Add( new Gauss() );
            caller.Inventory.Add( new Egon() );
            caller.Inventory.Add( new HornetGun() );
            caller.Inventory.Add( new SnarkWeapon() );
            caller.Inventory.Add( new SatchelWeapon() );


            caller.GiveAmmo( AmmoType.Pistol, 67 );
            caller.GiveAmmo( AmmoType.Python, 6 );
            caller.GiveAmmo( AmmoType.Buckshot, 16 );
            caller.GiveAmmo( AmmoType.Crossbow, 5 );
            caller.GiveAmmo( AmmoType.RPG, 1 );
            caller.GiveAmmo( AmmoType.Uranium, 60 );

            caller.GiveAmmo( AmmoType.Grenade, 5 );
            caller.GiveAmmo( AmmoType.SMGGrenade, 2 );
            caller.GiveAmmo( AmmoType.Satchel, 1 );

            caller.GiveAmmo( AmmoType.Tripmine, 1 );
            caller.GiveAmmo( AmmoType.Snark, 5 );

            var battery = new Battery();
            battery.Position = ConsoleSystem.Caller.Pawn.Position;
            battery.Spawn();

            var suit = new Suit();
            suit.Position = ConsoleSystem.Caller.Pawn.Position;
            suit.Spawn();
        }

        if ( impulse == 201 )
        {
            HLPlayer.Spray();
        }
    }
}
