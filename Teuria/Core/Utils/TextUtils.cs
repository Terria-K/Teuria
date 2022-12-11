using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class TextUtils 
{
    public static float MeasureScreenString(this SpriteFont spriteFont, string text, float width) 
    {
        return width / 2 - spriteFont.MeasureString(text).Length() / 2;
    }
}