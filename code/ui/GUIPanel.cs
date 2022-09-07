using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

[UseTemplate("/resource/templates/options.html")]
public class GUIPanel : Panel
{
    public int ZIndex { get; set; } = 0;
	public bool Dragging;
    public bool MenuOpen;
	public Panel MenuPanel { get; set; }

	public GUIPanel()
	{
		StyleSheet.Load("/resource/styles/GUI.scss");
		Style.Left = 0;
		Style.Right = 0;
		Style.Top = 0;
		Style.Bottom = 0;
		AcceptsFocus = true;
        Style.ZIndex = 0;
        Focus();
	}
	protected override void OnMouseDown(MousePanelEvent e)
	{
		base.OnMouseDown(e);
        Focus();
        Parent.SortChildren(x => x.HasFocus ? 1 : 0);
    }
	public override void Tick()
	{

        Style.ZIndex = Parent.GetChildIndex(this);
        base.Tick();
		Drag();
        SetClass("active", MenuOpen);
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
		if (!Dragging) return;
		Style.Left = Parent.MousePosition.x - xoff;
		Style.Top = Parent.MousePosition.y - yoff;
	}
	public void down()
	{
        
		

        if (Style.Left == null)
		{
            Style.Left = 0;
            Style.Top = 0;
        }
		xoff = (float)(Parent.MousePosition.x - Box.Rect.Left);
        yoff = (float)(Parent.MousePosition.y - Box.Rect.Top);
        Dragging = true;
	}
	public void up()
    {
        Dragging = false;
    }
}
