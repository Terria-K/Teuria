using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LightJson.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public interface IAtlasLoader
{
    Dictionary<string, SpriteTexture> Load(FileStream fs, Texture2D baseTexture);
}


public class Atlas 
{
    private Dictionary<string, SpriteTexture> sprites;
    public Texture2D BaseTexture;
    private Atlas() {}

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

    public static Atlas Create(FileStream fs, Texture2D baseTexture, IAtlasLoader loader) 
    {
        var atl = new Atlas() {
            sprites = loader.Load(fs, baseTexture),
            BaseTexture = baseTexture
        };
        return atl;
    }

    public static Atlas Create(string path, Texture2D baseTexture, IAtlasLoader loader) 
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return Create(fs, baseTexture, loader);
    }

    public bool Contains(string id) => sprites.ContainsKey(id);
    public SpriteTexture GetTexture(string id) => sprites[id];
    
    public SpriteTexture GetTextureUnchecked(string id) 
    {
        return CollectionsMarshal.GetValueRefOrNullRef(sprites, id);
    }
    public bool TryGetTexture(string id, out SpriteTexture texture) 
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
    public Dictionary<string, SpriteTexture> Load(FileStream fs, Texture2D baseTexture)
    {
        var reader = new BinaryReader(fs);
        var length = reader.ReadUInt32();
        var atlas = new Dictionary<string, SpriteTexture>();
        for (uint i = 0; i < length; i++) 
        {
            var name = reader.ReadString();
            var x = (int)reader.ReadUInt32();
            var y = (int)reader.ReadUInt32();
            var w = (int)reader.ReadUInt32();
            var h = (int)reader.ReadUInt32();
            var spriteTexture = new SpriteTexture(
                baseTexture,
                new Point(x, y),
                w, h,
                new Rectangle(4, 4, 4, 4)
            );
            atlas.Add(name, spriteTexture);
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
///             "w": [number]
///             "h": [number]
///         }
///     }
/// }
/// </code>
/// </example>
/// </summary>
public sealed class ClutterJsonLoader: IAtlasLoader
{
    public Dictionary<string, SpriteTexture> Load(FileStream fs, Texture2D baseTexture)
    {
        var val = JsonTextReader.ParseFile(fs);
        var frames = val["frames"].AsJsonObject;
        var atlas = new Dictionary<string, SpriteTexture>();
        foreach (var keyValue in frames) 
        {
            var name = keyValue.Key;
            var x = keyValue.Value["x"].AsInteger;
            var y = keyValue.Value["y"].AsInteger;
            var w = keyValue.Value["width"].AsInteger;
            var h = keyValue.Value["height"].AsInteger;
            var spriteTexture = new SpriteTexture(
                baseTexture,
                new Point(x, y),
                w, h,
                new Rectangle(4, 4, 4, 4)
            );
            atlas.Add(name, spriteTexture);
        }

        return atlas;
    }
}