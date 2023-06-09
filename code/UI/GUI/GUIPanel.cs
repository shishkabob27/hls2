using Sandbox.UI;

[UseTemplate( "/Resource/templates/options.html" )]
public class GUIPanel : Panel
{
    public int ZIndex { get; set; } = 0;
    public bool Dragging;
    public bool MenuOpen;
    public Panel MenuPanel { get; set; }

    public Vector2 Position;

    public GUIPanel()
    {
        StyleSheet.Load( "/Resource/styles/GUI.scss" );
        Style.FontFamily = "Tahoma";
        Style.Left = 0;
        Style.Right = 0;
        Style.Top = 0;
        Style.Bottom = 0;
        AcceptsFocus = true;
        Style.ZIndex = 0;
        Focus();
    }
    protected override void OnMouseDown( MousePanelEvent e )
    {
        base.OnMouseDown( e );
		if (!HasFocus)
		{
			Focus();
		}
		Parent.SortChildren( x => x.HasFocus ? 1 : 0 );
	}
    public override void Tick()
    {
        Style.ZIndex = Parent.GetChildIndex( this );
        base.Tick();
        Drag();
        SetClass( "active", MenuOpen );
        if ( HLGame.hl_pixelfont )
        {
            Parent.Style.FontFamily = "Tahoma";
            Style.FontFamily = "Tahoma";
        }
        else
        {
            Parent.Style.FontFamily = "Tahoma";
            Style.FontFamily = "Tahoma";
        }
        Style.Dirty();
    }
    [Event.PreRender]
    void draw()
    {
        Style.Left = Position.x / ( Parent as GUIRootPanel ).Scale;
        Style.Top = Position.y / ( Parent as GUIRootPanel ).Scale;
    }
    public virtual void Close()
    {
        MenuOpen = !MenuOpen;
        this.Delete();
    }
    float xoff = 0;
    float yoff = 0;
    public void Drag()
    {
        if ( !Dragging ) return;
        Position.x = ( ( Parent.MousePosition.x ) - xoff );
        Position.y = ( ( Parent.MousePosition.y ) - yoff );
    }
    public void down()
    {



        if ( Style.Left == null )
        {
            Style.Left = 0;
            Style.Top = 0;
        }
        xoff = (float)( Parent.MousePosition.x - Box.Rect.Left );
        yoff = (float)( Parent.MousePosition.y - Box.Rect.Top );
        Dragging = true;
    }
    public void up()
    {
        Dragging = false;
    }
}
