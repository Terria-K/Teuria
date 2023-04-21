using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public static class TeuriaImporter 
{
    internal static List<Texture2D> cleanupCache = new List<Texture2D>();

    public static Effect LoadEffect(string path) 
    {
        using var tc = TitleContainer.OpenStream(path);
        using var ms = new MemoryStream();
        tc.CopyTo(ms);
        return new Effect(GameApp.Instance.GraphicsDevice, ms.ToArray());
    }

    public static Texture2D LoadImage(string path) 
    {
        using var fs = TitleContainer.OpenStream(path);

        return InternalLoadTexture(fs);
    }

    public static Texture2D LoadImage(Stream fs) 
    {
        return InternalLoadTexture(fs);
    }

    private unsafe static Texture2D InternalLoadTexture(Stream fs) 
    {
        var device = GameApp.Instance.GraphicsDevice;
        var tex = Texture2D.FromStream(device, fs);
        var size = tex.Width * tex.Height;
        Color[] texColor = new Color[size];
        tex.GetData<Color>(texColor, 0, size);
        fixed (Color *ptr = &texColor[0]) 
        {
            for (int i = 0; i < size; i++) 
            {
                ptr[i].R = (byte)(ptr[i].R * (ptr[i].A / 255f));
                ptr[i].G = (byte)(ptr[i].G * (ptr[i].A / 255f));
                ptr[i].B = (byte)(ptr[i].B * (ptr[i].A / 255f));
            }
        }
        tex.SetData<Color>(texColor, 0, size);
        cleanupCache.Add(tex);
        return tex;
    }

    public static void CleanUp(Texture2D texture) 
    {
        if (!cleanupCache.Remove(texture)) 
        {
            texture?.Dispose();
            return;
        }
        texture.Dispose();
    }

    public static Texture2D LoadQoiImage(string path) 
    {
        using var fs = TitleContainer.OpenStream(path);
        return LoadQoiImage(fs);
    }

    public static Texture2D LoadQoiImage(Stream fs) 
    {
        using var ms = new MemoryStream();
        fs.CopyTo(ms);
        var qoi = Qoi.QoiDecoder.Decode(ms.ToArray());
        return qoi.ToTexture2D();
    }

    [Conditional("DEBUG")]
    public static void AssertQoiImageWithPNG(string qoi, string png) 
    {
        var qoiData = LoadQoiImage(qoi);
        var device = GameApp.Instance.GraphicsDevice;
        using var fs = TitleContainer.OpenStream(png);
        var tex = Texture2D.FromStream(device, fs);
        byte[] b = new byte[tex.Width * tex.Height * 4];
        tex.GetData<byte>(b);
        byte[] c = new byte[qoiData.Width * qoiData.Height * 4];
        qoiData.GetData<byte>(c);

        SkyLog.Assert(b.SequenceEqual(c), "Qoi Encoding messed up some Pixels");
    }
}