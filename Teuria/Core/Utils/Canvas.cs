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

    public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, float angle, float length, Color color) 
    {
        spriteBatch.Draw(Pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
    }

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
        Pixel.Dispose();
    }
}