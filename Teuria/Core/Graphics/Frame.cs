using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TeuJson;
using TeuJson.Attributes;


namespace Teuria;

public readonly ref struct SpriteFrameLoader 
{
    internal Spritesheet Sheet { get; init; }
    internal SpriteTexture Texture { get; init; }
    internal Vector2 Size { get; init; }
    internal Dictionary<string, SFCyclesFrame> CycleFrame { get; init; }
    
    public SpriteFrameLoader(string contentTexturePath, ContentManager content) 
    {
        using var fs = TitleContainer.OpenStream($"Content/{contentTexturePath}.sf");
        var result = JsonConvert.DeserializeFromStream<SpriteFactory>(fs);

        Texture = SpriteTexture.FromContent(content, contentTexturePath);
        var atlas = result.SFAtlas;
        Sheet = new Spritesheet(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }

    public SpriteFrameLoader(string sfPath, SpriteTexture texture) 
    {
        using var fs = TitleContainer.OpenStream($"Content/{sfPath}.sf");
        var result = JsonConvert.DeserializeFromStream<SpriteFactory>(fs);
        
        var atlas = result.SFAtlas;
        Texture = texture;
        Sheet = new Spritesheet(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }
}

[TeuJsonSerializable(Deserializable = true)]
internal partial struct SpriteFactory
{
    [Name("textureAtlas")]
    public SFAtlas SFAtlas { get; set; }
    [Name("cycles")]
    public Dictionary<string, SFCyclesFrame> SFCycles { get; set; }
}

[TeuJsonSerializable(Deserializable = true)]
internal partial struct SFAtlas 
{
    [Name("texture")]
    public string Texture { get; set; }
    [Name("regionWidth")]
    public int RegionWidth { get; set; }
    [Name("regionHeight")]
    public int RegionHeight { get; set; }
}

[TeuJsonSerializable(Deserializable = true)]
internal partial struct SFCyclesFrame 
{
    [Name("frames")]
    public int[] Frames { get; set; }
    [Name("isLooping")]
    public bool IsLooping { get; set; }
}