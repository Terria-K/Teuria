using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Control : Entity
{
    public int RectDepth;
    public Vector2 RectSize 
    {
        get => rectSize;
        set 
        {
            rectSize = value;
            dirty = true;
        } 
    }
    public Vector2 RectPosition 
    {
        get => LocalPosition;
        set 
        {
            LocalPosition = value;
        } 
    }

    public float RectPosX 
    {
        get => LocalPosX;
        set 
        {
            LocalPosX = value;
        }
    }

    public float RectPosY 
    {
        get => LocalPosY;
        set 
        {
            LocalPosY = value;
        }
    }

    public Rectangle RectSizeWithPadding => new Rectangle(
        (int)rectSize.X + currentStyle.PaddingLeft, 
        (int)rectSize.Y + currentStyle.PaddingTop, 
        (int)rectSize.X - currentStyle.PaddingRight * 2, 
        (int)rectSize.Y - currentStyle.PaddingBottom * 2
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

    public IReadOnlyList<Control> Childrens => childs;
    public Control? Parent;

    private bool hAlignmentSetup;
    private HorizontalAlignment horizontalAlignment;
    private bool vAlignmentSetup;
    private VerticalAlignment verticalAlignment;
    private Vector2 rectSize;
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
        ChangeStyle(BaseStyle);
        base.EnterScene(scene, content);
        foreach (var child in childs) 
        {
            child.Depth = (child.RectDepth + Depth) - 1;
            scene.Add(child);
        }
        if (hAlignmentSetup)
            SetAlignHorizontal(horizontalAlignment);
        if (vAlignmentSetup)
            SetAlignVertical(verticalAlignment);
    }

    public override void ExitScene(Scene scene)
    {
        base.ExitScene(scene);
        foreach (var child in childs) 
        {
            scene.Remove(child);
        }
    }

    public Control AddChild(Control control) 
    {
        childs.Add(control);
        AddTransform(control);
        control.Parent = this;
        if (Scene != null) 
        {
            control.Depth = (control.RectDepth + Depth) - 1;
            Scene.Add(control);
        }
        if (control.rectSize.X > rectSize.X)
            rectSize.X = control.rectSize.X;
        if (control.rectSize.Y > rectSize.Y)
            rectSize.Y = control.rectSize.Y;

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

    public T GetChild<T>(string name) 
    where T : Control
    {
        return (T)childs[indexLookup[name]];
    }

    public T GetChild<T>(int idx) 
    where T : Control
    {
        return (T)childs[idx];
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

    public Control SetAlignHorizontal(HorizontalAlignment alignment) 
    {
        if (Scene == null) 
        {
            hAlignmentSetup = true;
            horizontalAlignment = alignment;
            return this;
        }
        Padding parentPadding = Parent != null ? Parent.Padding : Padding.Empty;
        Vector2 parentSize = Parent != null ? Parent.RectSize : Vector2.Zero;

        float alignmentPosition = 0f;

        switch (alignment) 
        {
        case HorizontalAlignment.Left: {
            var sizeX = parentPadding.Left;
            alignmentPosition = sizeX;
            break;
        }
        case HorizontalAlignment.Right: {
            var sizeX = parentSize.X - parentPadding.Right;
            alignmentPosition = sizeX - rectSize.X;
            break;
        }
        case HorizontalAlignment.Center: {
            var sizeX = ((parentSize.X / 2) - parentPadding.Right) + parentPadding.Left;
            alignmentPosition = sizeX - (rectSize.X / 2);
            break;
        }
        }
        LocalPosX = alignmentPosition;
        return this;
    }

    public Control SetAlignVertical(VerticalAlignment alignment) 
    {
        if (Scene == null) 
        {
            vAlignmentSetup = true;
            verticalAlignment = alignment;
            return this;
        }
        Padding parentPosition = Parent != null ? Parent.Padding : Padding.Empty;
        Vector2 parentSize = Parent != null ? Parent.RectSize : Vector2.Zero;

        float alignmentPosition = 0f;

        switch (alignment) 
        {
        case VerticalAlignment.Top: {
            var sizeX = parentPosition.Top;
            alignmentPosition = sizeX;
            break;
        }
        case VerticalAlignment.Bottom: {
            var sizeY = parentSize.Y - parentPosition.Bottom;
            alignmentPosition = sizeY - rectSize.Y;
            break;
        }
        case VerticalAlignment.Center: {
            var sizeY = ((parentSize.Y / 2) - parentPosition.Bottom) + parentPosition.Top;
            alignmentPosition = sizeY - (rectSize.Y / 2);
            break;
        }
        }
        LocalPosY = alignmentPosition;
        return this;
    }

    private void StyleUpdate() 
    {
        if (!dirty)
            return;
        

        foreach (var child in Childrens) 
        {
            child.dirty = true;
        }

        dirty = false;
    }
    
    private void EventUpdate() 
    {
        // Clicked
        if (MouseDetectionArea().Contains(TInput.Mouse.ScreenToWorld(Scene!.Camera!)))
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
        if (style == null || currentStyle == style)
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
            (int)PosX, (int)PosY, 
            (int)rectSize.X, (int)rectSize.Y), 1, Color.Red);
        if (currentStyle.PaddingBottom > 0 || 
            currentStyle.PaddingLeft > 0 || 
            currentStyle.PaddingRight > 0 || 
            currentStyle.PaddingTop > 0
        ) 
        {
            Canvas.DrawRect(spriteBatch, new Rectangle(
                (int)PosX + currentStyle.PaddingLeft, (int)PosY + currentStyle.PaddingTop, 
                (int)RectSize.X - currentStyle.PaddingRight, (int)RectSize.Y - currentStyle.PaddingBottom), 1, Color.Blue);
        }
    }

    public virtual Rectangle MouseDetectionArea() 
    {
        return new Rectangle((int)PosX, (int)PosY, (int)rectSize.X, (int)rectSize.Y);
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