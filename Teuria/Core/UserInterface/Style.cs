using Microsoft.Xna.Framework;
using TeuJson;
using TeuJson.Attributes;

namespace Teuria;

public partial class Style : IDeserialize
{
    [TeuObject]
    [Custom("Teuria.TeuriaCustomConverter")]
    public Color BackgroundColor;

    [TeuObject]
    public int PaddingLeft;

    [TeuObject]
    public int PaddingRight;

    [TeuObject]
    public int PaddingTop;

    [TeuObject]
    public int PaddingBottom;

    [TeuObject]
    public int TextOutlineStroke;

    [TeuObject]
    [Custom("Teuria.TeuriaCustomConverter")]
    public Color TextOutlineColor;

    [TeuObject]
    [Custom("Teuria.TeuriaCustomConverter")]
    public Color TextColor;

    [TeuObject]
    public HorizontalAlignment TextHorizontalAlignment;

    [TeuObject]
    public VerticalAlignment TextVerticalAlignment;

    [Ignore]
    public Padding Padding => new Padding(PaddingLeft, PaddingTop, PaddingRight, PaddingBottom);

    public Style() {}

    public Style(Style style) 
    {
        BackgroundColor = style.BackgroundColor;
        PaddingLeft = style.PaddingLeft;
        PaddingRight = style.PaddingRight;
        PaddingTop = style.PaddingTop;
        PaddingBottom = style.PaddingBottom;
        TextOutlineStroke = style.TextOutlineStroke;
        TextOutlineColor = style.TextOutlineColor;
        TextColor = style.TextColor;
        TextHorizontalAlignment = style.TextHorizontalAlignment;
        TextVerticalAlignment = style.TextVerticalAlignment;
    }



    public static readonly Style Default = new Style() 
    {
        BackgroundColor = Color.White,
    };
}

public enum HorizontalAlignment 
{
    Left,
    Right,
    Center,
}

public enum VerticalAlignment 
{
    Top,
    Bottom,
    Center
}