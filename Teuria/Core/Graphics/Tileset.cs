using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using System.Linq;
using LightJson;

namespace Teuria;


public class Tileset 
{
    public List<Terrain> Terrains = new List<Terrain>();
    public TextureAtlas TilesetAtlas;
    public int Width { get; private set; }
    public int Height { get; private set ; }
    private List<Rules> rules = new List<Rules>();    

    private Tileset(string path, ContentManager manager, SpriteTexture texture) 
    {
        AddToList(path, texture, manager);
    }

    private Tileset(SpriteTexture texture, int width, int height) 
    {
        TilesetAtlas = new TextureAtlas(texture, width, height);
        Width = width;
        Height = height;
    }

    private void AddToList(string path, SpriteTexture texture, ContentManager manager) 
    {
        var result = JsonConvert.DeserializeFromFile<TeuriaTileset>(path);

        var textureAtlas = new TextureAtlas(texture == null 
            ? SpriteTexture.FromContent(manager, result.Path)
            : texture, result.Width, result.Height);
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
        var path = Path.Join(TeuriaEngine.ContentPath, tilesetPath);
        AddToList(path, texture, null);
    }

    public static Tileset LoadTileset(string tilesetPath, ContentManager manager, SpriteTexture texture = null) 
    {
        var path = Path.Join(TeuriaEngine.ContentPath, tilesetPath);
        return new Tileset(path, manager, texture);
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

internal struct TeuriaTileset : IJsonDeserializable
{
    public string Name { get; set; }
    public string Path { get; set; }
    public TeuriaRules[] Rules { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public void Deserialize(JsonObject obj)
    {
        Name = obj["name"];
        Path = obj["path"];
        Rules = obj["rules"].ConvertToArray<TeuriaRules>();
        Width = obj["width"];
        Height = obj["height"];
    }
}
internal struct TeuriaRules : IJsonDeserializable
{
    public string Name { get; set; }
    public int[] Mask { get; set; }
    public int[,] Tiles { get; set; }
    public string MaskType { get; set; }

    public void Deserialize(JsonObject obj)
    {
        Name = obj["name"];
        Mask = obj["mask"].ConvertToArrayInt();
        Tiles = obj["tiles"].ConvertToArrayInt2D();
        MaskType = obj["maskType"];
    }
}
