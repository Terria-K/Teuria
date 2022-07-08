using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Label : Entity 
{
    public SpriteFont SpriteFont { get; private set; }
    public string Text { get; set; }
    public int Size { get; set; }

    public Label(SpriteFont spriteFont)
    {
        this.SpriteFont = spriteFont;
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
        spriteBatch.DrawString(SpriteFont, Text, Position, Modulate, Rotation, Vector2.Zero, Scale, SpriteEffects.None, ZIndex);
        base.Draw(spriteBatch);
    }
}