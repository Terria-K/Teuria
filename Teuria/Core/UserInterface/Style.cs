using Microsoft.Xna.Framework;

namespace Teuria;

public class Style 
{
    public Color BackgroundColor;
    public int PaddingLeft;
    public int PaddingRight;
    public int PaddingTop;
    public int PaddingBottom;

    public Padding Padding => new Padding(PaddingLeft, PaddingTop, PaddingRight, PaddingBottom);

    public static readonly Style Default = new Style() 
    {
        BackgroundColor = Color.White,
    };
}

public sealed class FontStyle : Style 
{
    public int TextOutlineStroke;
    public Color TextOutlineColor;
    public Color TextColor;
    public HorizontalAlignment TextHorizontalAlignment;
    public VerticalAlignment TextVerticalAlignment;
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