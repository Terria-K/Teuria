using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class TextureImporter 
{
#if NET5_0_OR_GREATER
    public static Texture2D LoadImage(GraphicsDevice device, ReadOnlySpan<char> path) 
    {
        using var fs = File.OpenRead($"Content/{path}");
        return Texture2D.FromStream(device, fs);
    }
#else
    public static Texture2D LoadImage(GraphicsDevice device, string path) 
    {
        using var fs = File.OpenRead("Content/" + path);
        return Texture2D.FromStream(device, fs);
    }
#endif
}