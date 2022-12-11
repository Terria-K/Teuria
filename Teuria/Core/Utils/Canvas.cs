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

    public static void DrawGrid<T>(SpriteBatch spriteBatch, int width, int height, int cellSize, List<T>[,] grid) 
    {
        // for (int i = 0; i < grid.GetLength(0); i++) 
        // {
        //     if (grid[i].Count > 0) 
        //     {
        //         float x = (float)((decimal)(i % width) * cellSize);
        //         float y = (float)(Math.Floor((decimal)i / width) * cellSize);

        //         Color color;
        //         if (grid[i].Count == 1)
        //             color = new Color(0.4f, 0.4f, 0.4f, 0.4f);
        //         else
        //             color = new Color(0.7f, 0.4f, 0.4f, 0.6f);
                
        //         DrawRect(spriteBatch, new Rectangle((int)x, (int)y, cellSize,cellSize), 1, color);
        //     }
        // }
    }

    internal static void Dispose() 
    {
        pixel.Dispose();
    }
}