using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class FontText : Component 
{
    private SpriteFont spriteFont;
    public string Text { get; set; }
    public int Size { get; set; } = 1;
    public float Spacing { get => spriteFont.Spacing; set => spriteFont.Spacing = value; }

    public FontText(SpriteFont spriteFont) 
    {
        this.spriteFont = spriteFont;
    }

    public static FontText Create(string path, ContentManager content) 
    {
        var font = content.Load<SpriteFont>(path);
        return new FontText(font);
    }

    public ref SpriteFont ShareSpriteFont() 
    {
        return ref spriteFont;
    }

    public float MeasureString() 
    {
        return spriteFont.MeasureString(Text).Length();
    }

    public float MeasureStringHalf() 
    {
        return spriteFont.MeasureString(Text).Length() / 2;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(spriteFont, Text, Entity.Position, Entity.Modulate, 0, Vector2.Zero, Size, SpriteEffects.None, Entity.ZIndex);
        base.Draw(spriteBatch);
    }
}