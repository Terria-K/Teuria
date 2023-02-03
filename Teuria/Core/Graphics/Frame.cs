using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LightJson;
using LightJson.Serialization;


namespace Teuria;

public readonly ref struct SpriteFrameLoader 
{
    internal TextureAtlas Atlas { get; init; }
    internal SpriteTexture Texture { get; init; }
    internal Vector2 Size { get; init; }
    internal Dictionary<string, SFCyclesFrame> CycleFrame { get; init; }
    
    public SpriteFrameLoader(string contentTexturePath, ContentManager content) 
    {
        using var fs = TitleContainer.OpenStream($"Content/{contentTexturePath}.sf");
        var result = JsonConvert.DeserializeFromStream<SpriteFactory>(fs);

        Texture = SpriteTexture.FromContent(content, contentTexturePath);
        var atlas = result.SFAtlas;
        Atlas = new TextureAtlas(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }

    public SpriteFrameLoader(string sfPath, SpriteTexture texture) 
    {
        using var fs = TitleContainer.OpenStream($"Content/{sfPath}.sf");
        var result = JsonConvert.DeserializeFromStream<SpriteFactory>(fs);
        
        var atlas = result.SFAtlas;
        Texture = texture;
        Atlas = new TextureAtlas(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }
}

[JsonSerializable]
internal partial struct SpriteFactory
{
    [JName("textureAtlas")]
    public SFAtlas SFAtlas { get; set; }
    [JName("cycles")]
    [JDictionary()]
    public Dictionary<string, SFCyclesFrame> SFCycles { get; set; }
}

[JsonSerializable]
internal partial struct SFAtlas 
{
    [JName("texture")]
    public string Texture { get; set; }
    [JName("regionWidth")]
    public int RegionWidth { get; set; }
    [JName("regionHeight")]
    public int RegionHeight { get; set; }
}

[JsonSerializable]
internal partial struct SFCyclesFrame 
{
    [JName("frames")]
    [JArray(SupportedTypes.Int)]
    public int[] Frames { get; set; }
    [JName("isLooping")]
    public bool IsLooping { get; set; }
}