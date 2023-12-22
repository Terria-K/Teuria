using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using TeuJson.Attributes;
using TeuJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public interface IAtlasLoader
{
    Dictionary<string, SpriteTexture> Load(Stream fs, Texture2D baseTexture);
}

public delegate Dictionary<string, SpriteTexture> AtlasDelegate(Stream fs, Texture2D baseTexture);

public class Atlas 
{
    public enum Extension { Json, Bin }
    private Dictionary<string, SpriteTexture> sprites = new Dictionary<string, SpriteTexture>();
    public Texture2D? BaseTexture;

    public SpriteTexture this[string id] 
    {
        get 
        {
            if (sprites.ContainsKey(id)) 
            {
                return sprites[id];
            }
            throw new Exception($"{id} is not in the atlas.");
        }
    }

    public Atlas(Stream fs, Texture2D baseTexture, IAtlasLoader loader) 
    {
        sprites = loader.Load(fs, baseTexture);
        BaseTexture = baseTexture;
    }

    public Atlas(string path, Texture2D baseTexture, IAtlasLoader loader) 
    {
        using var tc = TitleContainer.OpenStream(path);
        sprites = loader.Load(tc, baseTexture);
        BaseTexture = baseTexture;
    }

    public Atlas(Stream fs, Texture2D baseTexture, AtlasDelegate func) 
    {
        sprites = func(fs, baseTexture);
        BaseTexture = baseTexture;
    }

    public Atlas(string path, Texture2D baseTexture, AtlasDelegate func) 
    {
        using var tc = TitleContainer.OpenStream(path);
        sprites = func(tc, baseTexture);
        BaseTexture = baseTexture;
    }

    public static Atlas LoadPng(string path, IAtlasLoader loader, Extension extension = Extension.Json) 
    {
        var atlasExtension = extension switch 
        {
            Extension.Bin => ".bin",
            Extension.Json => ".json",
            _ => throw new InvalidOperationException()
        };
        var baseTexture = TeuriaImporter.LoadImage(path + ".png");
        using var tc = TitleContainer.OpenStream(path + atlasExtension);
        return new Atlas(tc, baseTexture, loader);
    }

    public static Atlas LoadQoi(string path, IAtlasLoader loader, Extension extension = Extension.Json) 
    {
        var atlasExtension = extension switch 
        {
            Extension.Bin => ".bin",
            Extension.Json => ".json",
            _ => throw new InvalidOperationException()
        };
        var baseTexture = TeuriaImporter.LoadImage(path + ".qoi");
        using var tc = TitleContainer.OpenStream(path + atlasExtension);
        return new Atlas(tc, baseTexture, loader);
    }

    public bool Contains(string id) => sprites.ContainsKey(id);
    public SpriteTexture GetTexture(string id) => sprites[id];

    public SpriteTexture? GetTextureOrNull(string id) 
    {
        var texture = CollectionsMarshal.GetValueRefOrNullRef(sprites, id);
        if (System.Runtime.CompilerServices.Unsafe.IsNullRef(ref texture)) 
            return null;

        return texture;
    }
    
    public unsafe SpriteTexture GetTextureUnchecked(string id) 
    {
        return CollectionsMarshal.GetValueRefOrNullRef(sprites, id);
    }
    public bool TryGetTexture(string id, out SpriteTexture? texture) 
    {
        if (sprites.TryGetValue(id, out texture)) 
        {
            return true;
        }
        return false;
    }
}
/// <summary>
/// Binary Format Example
/// <example>
/// <code>
///    [UInt32] - Texture Count
///      L [String] - Name
///        [UInt32] - X
///        [UInt32] - Y
///        [UInt32] - W
///        [UInt32] - H
/// </code>
/// </example>
/// </summary>
public sealed class ClutterBinaryLoader : IAtlasLoader
{
    public required bool NinePatchEnabled;
    public Dictionary<string, SpriteTexture> Load(Stream fs, Texture2D baseTexture)
    {
        var reader = new BinaryReader(fs);
        reader.ReadString();
        var length = reader.ReadUInt32();
        var atlas = new Dictionary<string, SpriteTexture>();
        for (uint i = 0; i < length; i++) 
        {
            var name = reader.ReadString();
            var x = (int)reader.ReadUInt32();
            var y = (int)reader.ReadUInt32();
            var w = (int)reader.ReadUInt32();
            var h = (int)reader.ReadUInt32();
            if (!NinePatchEnabled) 
            {
                var spriteTexture = new SpriteTexture(
                    baseTexture,
                    new Point(x, y),
                    w, h
                );
                atlas.Add(name, spriteTexture);
                continue;
            }
            var hasNinePatch = reader.ReadBoolean();
            SpriteTexture ninePatchTexture;
            if (!hasNinePatch) 
            {
                ninePatchTexture = new SpriteTexture(
                    baseTexture,
                    new Point(x, y),
                    w, h
                );
            }
            else 
            {
                var nx = (int)reader.ReadUInt32();
                var ny = (int)reader.ReadUInt32();
                var nw = (int)reader.ReadUInt32();
                var nh = (int)reader.ReadUInt32();
                ninePatchTexture = new SpriteTexture(
                    baseTexture,
                    new Point(x, y),
                    w, h,
                    new Rectangle(nx, ny, nw, nh)
                );
            }

            atlas.Add(name, ninePatchTexture);
        }
        return atlas;
    }
}

/// <summary>
/// Json Format Example
/// <example>
/// <code>
/// {
///  "frames": {
///         [Key]: {
///             "x": [number]
///             "y": [number]
///             "width": [number]
///             "height": [number]
///         }
///     }
/// }
/// </code>
/// </example>
/// </summary>
public sealed class ClutterJsonLoader: IAtlasLoader
{
    public Dictionary<string, SpriteTexture> Load(Stream fs, Texture2D baseTexture)
    {
        var val = JsonTextReader.FromStream(fs);
        var frames = val["frames"].AsJsonObject;
        var atlas = new Dictionary<string, SpriteTexture>();
        foreach (var keyValue in frames.Pairs) 
        {
            var name = keyValue.Key;
            var x = keyValue.Value["x"].AsInt32;
            var y = keyValue.Value["y"].AsInt32;
            var w = keyValue.Value["width"].AsInt32;
            var h = keyValue.Value["height"].AsInt32;
            if (!keyValue.Value.Contains("nine_patch")) 
            {
                var spriteTexture = new SpriteTexture(
                    baseTexture,
                    new Point(x, y),
                    w, h
                );
                atlas.Add(name, spriteTexture);
                continue;
            }
            var ninePatch = keyValue.Value["nine_patch"].AsJsonObject;
            var nx = ninePatch["x"].AsInt32;
            var ny = ninePatch["y"].AsInt32;
            var nw = ninePatch["w"].AsInt32;
            var nh = ninePatch["h"].AsInt32;

            var ninePatchTexture = new SpriteTexture(
                baseTexture,
                new Point(x, y),
                w, h,
                new Rectangle(nx, ny, nw, nh)
            );
            atlas.Add(name, ninePatchTexture);
        }

        return atlas;
    }
}