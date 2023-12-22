using Microsoft.Xna.Framework;
using TeuJson;
using TeuJson.Attributes;

namespace Teuria;

public class Style
{
    public Color BackgroundColor;
    public int PaddingLeft;
    public int PaddingRight;
    public int PaddingTop;
    public int PaddingBottom;
    public int TextOutlineStroke;
    public Color TextOutlineColor;
    public Color TextColor;
    public HorizontalAlignment TextHorizontalAlignment;
    public VerticalAlignment TextVerticalAlignment;
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