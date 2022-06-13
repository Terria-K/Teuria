using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class TextureImporter 
{
    public static Texture2D LoadImage(GraphicsDevice device, string path) 
    {
        using var fs = File.OpenRead("Content/" + path);
        return Texture2D.FromStream(device, fs);
    }
}