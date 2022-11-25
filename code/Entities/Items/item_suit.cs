[Library("item_suit"), HammerEntity]
[Model]
[Title("HEV Suit"), Category("Items"), MenuCategory( "Items" )]
public partial class Suit : ModelEntity
{
    [Flags]
		public enum Flags
		{
			ShortLogon = 1,
		}
    public override void Spawn()
    {
        base.Spawn();

        //SetModel("models/hl1/items/suit.vmdl");

		var c = Components.GetOrCreate<Movement>();

		Tags.Add("weapon");
    }

    protected Output OnPlayerTouch { get; set; }
    
    public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is not HLPlayer pl ) return;

        if (!pl.HasHEV)
        {
            pl.HasHEV = true;
            Sound.FromScreen("bell");
            Sound.FromScreen("hev_logon");
        }

        OnPlayerTouch.Fire( other );

		if (IsServer)
			Delete();
	}
}
