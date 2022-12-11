using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class TextureImporter 
{
    internal static List<Texture2D> cleanupCache = new List<Texture2D>();

#if NET5_0_OR_GREATER
    public static Texture2D LoadImage(GraphicsDevice device, ReadOnlySpan<char> path) 
    {
        using var fs = File.OpenRead($"Content/{path}");
        var tex = Texture2D.FromStream(device, fs);
        cleanupCache.Add(tex);
        return tex;
    }
    public static Texture2D LoadImageFar(GraphicsDevice device, ReadOnlySpan<char> path) 
    {
        using var fs = File.OpenRead($"{path}");
        var tex = Texture2D.FromStream(device, fs);
        cleanupCache.Add(tex);
        return tex;
    }
#else
    public static Texture2D LoadImage(GraphicsDevice device, string path) 
    {
        using var fs = File.OpenRead("Content/" + path);
        var tex = Texture2D.FromStream(device, fs);
        cleanupCache.Add(tex);
        return tex;
    }
    public static Texture2D LoadImageFar(GraphicsDevice device, string path) 
    {
        using var fs = File.OpenRead(path);
        var tex = Texture2D.FromStream(device, fs);
        cleanupCache.Add(tex);
        return tex;
    }
#endif

    public static void CleanUp(Texture2D texture) 
    {
        if (!cleanupCache.Remove(texture)) return;
        texture.Dispose();
    }
}