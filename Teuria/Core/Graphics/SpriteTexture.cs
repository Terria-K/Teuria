using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;

namespace Teuria;

public class SpriteTexture 
{
    public Texture2D Texture { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Rectangle Clip { get; private set; }
    public Vector2 Origin { get; private set; }
    public string AtlasPath { get; private set; }
    public Rectangle[] Patches { get; private set; }
    public Rectangle Padding { get; private set; }
    
    public SpriteTexture(Texture2D texture, Rectangle padding = default) 
    {
        Texture = texture;
        Clip = new Rectangle(0, 0, Texture.Width, Texture.Height);
        Origin = Vector2.Zero;
        Width = Clip.Width;
        Height = Clip.Height;
        Padding = padding;
        Patches = CreatePatches(
            Texture.Bounds, 
            padding.X, 
            padding.X + padding.Width, 
            padding.Y, 
            padding.Y + padding.Height
        );
    }

    public SpriteTexture(Texture2D texture, Point regionPos, int width, int height, Rectangle padding = default) 
    {
        Texture = texture;
        X = regionPos.X;
        Y = regionPos.Y;
        Clip = new Rectangle(X, Y, width, height);
        Origin = Vector2.Zero;
        Width = Clip.Width;
        Height = Clip.Height;
        Padding = padding;
        Patches = CreatePatches(
            Clip, 
            padding.X, 
            padding.X + padding.Width, 
            padding.Y, 
            padding.Y + padding.Height
        );
    }

    public SpriteTexture(SpriteTexture spriteTexture, string atlasPath, Rectangle clip, Vector2 offset, int width, int height) 
    {
        Texture = spriteTexture.Texture;
        AtlasPath = atlasPath;
        Clip = clip;
        Origin = offset;
        Width = width;
        Height = height;
    }

    public SpriteTexture(SpriteTexture spriteTexture, Rectangle rect) 
    {
        Texture = spriteTexture.Texture;
        Clip = spriteTexture.GetRelativeRect(ref rect);
        var origX = Math.Min(rect.X - spriteTexture.Origin.X, 0);
        var origY = Math.Min(rect.Y - spriteTexture.Origin.Y, 0);
        Origin = new Vector2(-origX, -origY);
        Width = rect.Width;
        Height = rect.Height;
    }

    public Rectangle GetRelativeRect(ref Rectangle rectangle) 
    {
        return GetRelativeRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    public Rectangle GetRelativeRect(int x, int y, int width, int height)
    {
        var orig = new Point(
            (int)(Clip.X - Origin.X + x),
            (int)(Clip.Y - Origin.Y + y)
        );
        var pos = new Point(
            MathHelper.Clamp(orig.X, Clip.Left, Clip.Right),
            MathHelper.Clamp(orig.Y, Clip.Top, Clip.Bottom)
        );
        var w = Math.Max(0, Math.Min(orig.X + width, Clip.Right) - pos.X);
        var h = Math.Max(0, Math.Min(orig.Y + height, Clip.Bottom) - pos.Y);

        return new Rectangle(pos.X, pos.Y, w, h);
    }

    public static SpriteTexture FromFile(GraphicsDevice device, string filename) 
    {
        return new SpriteTexture(TextureImporter.LoadImage(device, filename));
    }

    public static SpriteTexture FromContent(ContentManager content, string filename) 
    {
        return new SpriteTexture(content.Load<Texture2D>(filename));
    }

    

    private Rectangle[] CreatePatches(Rectangle rectangle, int left, int right, int bottom, int top) 
    {
        var x = rectangle.X;
        var y = rectangle.Y;
        var w = rectangle.Width;
        var h = rectangle.Height;
        var mWidth = w - left - right;
        var mHeight = h - top - bottom;
        var bottomY = y + h - bottom;
        var rightX = x + w - right;
        var leftX = x + left;
        var topY = y + top;
        var patches = new[] 
        {
            new Rectangle(x, y, left, top),
            new Rectangle(leftX, y, mWidth, top),
            new Rectangle(rightX, y, right, top),
            new Rectangle(x, topY, left, mHeight),
            new Rectangle(leftX, topY, mWidth, mHeight),
            new Rectangle(rightX, topY, right, mHeight),
            new Rectangle(x, bottomY, left, bottom),
            new Rectangle(leftX, bottomY, mWidth, bottom),
            new Rectangle(rightX, bottomY, right, bottom)
        };
        return patches;
    }

    public void DrawTexture(SpriteBatch spriteBatch, Rectangle rectangle) 
    {
        var col = Color.White;
#if DEBUG
        if (RectangleShape.DebugRender)
            col = Color.White * 0.5f;
#endif
        var destPatches = CreatePatches(rectangle, Padding.X, Padding.X + Padding.Width, Padding.Y, Padding.Y + Padding.Height);
        for (int i = 0; i < Patches.Length; i++)
            spriteBatch.Draw(Texture, sourceRectangle: Patches[i], destinationRectangle: destPatches[i], color: col);
    }

    public void DrawTexture(SpriteBatch spriteBatch, Vector2 position) 
    {
        var color = Color.White;
#if DEBUG
        if (RectangleShape.DebugRender)
            color = color * 0.5f;
#endif

        spriteBatch.Draw(Texture, position, Clip, color, 0, -Origin, 1f, SpriteEffects.None, 0);
    }

    public void DrawTexture(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 scale, SpriteEffects spriteEffects, float zIndex) 
    {
        var col = color;
#if DEBUG
        if (RectangleShape.DebugRender)
            col = color * 0.5f;
#endif
        spriteBatch.Draw(Texture, position, Clip, col, rotation, -Origin, scale, spriteEffects, zIndex);
    }

    public void DrawTexture(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 offset, Vector2 scale, SpriteEffects spriteEffects, float zIndex) 
    {
        var col = color;
#if DEBUG
        if (RectangleShape.DebugRender)
            col = color * 0.5f;
#endif
        spriteBatch.Draw(Texture, position, Clip, col, rotation, offset, scale, spriteEffects, zIndex);
    }

    public void DrawTexture(SpriteBatch spriteBatch, Vector2 position, Rectangle rectangle, Color color, float rotation, Vector2 offset, Vector2 scale, SpriteEffects spriteEffects, float zIndex) 
    {
        var col = color;
#if DEBUG
        if (RectangleShape.DebugRender)
            col = color * 0.5f;
#endif
        spriteBatch.Draw(Texture, position, rectangle, col, rotation, offset, scale, spriteEffects, zIndex);
    }

    public void DrawTexture(SpriteBatch spriteBatch, Vector2 position, Rectangle rectangle, Color color, float rotation, Vector2 offset, float scale, SpriteEffects spriteEffects, float zIndex) 
    {
        var col = color;
#if DEBUG
        if (RectangleShape.DebugRender)
            col = color * 0.5f;
#endif
        spriteBatch.Draw(Texture, position, rectangle, col, rotation, offset, scale, spriteEffects, zIndex);
    }
}