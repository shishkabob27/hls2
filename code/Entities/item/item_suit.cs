[Library("item_suit"), HammerEntity]
[EditorModel("models/hl1/items/suit.vmdl")]
[Title("HEV Suit"), Category("Items")]
internal class Suit : ModelEntity
{
    [Flags]
		public enum Flags
		{
			ShortLogon = 1,
		}
    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/hl1/items/suit.vmdl");

        PhysicsEnabled = true;
		UsePhysicsCollision = true;

        Tags.Add("weapon");
    }

    public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not HLPlayer pl ) return;

        pl.HasHEV = true;

        Sound.FromScreen("bell");
        Sound.FromScreen("hev_logon");

		if (IsServer)
			Delete();
	}
}
