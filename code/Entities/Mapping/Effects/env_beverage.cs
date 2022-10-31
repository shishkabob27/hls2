[Library("env_beverage")]
[HammerEntity]
[Title("env_beverage"), Category("Legacy")]
public partial class env_beverage : Entity
{

	int Activated;

    /// <summary>
	/// 0: Coca-Cola
    /// 1: Sprite
    /// 2: Diet Coke
    /// 3: Orange
    /// 4: Surge
    /// 5: Moxie
    /// 6: Random
	/// </summary>
    [Property]
    public int beveragetype { get; set; } = 6;

    [Property]
    public int health { get; set; } = 0;
    // stub
    [Input]
    public void Activate()
    {
		if (health != Activated)
		{
			var soda = new item_sodacan{
            type = beveragetype,
            Position = Position,
			Rotation = Rotation
        	};

			Activated++;
		}
    }
}
