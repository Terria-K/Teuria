using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Label : Entity 
{
    public enum LabelType { Normal, Outline }
    public SpriteFont SpriteFont { get; private set; }
    public string Text { get; set; }
    public float Size { get; set; } = 1;
    public LabelType TextType;
    public Rectangle Rect { get; set; }

    public Label(SpriteFont spriteFont, Rectangle rectangle = default)
    {
        this.SpriteFont = spriteFont;
        Rect = rectangle;
    }

    public string WrapText() 
    {
        string[] words = Text.Split(' ');
        var sb = new StringBuilder();
        float lineWidth = 0f;
        float spaceWidth = SpriteFont.MeasureString(" ").X;

        foreach (var word in words) 
        {
            var size = SpriteFont.MeasureString(word);

            if (lineWidth + size.X < Rect.Width) 
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

    public float MeasureString() 
    {
        return SpriteFont.MeasureString(Text).Length();
    }

    public float MeasureStringHalf() 
    {
        return SpriteFont.MeasureString(Text).Length() / 2;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var text = Text;
        if (Rect.Width > 0)
            text = WrapText();
        
        switch (TextType) 
        {
        case LabelType.Normal:
            Canvas.DrawText(SpriteFont, text, Position, Modulate, Vector2.Zero, new Vector2(Size), Rotation);
            break;
        case LabelType.Outline:
            Canvas.DrawOutlineTextAlign(SpriteFont, text, Position, Modulate, Color.Black, new Vector2(0, 0), Size);
            break;
        }
        base.Draw(spriteBatch);
    }
}