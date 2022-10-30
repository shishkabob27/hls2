public class BaseGamemodeStub : Entity
{
    public bool SpawnCheck()
    {
        var b = Entity.All.OfType<Weapon>().ToList();
        b.RemoveAll( x => ( x as Entity ).Tags.Has( "stubmade" ) );
        b.RemoveAll( x => ( x as Entity ).Owner is HLPlayer );
        if ( b.Count() > 2 ) // If we find any of our base entities from this gamemode we should abort.
        {
            Delete();
            return true;
        }
        return false;
    }
    public void SpawnRandomWeapon()
    {
        var rand = Rand.Int( 1, 14 );
        Entity a;
        switch ( rand )
        {
            case 1:
                a = new Crowbar();
                break;
            case 2:
                a = new Pistol();
                break;
            case 3:
                a = new Python();
                break;
            case 4:
                a = new SMG();
                break;
            case 5:
                a = new Shotgun();
                break;
            case 6:
                a = new Crossbow();
                break;
            case 7:
                a = new RPG();
                break;
            case 8:
                a = new Gauss();
                break;
            case 9:
                a = new Egon();
                break;
            case 10:
                a = new HornetGun();
                break;
            case 11:
                a = new GrenadeWeapon();
                break;
            case 12:
                a = new SatchelWeapon();
                break;
            case 13:
                a = new TripmineWeapon();
                break;
            case 14:
                a = new SnarkWeapon();
                break;
            default:
                a = new SMG();
                break;
        }
        a.Position = Position;
        a.Tags.Add( "stubmade" );
    }
    public void SpawnRandomAmmo()
    {
        var rand = Rand.Int( 1, 7 );
        Entity a;
        switch ( rand )
        {
            case 1:
                a = new Ammo9mmClip();
                break;
            case 2:
                a = new Ammo357();
                break;
            case 3:
                a = new Ammo9mmAR();
                break;
            case 4:
                a = new AmmoBuckshot();
                break;
            case 5:
                a = new AmmoCrossbow();
                break;
            case 6:
                a = new AmmoRPG();
                break;
            case 7:
                a = new AmmoUranium();
                break;
            default:
                a = new Ammo9mmAR();
                break;
        }
        a.Position = Position;
        a.Tags.Add( "stubmade" );
    }
    public void SpawnRandomGrenade()
    {
        var rand = Rand.Int( 1, 4 );
        Entity a;
        switch ( rand )
        {
            case 1:
                a = new GrenadeWeapon();
                break;
            case 2:
                a = new SatchelWeapon();
                break;
            case 3:
                a = new TripmineWeapon();
                break;
            case 4:
                a = new SnarkWeapon();
                break;
            default:
                a = new GrenadeWeapon();
                break;
        }
        a.Position = Position;
        a.Tags.Add( "stubmade" );
    }
}
