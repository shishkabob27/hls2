[Library( "env_texturetoggle" )]
[HammerEntity]
[Title( "env_texturetoggle" ), Category( "Effects" ), Icon( "toggle_on" )]
public partial class TextureToggle : Entity
{
	[Property( "target" ), Title( "Target" ), FGDType( "target_destination" ), Net]
	public string TargetEntity { get; set; }
	int curFrame = 0;
	[Input]
	public void IncrementTextureIndex()
	{
		inctextindex();
	}
	[Input]
	public void SetTextureIndex(int i)
	{
		curFrame = i - 1;
		inctextindex();
	}
	[ClientRpc]
	void inctextindex()
	{
		if ( Entity.FindAllByName( TargetEntity ).First() is BrushEntity b )
		{
			curFrame++;
			b.SceneObject.Attributes.Set( "frame", curFrame );
		}
		else if ( Entity.FindAllByName( TargetEntity ).First() is ModelEntity m )
		{
			curFrame++;
			m.SceneObject.Attributes.Set( "frame", curFrame );
		}
	}

}
