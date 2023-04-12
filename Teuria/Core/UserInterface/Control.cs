using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TeuJson;
using TeuJson.Attributes;

namespace Teuria;

public partial class Control : Entity, IDeserialize
{
    [Custom("Teuria.TeuriaCustomConverter")]
    public Vector2 RectSize 
    {
        get => rectSize;
        set 
        {
            rectSize = value;
            dirty = true;
        } 
    }

    [Custom("Teuria.TeuriaCustomConverter")]
    public Vector2 RectPosition 
    {
        get => rectPosition;
        set => rectPosition = value;
    }

    [Ignore]
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

    [TeuObject]
    public Style BaseStyle;

    [TeuObject]
    public Style? HoveredStyle;

    [TeuObject]
    public Style? PressedStyle;

    [Ignore]
    public IReadOnlyList<Control> Childrens => childs;
    public Control? Parent;

    private bool hAlignmentSetup;
    private HorizontalAlignment horizontalAlignment;
    private bool vAlignmentSetup;
    private VerticalAlignment verticalAlignment;
    private Vector2 rectPosition;
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
            child.Depth = Depth - 1;
            scene.Add(child);
        }
        if (hAlignmentSetup)
            SetAlignHorizontal(horizontalAlignment);
        if (vAlignmentSetup)
            SetAlignVertical(verticalAlignment);
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
        Padding parentPosition = Parent != null ? Parent.Padding : Padding.Empty;
        Vector2 parentSize = Parent != null ? Parent.RectSize : Vector2.Zero;

        float alignmentPosition = 0f;

        switch (alignment) 
        {
        case HorizontalAlignment.Left: {
            var sizeX = parentPosition.Left;
            alignmentPosition = sizeX;
            break;
        }
        case HorizontalAlignment.Right: {
            var sizeX = parentSize.X - parentPosition.Right;
            alignmentPosition = sizeX - rectSize.X;
            break;
        }
        case HorizontalAlignment.Center: {
            var sizeX = ((parentSize.X / 2) - parentPosition.Right) + parentPosition.Left;
            alignmentPosition = sizeX - (rectSize.X / 2);
            break;
        }
        }
        rectPosition.X = alignmentPosition;
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
        rectPosition.Y = alignmentPosition;
        return this;
    }

    private void StyleUpdate() 
    {
        if (LocalPosition != rectPosition)
            LocalPosition = rectPosition;

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
            (int)RectSize.X, (int)RectSize.Y), 1, Color.Red);
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

    public static T LoadTheme<T>(string jsonPath) 
    where T : Control, new()
    {
        using var tc = TitleContainer.OpenStream(jsonPath);
        var deserialized = JsonConvert.DeserializeFromStream<T>(tc);
        return deserialized;
    }
}

public enum MouseButton 
{
    LeftClicked,
    RightClicked,
    MiddleClicked
}

public partial struct Padding : IDeserialize
{
    public static readonly Padding Empty = new Padding(0, 0, 0, 0);
    [TeuObject]
    public int Left;
    [TeuObject]
    public int Right;
    [TeuObject]
    public int Top;
    [TeuObject]
    public int Bottom;

    public Padding(int left, int right, int top, int bottom) 
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
    }
}

public static class TeuriaCustomConverter 
{
    public static JsonValue ToJson(this Color value) 
    {
        return new JsonObject 
        {
            ["r"] = value.R,
            ["g"] = value.G,
            ["b"] = value.B,
            ["a"] = value.A
        };
    }

    public static Color ToColor(this JsonValue value) 
    {
        if (value.IsObject) 
        {
            int r = value["r"];
            int g = value["g"];
            int b = value["b"];
            int a = value["a"];
            return new Color(r, g, b, a);
        }
        return Color.White;
    }

    public static JsonValue ToJson(this Vector2 value) 
    {
        return new JsonObject 
        {
            ["x"] = value.X,
            ["y"] = value.Y
        };
    }

    public static Vector2 ToVector2(this JsonValue value) 
    {
        if (value.IsObject) 
        {
            int x = value["x"];
            int y = value["y"];
            return new Vector2(x, y);
        }
        return Vector2.Zero;
    }

    public static Rectangle ToRectangle(this JsonValue value) 
    {
        if (value.IsNull)
            return Rectangle.Empty;
        
        return new Rectangle(value["x"], value["y"], value["width"], value["height"]);
    }

    public static JsonValue ToJson(this Rectangle rectangle) 
    {
        return new JsonObject 
        {
            ["x"] = rectangle.X,
            ["y"] = rectangle.Y,
            ["width"] = rectangle.Width,
            ["height"] = rectangle.Height
        };
    }
}