using Microsoft.Xna.Framework.Graphics;

namespace Teuria.Qoi;

public struct QoiImage
{
    public byte[] Data;
    public int Width;
    public int Height;
    public Channels Channels;
    public ColorSpace ColorSpace;


    public QoiImage(byte[] data, int width, int height, Channels channels, ColorSpace colorSpace = ColorSpace.SRgb)
    {
        Data = data;
        Width = width;
        Height = height;
        Channels = channels;
        ColorSpace = colorSpace;
    }

    public Texture2D ToTexture2D() 
    {
        var tex2D = new Texture2D(GameApp.Instance.GraphicsDevice, Width, Height, false, SurfaceFormat.Color);
        DefaultColorProcessors.ZeroTransparentPixels(Data);
        tex2D.SetData(Data);
        return tex2D;
    }
}
