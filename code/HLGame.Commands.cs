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
        
		[ConCmd.Admin("respawn_entities")]
		public static void RespawnEntities()
		{
			Map.Reset(DefaultCleanupFilter);
		}

		[ConCmd.Server("ent_create")]
		public static void SpawnEntity(string entName)
		{
			var owner = ConsoleSystem.Caller.Pawn as Player;

			if (owner == null)
				return;

			var entityType = TypeLibrary.GetTypeByName<Entity>(entName);
			if (entityType == null)

				if (!TypeLibrary.Has<SpawnableAttribute>(entityType))
					return;

			var tr = Trace.Ray(owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 200)
				.UseHitboxes()
				.Ignore(owner)
				.Size(2)
				.Run();

			var ent = TypeLibrary.Create<Entity>(entityType);
			if (ent is BaseCarriable && owner.Inventory != null)
			{
				if (owner.Inventory.Add(ent, true))
					return;
			}

			ent.Position = tr.EndPosition;
			ent.Rotation = Rotation.From(new Angles(0, owner.EyeRotation.Angles().yaw, 0));

			//Log.Info( $"ent: {ent}" );
		}
		[ConCmd.Server("Gib")]
		public static void Gib()
        {
            HLCombat.CreateGibs(ConsoleSystem.Caller.Pawn.Position, ConsoleSystem.Caller.Pawn.Position, 0);
        }
	}
}
