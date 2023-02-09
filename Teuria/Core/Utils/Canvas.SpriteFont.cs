#if !FontStashSharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public partial class Canvas 
{
#region NormalText
    public static void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color) =>
        SpriteBatch.DrawString(spriteFont, text, MathUtils.Floor(position), color);    
    

    public static void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color,
        Vector2 origin, Vector2 scale, float rotation) =>
        SpriteBatch.DrawString(spriteFont, text, MathUtils.Floor(position), color, rotation, origin, scale, SpriteEffects.None, 0f);    
#endregion
    
#region AlignText
    public static void DrawTextAlign(SpriteFont spriteFont, string text, Vector2 position, Color color, 
        Vector2 alignment, float scale = 1) 
    {
        var origin = spriteFont.MeasureString(text);
        origin *= alignment;
        SpriteBatch.DrawString(spriteFont, text, MathUtils.Floor(position), color, 0, origin, scale, SpriteEffects.None, 0f);
    }
#endregion

#region CenteredText
    public static void DrawTextCentered(SpriteFont spriteFont, string text, Vector2 position, Color color) => 
        DrawText(spriteFont, text, position - spriteFont.MeasureString(text) * 0.5f, color);

    public static void DrawTextCentered(SpriteFont spriteFont, string text, Vector2 position, Color color, 
        Vector2 scale, float rotation = 0) => 
        DrawText(spriteFont, text, position, color, spriteFont.MeasureString(text) * 0.5f, Vector2.One * scale, rotation);
#endregion

    public static void DrawOutlineTextCentered(SpriteFont spriteFont, string text, Vector2 position, 
        Color color, Color outlineColor, float scale = 1) 
    {
        var origin = spriteFont.MeasureString(text) * 0.5f;
        var pos = MathUtils.Floor(position);
        for (int x = -1; x < 2; x++)
            for (int y = -1; y < 2; y++) 
                if (x != 0 || y != 0)
                    SpriteBatch.DrawString(
                        spriteFont, text, pos + new Vector2(x, y), 
                        outlineColor, 0, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(spriteFont, text, pos, 
            color, 0, origin, scale, SpriteEffects.None, 0f);
    }

    public static void DrawOutlineTextCentered(SpriteFont spriteFont, string text, Vector2 position, Color color, float scale = 1) =>
        DrawOutlineTextCentered(spriteFont, text, position, color, Color.Black, scale);
    

    public static void DrawOutlineTextAlign(SpriteFont spriteFont, string text, Vector2 position, 
        Color color, Color outlineColor, Vector2 justify, float scale = 1f) 
    {
        var origin = spriteFont.MeasureString(text) * justify;
        var pos = MathUtils.Floor(position);
        for (int x = -1; x < 2; x++)
            for (int y = -1; y < 2; y++) 
                if (x != 0 || y != 0)
                    SpriteBatch.DrawString(
                        spriteFont, text, pos + new Vector2(x, y), 
                        outlineColor, 0, origin, scale, SpriteEffects.None, 0f);
        SpriteBatch.DrawString(spriteFont, text, pos, 
            color, 0, origin, scale, SpriteEffects.None, 0f);
    }

    public static void DrawOutlineTextAlign(SpriteFont spriteFont, string text, Vector2 position, 
        Color color, Vector2 justify, float scale = 1f) =>
        DrawOutlineTextAlign(spriteFont, text, position, color, Color.Black, justify, scale);
 
}
#endif