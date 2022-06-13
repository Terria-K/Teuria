using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Label : Entity 
{
    protected SpriteFont SpriteFont;
    private string text;
    public string Text { get => text; set => text = value; }

    public Label(SpriteFont spriteFont)
    {
        this.SpriteFont = spriteFont;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (text == null) return;
        spriteBatch.DrawString(SpriteFont, text, Position, Modulate);
        base.Draw(spriteBatch);
    }
}