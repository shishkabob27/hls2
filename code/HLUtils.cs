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
        var plys = Game.Clients.OrderBy(o => (o.Pawn.Position.Distance(pos)));
        return plys.First().Pawn as HLPlayer;
    }
    
    static public bool PlayerInRangeOf(Vector3 pos, float range = 1024)
    {
		var plys = Game.Clients.Where( ply => ply.Pawn.Position.Distance( pos ) < range );
		return plys.Count() > 0;
		//return true;
		//return false;
    }
    static public int AmountOf<T>()
    {
        var ent = Entity.All.OfType<T>();
        return ent.Count();
    }

    static public float CorrectPitch(float pitch, bool mult = true)
    {
        if (mult) return(float)Math.Sqrt(pitch / 100);
        return (float)Math.Sqrt(pitch);
    }

    static public Vector3 GetCentreOf(Entity ent)
    {
        if (ent is ModelEntity) return (ent as ModelEntity).CollisionWorldSpaceCenter;
        return ent.WorldSpaceBounds.Center;
    }

    static public float VecToYaw(Vector3 vec)
    {
        var yaw = (int)(Math.Atan2(vec[1], vec[0]) * 180 / Math.PI);
        if (yaw < 0)
            yaw += 360;
        return yaw;
    }

}
public class MenuCategory : Attribute
{
	public string Value { get; set; }

	public MenuCategory( string value )
	{
		Value = value;
	}
}
