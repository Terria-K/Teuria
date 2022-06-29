using System.Collections.Generic;
using Name =
#if SYSTEMTEXTJSON
System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Content;
#else
Newtonsoft.Json.JsonPropertyAttribute;
using Newtonsoft.Json;
#endif

namespace Teuria;

public class SpriteFrameLoader 
{
    public TextureAtlas Atlas { get; private set; }
    public SpriteTexture Texture { get; private set; }
    public Vector2 Size { get; private set; }
    public Dictionary<string, SFCyclesFrame> CycleFrame { get; private set; }
    
    public SpriteFrameLoader(string spriteFramePath, string texturePath, ContentManager content) 
    {
        using var fs = new FileStream(spriteFramePath, FileMode.Open, FileAccess.Read);

#if SYSTEMTEXTJSON
        var result = JsonSerializer.Deserialize<SpriteFactory>(fs);
#else
        using var sr = new StreamReader(fs);
        using var jst = new JsonTextReader(sr);
        var serializer = new JsonSerializer();
        var result = serializer.Deserialize<SpriteFactory>(jst);
#endif   
        Texture = SpriteTexture.FromContent(content, texturePath);
        var atlas = result.SFAtlas;
        Atlas = new TextureAtlas(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }
}

public struct SpriteFactory 
{
    [Name("textureAtlas")]
    public SFAtlas SFAtlas { get; set; }
    [Name("cycles")]
    public Dictionary<string, SFCyclesFrame> SFCycles { get; set; }
}

public struct SFAtlas 
{
    [Name("texture")]
    public string Texture { get; set; }
    [Name("regionWidth")]
    public int RegionWidth { get; set; }
    [Name("regionHeight")]
    public int RegionHeight { get; set; }
}

public struct SFCyclesFrame 
{
    [Name("frames")]
    public int[] Frames { get; set; }
    [Name("isLooping")]
    public bool IsLooping { get; set; }
}

/**
- Create a texture atlas
- Create a Region row per row
- Create a Frame class
- Get all texture by index
- Animate all texture by index
*/