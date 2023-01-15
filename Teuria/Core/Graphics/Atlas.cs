using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LightJson;
using LightJson.Serialization;
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


