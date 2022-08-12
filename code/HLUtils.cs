public partial class HLUtils
{
    static public HLPlayer FindPlayerInBox(Vector3 Position, int AreaSize)
    {
        BBox bbox = new BBox(new Vector3(-AreaSize, -AreaSize, -AreaSize) + Position, new Vector3(AreaSize, AreaSize, AreaSize) + Position);
        
        return Entity.FindInBox(bbox).OfType<HLPlayer>().First();
    }
}
