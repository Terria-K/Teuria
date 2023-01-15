using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class Canvas 
{  
    private static Rectangle rectangle;
    private static Texture2D pixel;
    public static SpriteBatch SpriteBatch;

    internal static void Initialize(GraphicsDevice device, SpriteBatch spriteBatch) 
    {
        SpriteBatch = spriteBatch;
        pixel = new Texture2D(device, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, float x, float y, float width, float height, Color color) 
    {
        rectangle.X = (int)x;
        rectangle.Y = (int)y;
        rectangle.Width = (int)width;
        rectangle.Height = (int)height;
        spriteBatch.Draw(pixel, rectangle, color);
    }

    public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color) 
    {
        spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        spriteBatch.Draw(pixel, new Rectangle((rect.X + rect.Width - thickness), rect.Y, thickness, rect.Height), color);
        spriteBatch.Draw(pixel, new Rectangle(rect.X, (rect.Y + rect.Height - thickness), rect.Width, thickness), color);
    }

    public static void DrawRect(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color) 
    {
        spriteBatch.Draw(pixel, new Rectangle(x, y, width, thickness), color);
        spriteBatch.Draw(pixel, new Rectangle(x, y, thickness, height), color);
        spriteBatch.Draw(pixel, new Rectangle((x + width - thickness), y, thickness, height), color);
        spriteBatch.Draw(pixel, new Rectangle(x, (y + height - thickness), width, thickness), color);
    }

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
    

    public static void TextureCentered(SpriteTexture spriteTexture, Vector2 position, Color color)
    {
        position.Floor();
        SpriteBatch.Draw(spriteTexture.Texture, 
            position, new Rectangle?(spriteTexture.Clip), color, 
            0f, new Vector2(spriteTexture.Width * 0.5f, spriteTexture.Height * 0.5f), 
            1f, SpriteEffects.None, 0f);
    }

    public static void TextureHCentered(SpriteTexture spriteTexture, Vector2 position, Color color)
    {
        position.Floor();
        SpriteBatch.Draw(spriteTexture.Texture, 
            position, new Rectangle?(spriteTexture.Clip), color, 
            0f, new Vector2(spriteTexture.Width * 0.5f, 0), 
            1f, SpriteEffects.None, 0f);
    }

    public static void TextureHCentered(SpriteTexture spriteTexture, Rectangle rectangle, Color color)
    {
        var centeredHRectangle = new Rectangle(
            (int)(rectangle.X - (spriteTexture.Width * 0.5f)), (int)(rectangle.Y), rectangle.Width, rectangle.Height);
        var destPatches = spriteTexture.CreatePatches(
            centeredHRectangle, spriteTexture.Padding.X, spriteTexture.Padding.X + 
            spriteTexture.Padding.Width, spriteTexture.Padding.Y, spriteTexture.Padding.Y + 
            spriteTexture.Padding.Height);

        for (int i = 0; i < spriteTexture.Patches.Length; i++)
            SpriteBatch.Draw(
                spriteTexture.Texture, sourceRectangle: spriteTexture.Patches[i], 
                destinationRectangle: destPatches[i], color: color);
    }

    public static void TextureVCentered(SpriteTexture spriteTexture, Vector2 position, Color color)
    {
        position.Floor();
        SpriteBatch.Draw(spriteTexture.Texture, 
            position, new Rectangle?(spriteTexture.Clip), color, 
            0f, new Vector2(0f, spriteTexture.Height * 0.5f), 
            1f, SpriteEffects.None, 0f);
    }
    
    

    internal static void Dispose() 
    {
        pixel.Dispose();
    }
}