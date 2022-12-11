using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using Name =
#if SYSTEMTEXTJSON
System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json;
using Microsoft.Xna.Framework;
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

    private Tileset(FileStream fs, ContentManager manager, SpriteTexture texture) 
    {
#if SYSTEMTEXTJSON
        var result = JsonSerializer.Deserialize<TeuriaTileset>(fs);
#else
        using var sr = new StreamReader(fs);
        using var jst = new JsonTextReader(sr);
        var serializer = new JsonSerializer();
        var result = serializer.Deserialize<OgmoLevelData>(jst);
#endif
        var textureAtlas = new TextureAtlas(texture == null 
            ? SpriteTexture.FromContent(manager, result.Path)
            : texture, result.Width, result.Height);
        TilesetAtlas = textureAtlas;
        Width = result.Width;
        Height = result.Height;

        for (int i = 0; i < result.Rules.Length; i++) 
        {
            var rule = new Rules();
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
                rule.TextureLocation.Add(new Vector2(tile[j, 0] - 1, tile[j, 1] - 1));
            }
            rules.Add(rule);
        }
    }

    public static Tileset LoadTileset(string tilesetPath, ContentManager manager, SpriteTexture texture = null) 
    {
        var path = Path.Join(TeuriaEngine.ContentPath, tilesetPath);
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        return new Tileset(fs, manager, texture);
    }

    public Dictionary<byte, List<Vector2>> InitializeTerrain() 
    {
#if NET5_0_OR_GREATER
        ReadOnlySpan<int> directionalValues = stackalloc int[9]     
#else 
        int[] directionalValues = new int[9]     
#endif
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