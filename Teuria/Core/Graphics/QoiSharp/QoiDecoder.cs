using System;
using Teuria;

namespace Teuria.Qoi;

public static class QoiDecoder 
{
    public static QoiImage Decode(ReadOnlySpan<byte> data)
    {
#if DEBUG
        if (data.Length < QoiCodec.HeaderSize + QoiCodec.ReadOnlyPadding.Length)
        {
            throw new Exception("File too short");
        }
        
        if (!QoiCodec.IsValidMagic(data.Slice(0, 4)))
        {
            throw new Exception("Invalid file magic"); // TODO: add magic value
        }
#endif

        int width = data[4] << 24 | data[5] << 16 | data[6] << 8 | data[7];
        int height = data[8] << 24 | data[9] << 16 | data[10] << 8 | data[11];
        byte channels = data[12]; 
        var colorSpace = (ColorSpace)data[13];

#if DEBUG
        if (width == 0)
        {
            throw new Exception($"Invalid width: {width}");
        }
        if (height == 0 || height >= QoiCodec.MaxPixels / width)
        {
            throw new Exception($"Invalid height: {height}. Maximum for this image is {QoiCodec.MaxPixels / width - 1}");
        }
        if (channels is not 3 and not 4)
        {
            throw new Exception($"Invalid number of channels: {channels}");
        }
#endif

        Span<int> index = stackalloc int[QoiCodec.HashTableSize];
        
        byte[] pixels = new byte[width * height * channels];
        
        byte r = 0;
        byte g = 0;
        byte b = 0;
        byte a = 255;
        
        int p = QoiCodec.HeaderSize;

        for (int pxPos = 0; pxPos < pixels.Length; pxPos += channels)
        {
            byte b1 = data[p++];

            if (b1 == QoiCodec.Rgb)
            {
                r = data[p];
                g = data[p + 1];
                b = data[p + 2];
                p += 3;
            }
            else if (b1 == QoiCodec.Rgba)
            {
                r = data[p];
                g = data[p + 1];
                b = data[p + 2];
                a = data[p + 3];
                p += 4;
            }
            else if ((b1 & QoiCodec.Mask2) == QoiCodec.Index)
            {
                int indexPos = (b1 & ~QoiCodec.Mask2);
                int value = index[indexPos];
                r = (byte)(value >> 24);
                g = (byte)(value >> 16);
                b = (byte)(value >> 8);
                a = (byte)(value >> 0);
            }
            else if ((b1 & QoiCodec.Mask2) == QoiCodec.Diff)
            {
                r += (byte)(((b1 >> 4) & 0x03) - 2);
                g += (byte)(((b1 >> 2) & 0x03) - 2);
                b += (byte)((b1 & 0x03) - 2);
            }
            else if ((b1 & QoiCodec.Mask2) == QoiCodec.Luma) 
            {
                int b2 = data[p++];
                int vg = (b1 & 0x3F) - 32;
                r += (byte)(vg - 8 + ((b2 >> 4) & 0x0F));
                g += (byte)vg;
                b += (byte)(vg - 8 + (b2 & 0x0F));
            }
            else if ((b1 & QoiCodec.Mask2) == QoiCodec.Run) 
            {
                int run = b1 & 0x3F;
                int end = pxPos + run * channels;
                while (pxPos < end) 
                {
                    pixels[pxPos] = r;
                    pixels[pxPos + 1] = g;
                    pixels[pxPos + 2] = b;
                    if (channels == 4) 
                        pixels[pxPos + 3] = a;
                    pxPos += channels;
                }
            }
            
            int indexPos2 = 
                (r * 3 + 
                g * 5 + 
                b * 7 + 
                a * 11) % QoiCodec.HashTableSize;
            index[indexPos2] = r << 24 | g << 16 | b << 8 | a;

            pixels[pxPos] = r;
            pixels[pxPos + 1] = g;
            pixels[pxPos + 2] = b;

            if (channels == 4)
                pixels[pxPos + 3] = a;
        }
        
        SkyLog.Assert(
            QoiCodec.ReadOnlyPadding.Span.SequenceEqual(data.Slice(p, QoiCodec.ReadOnlyPadding.Length)), 
            "Invalid Padding");

        return new QoiImage(pixels, width, height, (Channels)channels, colorSpace);
    }
}