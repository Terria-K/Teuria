using System.Text;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class TextLabel : Control 
{
    public required SpriteFontBase Font 
    {
        get => font;
        set 
        {
            if (font == value)
                return;
            font = value;
            dirty = true;
        }
    }
    // This will be not null anyway as it handled by the Font property
    private SpriteFontBase font = null!;
    public string Text
    {
        get => text;
        set 
        {
            if (text == value)
                return;
            text = value;
            dirty = true;
        }
    }
    private string text = "";
    public int FontSize 
    {
        get => fontSize;
        set 
        {
            if (fontSize == value)
                return;
            fontSize = value;
            dirty = true;
        }
    }
    private int fontSize;
    public FontStyle? FontStyle 
    {
        get => fontStyle;
        set 
        {
            if (fontStyle == value)
                return;
            fontStyle = value;
            dirty = true;
        }
    }
    private FontStyle? fontStyle;
    

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        if (!dirty && Text != string.Empty)
        {
            var text = this.text;
            if (RectSize.Width > 0) 
            {
                text = WrapText();
            }
            if (fontStyle == null) 
            {
                Canvas.DrawText(font, text, Position, Color.White, Vector2.Zero, new Vector2(fontSize), Rotation);
            }
            else if (fontStyle.TextOutlineStroke <= 0) 
            {
                var alignedPosition = AdjustAlignment(font.MeasureString(text));
                Canvas.DrawText(font, text, alignedPosition, fontStyle.TextColor, Vector2.Zero, new Vector2(fontSize), Rotation);
            }
            else 
            {
                var alignedPosition = AdjustAlignment(font.MeasureString(text));
                Canvas.DrawOutlineTextAlign(
                    font, text, alignedPosition, 
                    fontStyle.TextColor, fontStyle.TextOutlineColor, Vector2.Zero, fontSize);
            }
        }
    }

    private Vector2 AdjustAlignment(Vector2 measured) 
    {
        if (fontStyle == null)
            return Position;
        var position = Position;
        var size = RectSize;
        Vector2 alignmentPosition = Position;

        switch (fontStyle.TextHorizontalAlignment) 
        {
        case HorizontalAlignment.Left: 
        {
            alignmentPosition.X = position.X + size.X;
            break;
        }

        case HorizontalAlignment.Right: 
        {
            var sizeX = size.Width - size.X;
            alignmentPosition.X = position.X + (sizeX - measured.X);
            break;
        }

        case HorizontalAlignment.Center: 
        {
            var sizeX = size.Width / 2;
            alignmentPosition.X = position.X + (sizeX - (measured.X / 2));
            break;
        }

        }

        switch (fontStyle.TextVerticalAlignment) 
        {
        case VerticalAlignment.Top: {
            alignmentPosition.Y = position.Y + (size.Y);
            break;
        }
        case VerticalAlignment.Bottom: {
            var sizeY = size.Height - position.Y;
            alignmentPosition.Y = position.Y + (sizeY - measured.Y);
            break;
        }
        case VerticalAlignment.Center: {
            var sizeY = size.Height / 2;
            alignmentPosition.Y = position.Y + (sizeY - (measured.Y / 2));
            break;
        }
        }
        return alignmentPosition;
    }

    private string WrapText() 
    {
        if (Text == null)
            return "";
        string[] words = Text.Split(' ');
        var sb = new StringBuilder();
        float lineWidth = 0f;
        float spaceWidth = Font.MeasureString(" ").X;

        foreach (var word in words) 
        {
            var size = Font.MeasureString(word);

            if (lineWidth + size.X < RectSize.Width) 
            {
                sb.Append(word + " ");
                lineWidth += size.X + spaceWidth;
            } else 
            {
                sb.Append("\n" + word + " ");
                lineWidth = size.X + spaceWidth;
            }
        }

        return sb.ToString();
    }
}