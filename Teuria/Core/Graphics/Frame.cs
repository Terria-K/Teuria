using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LightJson;


namespace Teuria;

public readonly ref struct SpriteFrameLoader 
{
    internal TextureAtlas Atlas { get; init; }
    internal SpriteTexture Texture { get; init; }
    internal Vector2 Size { get; init; }
    internal Dictionary<string, SFCyclesFrame> CycleFrame { get; init; }
    
    public SpriteFrameLoader(string contentTexturePath, ContentManager content) 
    {
        var result = JsonConvert.DeserializeFromFile<SpriteFactory>($"Content/{contentTexturePath}.sf");

        Texture = SpriteTexture.FromContent(content, contentTexturePath);
        var atlas = result.SFAtlas;
        Atlas = new TextureAtlas(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }

    public SpriteFrameLoader(string sfPath, SpriteTexture texture) 
    {
        var result = JsonConvert.DeserializeFromFile<SpriteFactory>($"Content/{sfPath}.sf");
        
        var atlas = result.SFAtlas;
        Texture = texture;
        Atlas = new TextureAtlas(Texture, atlas.RegionWidth, atlas.RegionHeight);
        Size = new Vector2(atlas.RegionWidth, atlas.RegionHeight);
        CycleFrame = result.SFCycles;
    }
}

internal struct SpriteFactory : IJsonDeserializable
{
    public SFAtlas SFAtlas { get; set; }
    public Dictionary<string, SFCyclesFrame> SFCycles { get; set; }

    public void Deserialize(JsonObject obj)
    {
        SFAtlas = JsonConvert.Deserialize<SFAtlas>(obj["textureAtlas"]);       
        SFCycles = obj["cycles"].ToDictionary<SFCyclesFrame>();
    }
}

internal struct SFAtlas : IJsonDeserializable
{
    public string Texture { get; set; }
    public int RegionWidth { get; set; }
    public int RegionHeight { get; set; }

    public void Deserialize(JsonObject obj)
    {
        Texture = obj["texture"];
        RegionWidth = obj["regionWidth"];
        RegionHeight = obj["regionHeight"];
    }
}

internal struct SFCyclesFrame : IJsonDeserializable
{
    public int[] Frames { get; set; }
    public bool IsLooping { get; set; }

    public void Deserialize(JsonObject obj)
    {
        Frames = obj["frames"].ConvertToArrayInt();
        IsLooping = obj["isLooping"];
    }
}