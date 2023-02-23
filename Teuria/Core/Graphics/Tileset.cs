using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using LightJson;
using LightJson.Serialization;
using Microsoft.Xna.Framework;

namespace Teuria;


public class Tileset 
{
    public List<Terrain> Terrains = new List<Terrain>();
    public TextureAtlas? TilesetAtlas;
    public int Width { get; private set; }
    public int Height { get; private set ; }
    private List<Rules> rules = new List<Rules>();    

    private Tileset(string path, SpriteTexture texture) 
    {
        AddToList(path, texture);
    }

    private Tileset(SpriteTexture texture, int width, int height) 
    {
        TilesetAtlas = new TextureAtlas(texture, width, height);
        Width = width;
        Height = height;
    }

    private void AddToList(string path, SpriteTexture texture) 
    {
        using var fs = TitleContainer.OpenStream(path);
        var result = JsonConvert.DeserializeFromStream<TeuriaTileset>(fs);

        var textureAtlas = new TextureAtlas(texture, result.Width, result.Height);
        TilesetAtlas = textureAtlas;
        Width = result.Width;
        Height = result.Height;
        var terrain = new Terrain(result.Name);

        for (int i = 0; i < result.Rules.Length; i++) 
        {
            var rule = new Rules();
            var teuriaRule = result.Rules[i];
            var tile = teuriaRule.Tiles;

            if (teuriaRule.Mask == null) { continue; }
            for (int mask = 0; mask < teuriaRule.Mask.Length; mask++) 
            {
                rule.Mask[mask] = ((byte)teuriaRule.Mask[mask]);
            }
            for (int j = 0; j < tile.GetLength(0); j++) 
            {
                rule.Textures.Add(TilesetAtlas[tile[j, 0] - 1, tile[j, 1] - 1]);
            }
            rules.Add(rule);
            terrain.RulesList.Add(rule);
        }
        Terrains.Add(terrain);
    }

    public void AddTerrain(string tilesetPath, SpriteTexture texture) 
    {
        var path = Path.Join(GameApp.ContentPath, tilesetPath);
        AddToList(path, texture);
    }

    public static Tileset LoadTileset(string tilesetPath, SpriteTexture texture) 
    {
        var path = Path.Join(GameApp.ContentPath, tilesetPath);
        return new Tileset(path, texture);
    }

    public static Tileset LoadTileset(SpriteTexture texture, int width, int height) 
    {
        return new Tileset(texture, width, height);
    }

    public Dictionary<byte, Rules> GetTerrainRules(string terrainName) 
    {
        ReadOnlySpan<int> directionalValues = stackalloc int[9]     
        {
            0x001, 0x002, 0x004,
            0x008, 0x000, 0x010,
            0x020, 0x040, 0x080,
        };
        var dict = new Dictionary<byte, Rules>();
        var terrain = Terrains.Where(x => x.Name == terrainName).First();
        foreach (var rule in terrain.RulesList) 
        {
            byte bit = 0;
            for (int i = 0; i < rule.Mask.Length; i++) 
            {
                var mask = rule.Mask[i];
                bit += (byte)((int)mask * directionalValues[i]);
            }
            dict.Add(bit, rule);
        }
        return dict;
    }

    public class Rules 
    {
        public byte[] Mask = new byte[9];
        public List<SpriteTexture> Textures = new List<SpriteTexture>();
    }

    public class Terrain 
    {
        public string Name;
        public List<Rules> RulesList = new List<Rules>();

        public Terrain(string name) 
        {
            Name = name;
        }
    }
}

[JsonSerializable]
partial struct TeuriaTileset
{
    [JName("name")]
    public string Name { get; set; }
    [JName("path")]
    public string Path { get; set; }
    [JName("rules")]
    [JArray(SupportedTypes.Other)]
    public TeuriaRules[] Rules { get; set; }
    [JName("width")]
    public int Width { get; set; }
    [JName("height")]
    public int Height { get; set; }
}

[JsonSerializable]
partial struct TeuriaRules 
{
    [JName("name")]
    public string Name { get; set; }
    [JName("mask")]
    [JArray(SupportedTypes.Int)]
    public int[] Mask { get; set; }
    [JName("tiles")]
    [JArray(SupportedTypes.Int2D)]
    public int[,] Tiles { get; set; }
    [JName("maskType")]
    public string MaskType { get; set; }
}
