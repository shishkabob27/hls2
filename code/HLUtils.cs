public partial class HLUtils
{
    static public HLPlayer FindPlayerInBox(Vector3 Position, int AreaSize)
    {
        BBox bbox = new BBox(new Vector3(-AreaSize, -AreaSize, -AreaSize) + Position, new Vector3(AreaSize, AreaSize, AreaSize) + Position);
        if (Entity.FindInBox(bbox).OfType<HLPlayer>().Count() > 0)
            return Entity.FindInBox(bbox).OfType<HLPlayer>().First();
        else
            return null;
    }

    static public bool IsPlayerInBox(Vector3 Position, int AreaSize)
    {
        BBox bbox = new BBox(new Vector3(-AreaSize, -AreaSize, -AreaSize) + Position, new Vector3(AreaSize, AreaSize, AreaSize) + Position);
        
        return Entity.FindInBox(bbox).OfType<HLPlayer>().Count() > 0;
    }
    
    static public HLPlayer ClosestPlayerTo(Vector3 pos)
    {
        var plys = Entity.All.OfType<HLPlayer>().ToList().OrderBy(o => (o.Position.Distance(pos)));
        return plys.First();
    }
    
    static public bool PlayerInRangeOf(Vector3 pos, float range = 1024)
    {
        var plys = Entity.All.OfType<HLPlayer>().ToList();
        plys.RemoveAll(ply => ply.Position.Distance(pos) > range);
        return plys.Count() > 0;
    }
    static public int AmountOf<T>()
    {
        var ent = Entity.All.OfType<T>().ToList();
        return ent.Count();
    }

    static public float CorrectPitch(float pitch)
    {
        return (float)Math.Sqrt(pitch / 100);
    }
}
