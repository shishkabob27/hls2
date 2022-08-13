[Library("item_suit"), HammerEntity]
[EditorModel("models/hl1/items/suit.vmdl")]
[Title("HEV Suit"), Category("Items")]
internal class Suit : ModelEntity
{
    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/hl1/items/suit.vmdl");
    }
}
