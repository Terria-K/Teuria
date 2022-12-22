using System.Collections.Generic;
using Name =
#if SYSTEMTEXTJSON
System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Text.Json.Serialization;
#else
Newtonsoft.Json.JsonPropertyAttribute;
using Newtonsoft.Json;
#endif

namespace Teuria;

public readonly ref struct SpriteFrameLoader 
{
    public TextureAtlas Atlas { get; init; }
    public SpriteTexture Texture { get; init; }
    public Vector2 Size { get; init; }
    public Dictionary<string, SFCyclesFrame> CycleFrame { get; init; }
    
    public SpriteFrameLoader(string contentTexturePath, ContentManager content) 
    {
        using var fs = new FileStream("Content/" + contentTexturePath + ".sf", FileMode.Open, FileAccess.Read);

#if SYSTEMTEXTJSON
        var result = JsonSerializer.Deserialize<SpriteFactory>(fs, Loader_SpriteFactory.Default.SpriteFactory);
#else
        using var sr = new StreamReader(fs);
        using var jst = new JsonTextReader(sr);
        var serializer = new JsonSerializer();
        var result = serializer.Deserialize<SpriteFactory>(jst);
#endif   
        Texture = SpriteTexture.FromContent(content, contentTexturePath);
        var atlas = result.SFAtlas;
        Atlas = new TextureAtlas(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }

    public SpriteFrameLoader(string sfPath, SpriteTexture texture) 
    {
        using var fs = new FileStream("Content/" + sfPath + ".sf", FileMode.Open, FileAccess.Read);

#if SYSTEMTEXTJSON
        var result = JsonSerializer.Deserialize<SpriteFactory>(fs, Loader_SpriteFactory.Default.SpriteFactory);
#else
        using var sr = new StreamReader(fs);
        using var jst = new JsonTextReader(sr);
        var serializer = new JsonSerializer();
        var result = serializer.Deserialize<SpriteFactory>(jst);
#endif   
        var atlas = result.SFAtlas;
        Texture = texture;
        Atlas = new TextureAtlas(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }
}

internal struct SpriteFactory 
{
    [Name("textureAtlas")]
    public SFAtlas SFAtlas { get; set; }
    [Name("cycles")]
    public Dictionary<string, SFCyclesFrame> SFCycles { get; set; }
}

[JsonSerializable(typeof(SpriteFactory))]
internal partial class Loader_SpriteFactory : JsonSerializerContext {}

internal struct SFAtlas 
{
    [Name("texture")]
    public string Texture { get; set; }
    [Name("regionWidth")]
    public int RegionWidth { get; set; }
    [Name("regionHeight")]
    public int RegionHeight { get; set; }
}

[JsonSerializable(typeof(SFAtlas))]
internal partial class Loader_SFAtlas : JsonSerializerContext {}

public struct SFCyclesFrame 
{
    [Name("frames")]
    public int[] Frames { get; set; }
    [Name("isLooping")]
    public bool IsLooping { get; set; }
}

[JsonSerializable(typeof(SFCyclesFrame))]
internal partial class Loader_SFCyclesFrame : JsonSerializerContext {}