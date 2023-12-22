using Microsoft.Xna.Framework;

namespace Teuria;

public abstract class Container : Control
{
    public float Offset;
    public override void Update()
    {
        UpdateChildrens();
        base.Update();
    }

    protected abstract void UpdateChildrens();
}

public class HBoxContainer : Container
{
    protected override void UpdateChildrens() 
    {
        if (!dirty)
            return;
        float lastWidth = 0;

        foreach (Control child in Childrens)
        {
            child.RectPosition = new Vector2(child.RectPosition.X + lastWidth, child.RectPosition.Y);
            if (child.RectSize.Y > RectSize.Y) 
            {
                child.RectSize = new Vector2(child.RectSize.X, RectSize.Y);
            }
            lastWidth += child.RectSize.X + Offset;
            if (lastWidth > RectSize.X)
                RectSize = new Vector2(lastWidth, RectSize.Y);
        }
    }
}

public class VBoxContainer : Container
{
    protected override void UpdateChildrens()
    {
        if (!dirty)
            return;
        float lastHeight = 0;

        foreach (var child in Childrens)
        {
            child.RectPosition = new Vector2(child.RectPosition.X, child.RectPosition.Y + lastHeight);
            if (child.RectSize.X > RectSize.X) 
            {
                child.RectSize = new Vector2(RectSize.X, child.RectSize.Y);
            }
            lastHeight += child.RectSize.Y + Offset;
            if (lastHeight > RectSize.Y)
                RectSize = new Vector2(RectSize.X, lastHeight);
        }
    }
}