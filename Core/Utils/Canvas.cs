using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class Canvas 
{  
    private static Rectangle rectangle;
    private static Texture2D pixel;

    public static void Initialize(GraphicsDevice device) 
    {
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
}