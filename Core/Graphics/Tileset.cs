using System.IO;
using Name =
#if SYSTEMTEXTJSON
System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
#else
Newtonsoft.Json.JsonPropertyAttribute;
using Newtonsoft.Json;
#endif

namespace Teuria;

public class Tileset 
{
    public TextureAtlas TilesetAtlas;
    public int Width { get; init; }
    public int Height { get; init; }
    private List<Rules> rules = new List<Rules>();    

    private Tileset(FileStream fs, ContentManager manager) 
    {
#if SYSTEMTEXTJSON
        var result = JsonSerializer.Deserialize<TeuriaTileset>(fs);
#else
        using var sr = new StreamReader(fs);
        using var jst = new JsonTextReader(sr);
        var serializer = new JsonSerializer();
        var result = serializer.Deserialize<OgmoLevelData>(jst);
#endif
        var textureAtlas = new TextureAtlas(SpriteTexture.FromContent(manager, result.Path), result.Width, result.Height);
        TilesetAtlas = textureAtlas;
        Width = result.Width;
        Height = result.Height;


        for (int i = 0; i < result.Rules.Length; i++) 
        {
            Rules rule = new Rules();
            var teuriaRule = result.Rules[i];
            var tile =
#if SYSTEMTEXTJSON
            teuriaRule.Tiles.To2D();
#else
            teuriaRule.Tiles;
#endif
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
        }
        
    }

    public static Tileset LoadTileset(string tilesetPath, ContentManager manager) 
    {
        var path = Path.Join(TeuriaEngine.ContentPath, tilesetPath);
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return new Tileset(fs, manager);
    }

    private Dictionary<byte, List<SpriteTexture>> InitializeTerrain() 
    {
        var dict = new Dictionary<byte, List<SpriteTexture>>();
        foreach (var rule in rules) 
        {
            byte bit = 0;
            for (int i = 0; i < rule.Mask.Length; i++) 
            {
                var mask = rule.Mask[i];
                bit = (byte)(i * (int)mask);
            }
            dict.Add(bit, rule.Textures);
        }
        return dict;
    }

    public class Terrain 
    {
        public char ID;
        public List<Rules> Rules = new List<Rules>();

        public Terrain(char id) 
        {
            ID = id;
        }

    }

    public class Rules 
    {
        public byte[] Mask = new byte[9];
        public List<SpriteTexture> Textures = new List<SpriteTexture>();
    }
}

public struct TeuriaTileset 
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

public struct TeuriaRules 
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