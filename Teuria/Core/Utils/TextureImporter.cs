using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class TextureImporter 
{
    internal static List<Texture2D> cleanupCache = new List<Texture2D>();

    public static Texture2D LoadImage(ReadOnlySpan<char> path) 
    {
        var device = GameApp.Instance.GraphicsDevice;
        using var fs = TitleContainer.OpenStream($"Content/{path}");
        var tex = Texture2D.FromStream(device, fs);
        var size = tex.Width * tex.Height;
        Color[] texColor = new Color[size];
        tex.GetData<Color>(texColor, 0, size);
        unsafe {
            fixed (Color* ptr = &texColor[0]) 
            {
                for (int i = 0; i < size; i++) 
                {
                    ptr[i].R = (byte)(ptr[i].R * (ptr[i].A / 255f));
                    ptr[i].G = (byte)(ptr[i].G * (ptr[i].A / 255f));
                    ptr[i].B = (byte)(ptr[i].B * (ptr[i].A / 255f));
                }
            }
        }
        tex.SetData<Color>(texColor, 0, size);

        cleanupCache.Add(tex);
        return tex;
    }

    public static void CleanUp(Texture2D texture) 
    {
        if (!cleanupCache.Remove(texture)) return;
        texture.Dispose();
    }
}