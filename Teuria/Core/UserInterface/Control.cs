using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Control : Entity
{
    public Rectangle RectSize 
    {
        get => rectSize;
        set => rectSize = value;
    }

    public Vector2 RectPosition 
    {
        get => rectPosition;
        set => rectPosition = value;
    }

    public Rectangle RectSizeWithPadding => new Rectangle(
        rectSize.X + currentStyle.PaddingLeft, 
        rectSize.Y + currentStyle.PaddingTop, 
        rectSize.Width - currentStyle.PaddingRight * 2, 
        rectSize.Height - currentStyle.PaddingBottom * 2
    );
    public Padding Padding 
    {
        get => currentStyle.Padding;
        set 
        {
            currentStyle.PaddingLeft = value.Left;
            currentStyle.PaddingTop = value.Top;
            currentStyle.PaddingBottom = value.Bottom;
            currentStyle.PaddingRight = value.Right;
            dirty = true;
        }
    }

    public Style BaseStyle;
    public Style? HoveredStyle;
    public Style? PressedStyle;
    public HorizontalAlignment HorizontalAlignment 
    {
        get => horizontalAlignment;
        set 
        {
            if (horizontalAlignment == value)
                return;
            horizontalAlignment = value;
            dirty = true;
        }
    }
    public VerticalAlignment VerticalAlignment 
    {
        get => verticalAlignment;
        set 
        {
            if (verticalAlignment == value)
                return;
            verticalAlignment = value;
            dirty = true;
        }
    }
    private HorizontalAlignment horizontalAlignment;
    private VerticalAlignment verticalAlignment;
    public IReadOnlyList<Control> Childrens => childs;
    public Control? Parent;

    private Vector2 rectPosition;
    private Rectangle rectSize;
    private Dictionary<string, int> indexLookup;
    private List<Control> childs;
    private bool holdingClick;

    protected Style currentStyle;
    protected bool dirty;

    public Control() 
    {
        childs = new List<Control>();
        indexLookup = new Dictionary<string, int>();
        BaseStyle = Style.Default;
        currentStyle = BaseStyle;
    }

    public override void EnterScene(Scene scene, ContentManager content)
    {
        currentStyle = BaseStyle;
        base.EnterScene(scene, content);
        foreach (var child in childs) 
        {
            child.Depth = Depth - 1;
            scene.Add(child);
        }
        dirty = true;
    }

    public Control AddChild(Control control) 
    {
        childs.Add(control);
        AddTransform(control);
        control.Parent = this;
        if (Scene != null) 
        {
            control.Depth = Depth - 1;
            Scene.Add(control);
        }

        dirty = true;
        return this;
    }

    public Control AddChild(string name, Control control) 
    {
        indexLookup.Add(name, childs.Count);
        return AddChild(control);
    }

    public Control GetChild(string name) 
    {
        return childs[indexLookup[name]];
    }

    public Control GetChild(int idx) 
    {
        return childs[idx];
    }

    public void ApplyStyleChanges(bool alsoWithChildrens = true) 
    {
        ChangeStyle(BaseStyle);
        if (alsoWithChildrens) 
        {
            foreach (var child in childs) 
            {
                child.ApplyStyleChanges(alsoWithChildrens);
            }
        }
    }

    public override void Update()
    {
        EventUpdate();
        StyleUpdate();

        base.Update();
    }

    private void StyleUpdate() 
    {
        if (!dirty)
            return;

        // foreach (var child in Childrens) 
        // {

        //     child.dirty = true;
        // }

        /* Setup Alignment */
        Padding parentPosition = Parent != null ? Parent.Padding : Padding.Empty;
        Rectangle parentSize = Parent != null ? Parent.RectSizeWithPadding : Rectangle.Empty;

        Vector2 alignmentPosition = default;

        switch (horizontalAlignment) 
        {
        case HorizontalAlignment.Left: {
            var sizeX = parentSize.X;
            alignmentPosition.X = sizeX;
            break;
        }
        case HorizontalAlignment.Right: {
            var sizeX = parentSize.Width + parentPosition.Right;
            alignmentPosition.X = sizeX - rectSize.Width;
            break;
        }
        case HorizontalAlignment.Center: {
            var sizeX = (parentSize.Width / 2) + parentPosition.Right;
            alignmentPosition.X = sizeX - (rectSize.Width / 2);
            break;
        }
        }

        switch (verticalAlignment) 
        {
        case VerticalAlignment.Top: {
            var sizeY = parentSize.Y; 
            alignmentPosition.Y = sizeY;
            break;
        }
        case VerticalAlignment.Bottom: {
            SkyLog.Log(parentPosition.Bottom);
            var sizeY = parentSize.Height + parentPosition.Bottom;
            alignmentPosition.Y = sizeY - rectSize.Height;
            break;
        }
        case VerticalAlignment.Center: {
            var sizeY = (parentSize.Height / 2) + parentPosition.Bottom;
            alignmentPosition.Y = sizeY - (rectSize.Height / 2);
            break;
        }
        }

        LocalPosition = rectPosition + alignmentPosition;

        dirty = false;
    }
    
    private void EventUpdate() 
    {
        // Clicked
        if (GetFullSize().Contains(TInput.Mouse.ScreenToWorld(Scene!.Camera!)))
        {
            if (TInput.Mouse.LeftClicked()) 
            {
                MouseReadyEvent(MouseButton.LeftClicked);
                return;
            }
            if (TInput.Mouse.RightClicked()) 
            {
                MouseReadyEvent(MouseButton.RightClicked);
                return;
            }
            if (TInput.Mouse.MiddleClicked()) 
            {
                MouseReadyEvent(MouseButton.MiddleClicked);
                return;
            }

            if (holdingClick) 
            {
                if (TInput.Mouse.LeftReleased()) 
                {
                    holdingClick = false;
                    MouseClickedEvent(MouseButton.LeftClicked);
                }
                if (TInput.Mouse.RightReleased()) 
                {
                    holdingClick = false;
                    MouseClickedEvent(MouseButton.RightClicked);
                }
                if (TInput.Mouse.MiddleReleased()) 
                {
                    holdingClick = false;
                    MouseClickedEvent(MouseButton.MiddleClicked);
                }
            }

            HoveredEvent();
            return;
        }

        if (currentStyle != BaseStyle) 
        {
            ChangeStyle(BaseStyle);
        }
    }

    private void ChangeStyle(Style? style) 
    {
        if (style == null)
            return;
        currentStyle = style;
        dirty = true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
#if DEBUG
        if (Shape.DebugRender)
            DebugDraw(spriteBatch);
#endif
        base.Draw(spriteBatch);
    }

    public virtual void DebugDraw(SpriteBatch spriteBatch) 
    {
        Canvas.DrawRect(spriteBatch, new Rectangle(
            (int)PosX + RectSize.X, (int)PosY + RectSize.Y, 
            RectSize.Width, RectSize.Height), 1, Color.Red);
        if (currentStyle.PaddingBottom > 0 || 
            currentStyle.PaddingLeft > 0 || 
            currentStyle.PaddingRight > 0 || 
            currentStyle.PaddingTop > 0
        ) 
        {
            Canvas.DrawRect(spriteBatch, new Rectangle(
                (int)PosX + RectSizeWithPadding.X, (int)PosY + RectSizeWithPadding.Y, 
                RectSizeWithPadding.Width, RectSizeWithPadding.Height), 1, Color.Blue);
        }
    }

    private Rectangle GetFullSize() 
    {
        return new Rectangle((int)PosX + rectSize.X, (int)PosY + rectSize.Y, rectSize.Width, rectSize.Height);
    }

    private void HoveredEvent() 
    {
        ChangeStyle(HoveredStyle);
        OnHoveredEvent();
    }

    private void MouseReadyEvent(MouseButton state) 
    {
        ChangeStyle(PressedStyle);
        holdingClick = true;
    }

    private void MouseClickedEvent(MouseButton state) 
    {
        OnMouseEvent(state);
    }

    public virtual void OnHoveredEvent() {}
    public virtual void OnMouseEvent(MouseButton state) {}
}

public enum MouseButton 
{
    LeftClicked,
    RightClicked,
    MiddleClicked
}

public struct Padding 
{
    public static readonly Padding Empty = new Padding(0, 0, 0, 0);
    public int Left;
    public int Right;
    public int Top;
    public int Bottom;

    public Padding(int left, int right, int top, int bottom) 
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }
}