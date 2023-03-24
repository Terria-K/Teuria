using System.IO;
using System.Collections.Generic;
using System.Linq;
using TeuJson;
using TeuJson.Attributes;
using Microsoft.Xna.Framework;

namespace Teuria;


public class Tileset 
{
    public List<Terrain> Terrains = new List<Terrain>();
    public Spritesheet Sheet;
    public int Width { get; private set; }
    public int Height { get; private set ; }

    private Tileset(string path, SpriteTexture texture) 
    {
        using var fs = TitleContainer.OpenStream(path);
        var result = JsonConvert.DeserializeFromStream<TeuriaTileset>(fs);

        var textureAtlas = new Spritesheet(texture, result.Width, result.Height);
        Sheet = textureAtlas;
        Width = result.Width;
        Height = result.Height;
        var terrain = CreateTerrain(result);
        Terrains.Add(terrain);
    }

    private Tileset(SpriteTexture texture, int width, int height) 
    {
        Sheet = new Spritesheet(texture, width, height);
        Width = width;
        Height = height;
    }

    private void AddToList(string path, SpriteTexture texture) 
    {
        using var fs = TitleContainer.OpenStream(path);
        var result = JsonConvert.DeserializeFromStream<TeuriaTileset>(fs);

        Sheet = new Spritesheet(texture, result.Width, result.Height);
        Width = result.Width;
        Height = result.Height;
        var terrain = CreateTerrain(result);
        Terrains.Add(terrain);
    }

    private Terrain CreateTerrain(TeuriaTileset result) 
    {
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
                rule.Textures.Add(Sheet[tile[j, 0] - 1, tile[j, 1] - 1]);
            }
            terrain.RulesList.Add(rule);
        }
        return terrain;
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
        var dict = new Dictionary<byte, Rules>();
        var terrain = Terrains.Where(x => x.Name == terrainName).First();

        foreach (var rule in terrain.RulesList) 
        {
            byte bit = 0;
            bit += (byte)(rule.Mask[0] * 1 << 0);
            bit += (byte)(rule.Mask[1] * 1 << 1);
            bit += (byte)(rule.Mask[2] * 1 << 2);
            bit += (byte)(rule.Mask[3] * 1 << 3);
            bit += (byte)(rule.Mask[5] * 1 << 4);
            bit += (byte)(rule.Mask[6] * 1 << 5);
            bit += (byte)(rule.Mask[7] * 1 << 6);
            bit += (byte)(rule.Mask[8] * 1 << 7);
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

partial struct TeuriaTileset : IDeserialize
{
    [Name("name")]
    public string Name { get; set; }
    [Name("path")]
    public string Path { get; set; }
    [Name("rules")]
    public TeuriaRules[] Rules { get; set; }
    [Name("width")]
    public int Width { get; set; }
    [Name("height")]
    public int Height { get; set; }
}

partial struct TeuriaRules : IDeserialize
{
    [Name("name")]
    public string Name { get; set; }
    [Name("mask")]
    public int[] Mask { get; set; }
    [Name("tiles")]
    public int[,] Tiles { get; set; }
    [Name("maskType")]
    public string MaskType { get; set; }
}
