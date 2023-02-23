#if FontStashSharp
using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static partial class Canvas 
{
    public static Dictionary<string, FontSystem> FontSystems = new();

    public static void CreateFontSystem(string name) 
    {
        var system = new FontSystem();
        FontSystems.Add(name, system);
    }

    public static StaticSpriteFont CreateStaticFont(string directory, string fntName) 
    {
        var fntStream = TitleContainer.OpenStream(Path.Combine(directory, fntName + ".fnt"));
        using TextReader textReader = new StreamReader(fntStream);
        var fntData = textReader.ReadToEnd();

        return StaticSpriteFont.FromBMFont(fntData, 
            filename => TitleContainer.OpenStream(Path.Combine(directory, filename)), GameApp.Instance.GraphicsDevice);
    }

    public static void AddFontSystem(string name, FontSystem system) 
    {
        FontSystems.Add(name, system);
    }

    public static void CreateAndAddFont(string name, string path) 
    {
        var titleContainer = TitleContainer.OpenStream(path);
        if (FontSystems.ContainsKey(name)) 
        {
            FontSystems[name].AddFont(titleContainer);
            return;
        }
        var system = new FontSystem();
        system.AddFont(titleContainer);
        FontSystems.Add(name, system);
    }

    public static void CreateAndAddFont(string name, byte[] bytes) 
    {
        if (FontSystems.ContainsKey(name)) 
        {
            FontSystems[name].AddFont(bytes);
            return;
        }
        var system = new FontSystem();
        system.AddFont(bytes);
        FontSystems.Add(name, system);
    }

    public static void AddFont(string name, string path) 
    {
        var titleContainer = TitleContainer.OpenStream(path);
        FontSystems[name].AddFont(titleContainer);
    }

    public static void AddFont(string name, byte[] bytes) 
    {
        FontSystems[name].AddFont(bytes);
    }

    public static DynamicSpriteFont UseFont(string name, float size) 
    {
        return Canvas.FontSystems[name].GetFont(size);
    }

#region NormalText
    public static void DrawText(SpriteFontBase spriteFont, string text, Vector2 position, Color color) =>
        SpriteBatch!.DrawText(spriteFont, text, MathUtils.Floor(position), color);    
    

    public static void DrawText(SpriteFontBase spriteFont, string text, Vector2 position, Color color,
        Vector2 origin, Vector2 scale, float rotation) =>
        SpriteBatch!.DrawText(spriteFont, text, MathUtils.Floor(position), color, 
            scale, rotation, origin, 0f);    
#endregion
    
#region AlignText
    public static void DrawTextAlign(SpriteFontBase spriteFont, string text, Vector2 position, Color color, 
        Vector2 alignment, float scale = 1) 
    {
        var origin = spriteFont.MeasureString(text);
        origin *= alignment;
        SpriteBatch!.DrawText(
            spriteFont, text, MathUtils.Floor(position), color, new Vector2(scale), 0, origin, 0f);
    }
#endregion

#region CenteredText
    public static void DrawTextCentered(SpriteFontBase spriteFont, string text, Vector2 position, Color color) => 
        DrawText(spriteFont, text, position - spriteFont.MeasureString(text) * 0.5f, color);

    public static void DrawTextCentered(SpriteFontBase spriteFont, string text, Vector2 position, Color color, 
        Vector2 scale, float rotation = 0) => 
        DrawText(spriteFont, text, position, color, spriteFont.MeasureString(text) * 0.5f, Vector2.One * scale, rotation);
#endregion

    public static void DrawOutlineTextCentered(SpriteFontBase spriteFont, string text, Vector2 position, 
        Color color, Color outlineColor, float scale = 1) 
    {
        var origin = spriteFont.MeasureString(text) * 0.5f;
        var pos = MathUtils.Floor(position);
        for (int x = -1; x < 2; x++)
            for (int y = -1; y < 2; y++) 
                if (x != 0 || y != 0)
                    SpriteBatch!.DrawText(
                        spriteFont, text, pos + new Vector2(x, y), 
                        outlineColor, new Vector2(scale), 0, origin, 0f, 
                        0f, 0f);

        SpriteBatch!.DrawText(
            spriteFont, text, pos, 
            color, new Vector2(scale), 0, origin, 0f);
    }

    public static void DrawOutlineTextCentered(SpriteFontBase spriteFont, string text, Vector2 position, Color color, float scale = 1) =>
        DrawOutlineTextCentered(spriteFont, text, position, color, Color.Black, scale);
    

    public static void DrawOutlineTextAlign(SpriteFontBase spriteFont, string text, Vector2 position, 
        Color color, Color outlineColor, Vector2 justify, float scale = 1f) 
    {
        var origin = spriteFont.MeasureString(text) * justify;
        var pos = MathUtils.Floor(position);
        for (int x = -1; x < 2; x++)
            for (int y = -1; y < 2; y++) 
                if (x != 0 || y != 0) 
                {
                    SpriteBatch!.DrawText(
                        spriteFont, text, pos + new Vector2(x, y), 
                        outlineColor, new Vector2(scale), 0, origin, 0f);
                }

        SpriteBatch!.DrawText(
            spriteFont, text, pos, 
            color, new Vector2(scale), 0, origin, 0f);
    }

    public static void DrawOutlineTextAlign(SpriteFontBase spriteFont, string text, Vector2 position, 
        Color color, Vector2 justify, float scale = 1f) =>
        DrawOutlineTextAlign(spriteFont, text, position, color, Color.Black, justify, scale);
    
}
#endif