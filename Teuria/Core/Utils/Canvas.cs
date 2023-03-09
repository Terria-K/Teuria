#nullable disable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static partial class Canvas 
{  
    private static Rectangle rectangle;
    public static Texture2D Pixel;
    private static GraphicsDevice device;
    public static SpriteBatch SpriteBatch;


    internal static void Initialize(GraphicsDevice device, SpriteBatch spriteBatch) 
    {
        Canvas.device = device;
        SpriteBatch = spriteBatch;
        Pixel = new Texture2D(device, 1, 1);
        Pixel.SetData(new[] { Color.White });
    }

#nullable enable
    public static void SetRenderTarget(RenderTarget2D? target) 
    {
        if (target == null)
            target = GameApp.Instance.TeuriaBackBuffer;
        GameApp.Instance.GraphicsDevice.SetRenderTarget(target);
    } 
#nullable disable

    public static void DrawRectangle(SpriteBatch spriteBatch, float x, float y, float width, float height, Color color) 
    {
        rectangle.X = (int)x;
        rectangle.Y = (int)y;
        rectangle.Width = (int)width;
        rectangle.Height = (int)height;
        spriteBatch.Draw(Pixel, rectangle, color);
    }

    public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color) 
    {
        spriteBatch.Draw(Pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        spriteBatch.Draw(Pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        spriteBatch.Draw(Pixel, new Rectangle((rect.X + rect.Width - thickness), rect.Y, thickness, rect.Height), color);
        spriteBatch.Draw(Pixel, new Rectangle(rect.X, (rect.Y + rect.Height - thickness), rect.Width, thickness), color);
    }

    public static void DrawRect(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color) 
    {
        spriteBatch.Draw(Pixel, new Rectangle(x, y, width, thickness), color);
        spriteBatch.Draw(Pixel, new Rectangle(x, y, thickness, height), color);
        spriteBatch.Draw(Pixel, new Rectangle((x + width - thickness), y, thickness, height), color);
        spriteBatch.Draw(Pixel, new Rectangle(x, (y + height - thickness), width, thickness), color);
    }

    public static void DrawCircle(SpriteBatch spriteBatch, Vector2 position, float radius, Color color, int resolution)
    {
        Vector2 last = Vector2.UnitX * radius;
        Vector2 lastP = new Vector2(-last.Y, last.X);
        for (int i = 1; i <= resolution; i++)
        {
            Vector2 at = MathUtils.DegToVec(i * MathHelper.PiOver2 / resolution, radius);
            Vector2 atP = new Vector2(-at.Y, at.X);

            DrawLine(spriteBatch, position + last, position + at, color);
            DrawLine(spriteBatch, position - last, position - at, color);
            DrawLine(spriteBatch, position + lastP, position + atP, color);
            DrawLine(spriteBatch, position - lastP, position - atP, color);

            last = at;
            lastP = atP;
        }
    }

    public static void DrawCircle(SpriteBatch spriteBatch, float x, float y, float radius, Color color, int resolution)
    {
        DrawCircle(spriteBatch, new Vector2(x, y), radius, color, resolution);
    }

    public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, float angle, float length, Color color) 
    {
        spriteBatch.Draw(Pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
    }

    public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
    {
        DrawLine(spriteBatch, start, MathUtils.Angle(start, end), Vector2.Distance(start, end), color);
    }

    public static void TextureCentered(SpriteBatch spriteBatch, SpriteTexture spriteTexture, Vector2 position, Color color)
    {
        position.Floor();
        spriteBatch.Draw(spriteTexture.Texture, 
            position, new Rectangle?(spriteTexture.Clip), color, 
            0f, new Vector2(spriteTexture.Width * 0.5f, spriteTexture.Height * 0.5f), 
            1f, SpriteEffects.None, 0f);
    }

    public static void TextureHCentered(SpriteBatch spriteBatch, SpriteTexture spriteTexture, Vector2 position, Color color)
    {
        position.Floor();
        spriteBatch.Draw(spriteTexture.Texture, 
            position, new Rectangle?(spriteTexture.Clip), color, 
            0f, new Vector2(spriteTexture.Width * 0.5f, 0), 
            1f, SpriteEffects.None, 0f);
    }

    public static void TextureHCentered(SpriteBatch spriteBatch, SpriteTexture spriteTexture, Rectangle rectangle, Color color)
    {
        var centeredHRectangle = new Rectangle(
            (int)(rectangle.X - (spriteTexture.Width * 0.5f)), (int)(rectangle.Y), rectangle.Width, rectangle.Height);
        var destPatches = spriteTexture.CreatePatches(
            centeredHRectangle, spriteTexture.Padding.X, spriteTexture.Padding.X + 
            spriteTexture.Padding.Width, spriteTexture.Padding.Y, spriteTexture.Padding.Y + 
            spriteTexture.Padding.Height);

        for (int i = 0; i < spriteTexture.Patches.Length; i++)
            spriteBatch.Draw(
                spriteTexture.Texture, sourceRectangle: spriteTexture.Patches[i], 
                destinationRectangle: destPatches[i], color: color);
    }

    public static void TextureVCentered(SpriteBatch spriteBatch, SpriteTexture spriteTexture, Vector2 position, Color color)
    {
        position.Floor();
        spriteBatch.Draw(spriteTexture.Texture, 
            position, new Rectangle?(spriteTexture.Clip), color, 
            0f, new Vector2(0f, spriteTexture.Height * 0.5f), 
            1f, SpriteEffects.None, 0f);
    }
    
    
    internal static void Dispose() 
    {
        Pixel?.Dispose();
    }
}