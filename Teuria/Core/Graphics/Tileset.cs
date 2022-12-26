using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using Name =
#if SYSTEMTEXTJSON
System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json;
using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;
#else
Newtonsoft.Json.JsonPropertyAttribute;
using Newtonsoft.Json;
#endif

namespace Teuria;

public class Tileset 
{
    public List<Terrain> Terrains = new List<Terrain>();
    public TextureAtlas TilesetAtlas;
    public int Width { get; private set; }
    public int Height { get; private set ; }
    private List<Rules> rules = new List<Rules>();    

    private Tileset(FileStream fs, ContentManager manager, SpriteTexture texture) 
    {
        AddToList(fs, texture, manager);
    }

    private Tileset(SpriteTexture texture, int width, int height) 
    {
        TilesetAtlas = new TextureAtlas(texture, width, height);
        Width = width;
        Height = height;
    }

    private void AddToList(FileStream fs, SpriteTexture texture, ContentManager manager) 
    {
        var result = JsonSerializer.Deserialize<TeuriaTileset>(fs, Loader_TeuriaTileset.Default.TeuriaTileset);

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
            var tile =
            teuriaRule.Tiles.To2D();

            if (teuriaRule.Mask == null) { continue; }
            for (int mask = 0; mask < teuriaRule.Mask.Length; mask++) 
            {
                rule.Mask[mask] = ((byte)teuriaRule.Mask[mask]);
            }
            for (int j = 0; j < tile.GetLength(0); j++) 
            {
                rule.Textures.Add(TilesetAtlas[tile[j, 0] - 1, tile[j, 1] - 1]);
                rule.TextureLocation.Add(new Vector2(tile[j, 0] - 1, tile[j, 1] - 1));
            }
            rules.Add(rule);
            terrain.RulesList.Add(rule);
        }
        Terrains.Add(terrain);
    }

    public void AddTerrain(string tilesetPath, SpriteTexture texture) 
    {
        var path = Path.Join(TeuriaEngine.ContentPath, tilesetPath);
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        AddToList(fs, texture, null);
    }

    public static Tileset LoadTileset(string tilesetPath, ContentManager manager, SpriteTexture texture = null) 
    {
        var path = Path.Join(TeuriaEngine.ContentPath, tilesetPath);
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return new Tileset(fs, manager, texture);
    }

    public static Tileset LoadTileset(SpriteTexture texture, int width, int height) 
    {
        return new Tileset(texture, width, height);
    }

    public Dictionary<byte, List<Vector2>> InitializeTerrain() 
    {
        ReadOnlySpan<int> directionalValues = stackalloc int[9]     
        {
            0x001, 0x002, 0x004,
            0x008, 0x000, 0x010,
            0x020, 0x040, 0x080,
        };

        var dict = new Dictionary<byte, List<Vector2>>();
        foreach (var rule in rules) 
        {
            byte bit = 0;
            for (int i = 0; i < rule.Mask.Length; i++) 
            {
                var mask = rule.Mask[i];
                bit += (byte)((int)mask * directionalValues[i]);
            }
            dict.Add(bit, rule.TextureLocation);
        }
        return dict;
    }

    public class Rules 
    {
        public byte[] Mask = new byte[9];
        public List<Vector2> TextureLocation = new List<Vector2>();
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

internal struct TeuriaTileset 
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

[JsonSerializable(typeof(TeuriaTileset))]
internal partial class Loader_TeuriaTileset : JsonSerializerContext {}

internal struct TeuriaRules 
{
    [Name("name")]
    public string Name { get; set; }
    [Name("mask")]
    public int[] Mask { get; set; }
#if !SYSTEMTEXTJSON
    [Name("tiles")]
    public int[,] Tiles { get; set; }
#else
    [Name("tiles")]
    public int[][] Tiles { get; set; }
#endif
    [Name("maskType")]
    public string MaskType { get; set; }
}

[JsonSerializable(typeof(TeuriaRules))]
internal partial class Loader_TeuriaRules : JsonSerializerContext {}