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
		inctextindex(i);
	}
	[ClientRpc]
	void inctextindex(int ovr = -255)
	{
		
		if (ovr != -255)
		{

			curFrame = ovr - 1;
		}
		try
		{

			if ( Entity.FindAllByName( TargetEntity ).First() is BrushEntity b )
			{
				curFrame++;
				if (b.SceneObject != null) b.SceneObject.Attributes.Set( "frame", curFrame );
			}
			else if ( Entity.FindAllByName( TargetEntity ).First() is ModelEntity m )
			{
				curFrame++;
				if ( m.SceneObject != null ) m.SceneObject.Attributes.Set( "frame", curFrame );
			}
		}
		catch { }
	}

}
