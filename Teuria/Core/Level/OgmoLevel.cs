using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Name =
#if SYSTEMTEXTJSON
System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json;
using System.Text.Json.Serialization;
#else
Newtonsoft.Json.JsonPropertyAttribute;
using Newtonsoft.Json;
#endif

namespace Teuria.Level;

public class OgmoLevel 
{
    public OgmoLevelData LevelData { get; private set; }
    public Point LevelSize { get; private set; }
    public Point TileSize { get; private set; }
    public Point LevelPixelSize { get; private set; }

    private OgmoLevel(FileStream fs) 
    {
        var result = JsonSerializer.Deserialize<OgmoLevelData>(fs, Loader_OgmoLevelData.Default.OgmoLevelData);

        LevelData = result;

        var firstLayer = result.Layers[0];
        LevelSize = new Point(firstLayer.GridCellsX, firstLayer.GridCellsY);
        TileSize = new Point(firstLayer.GridCellWidth, firstLayer.GridCellHeight);
        LevelPixelSize = LevelSize * TileSize;
    }

    public static OgmoLevel LoadLevel(string levelPath) 
    {
        if (!levelPath.EndsWith(".json")) 
        {
            levelPath += ".json";
        }
#if DEBUG
        try 
        {
#endif
            using var fs = new FileStream(levelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return new OgmoLevel(fs);
#if DEBUG
        } catch (IOException) 
        {
            return null;
        }
#endif

    }
}

public class OgmoLevelData
{
    [Name("width")]
    public int Width { get; set; }
    [Name("height")]
    public int Height { get; set; }
    [Name("offsetX")]
    public int OffsetX { get; set; }
    [Name("offsetY")]
    public int OffsetY { get; set; }
    [Name("layers")]
    public OgmoLayer[] Layers { get; set; }
    
}

[JsonSerializable(typeof(OgmoLevelData))]
internal partial class Loader_OgmoLevelData : JsonSerializerContext {}

public class OgmoLayer 
{
    [Name("name")]
    public string Name { get; set; }
    [Name("offsetX")]
    public int OffsetX { get; set; }
    [Name("offsetY")]
    public int OffsetY { get; set; }
    [Name("gridCellWidth")]
    public int GridCellWidth { get; set; }
    [Name("gridCellHeight")]
    public int GridCellHeight { get; set; }
    [Name("gridCellsX")]
    public int GridCellsX { get; set; }
    [Name("gridCellsY")]
    public int GridCellsY { get; set; }
    [Name("tileset")]
    public string Tileset { get; set; }
#if !SYSTEMTEXTJSON
    [Name("data2D")]
    public int[,] Data { get; set; }
    [Name("grid2D")]
    public string[,] Grid2D { get; set; }
#else
    [Name("data2D")]
    public int[][] Data { get; set; }
    [Name("grid2D")]
    public string[][] Grid2D { get; set; }
#endif
    [Name("grid")]
    public string[] Grid { get; set; }

    [Name("entities")]
    public OgmoEntity[] Entities { get; set; }
}

[JsonSerializable(typeof(OgmoLayer))]
internal partial class Loader_OgmoLayer : JsonSerializerContext {}

public class OgmoNode 
{
    [Name("x")]
    public float X { get; set; }
    [Name("y")]
    public float Y { get; set; }

    public Vector2 ToVector2() 
    {
        return new Vector2(X, Y);
    }
}

[JsonSerializable(typeof(OgmoNode))]
internal partial class Loader_OgmoNode : JsonSerializerContext {}

public class OgmoEntity
{
    [Name("name")]
    public string Name { get; set; }
    [Name("id")]
    public int ID { get; set; }
    [Name("x")]
    public int X { get; set; }
    [Name("y")]
    public int Y { get; set; }
    [Name("originX")]
    public int OriginX { get; set; }
    [Name("originY")]
    public int OriginY { get; set; }
    [Name("width")]
    public int Width { get; set; }
    [Name("height")]
    public int Height { get; set; }
    [Name("nodes")]
    public OgmoNode[] Nodes { get; set; }
    [Name("values")]
    public Dictionary<string, JsonElement> Values { get; set; }
    [JsonIgnore]
    public Vector2 Position => new Vector2(X, Y);
    [JsonIgnore]
    public Rectangle Size => new Rectangle(OriginX, OriginY, Width, Height);

    public int GetValueInt(string valueName) 
    {
        return Values[valueName].GetInt32();
    }

    public bool GetValueBoolean(string valueName) 
    {
        return Values[valueName].GetBoolean();
    }

    public float GetValueFloat(string valueName) 
    {
        return Values[valueName].GetSingle();
    }

    public Vector2 GetValueVector2(string x, string y) 
    {
        return new Vector2(Values[x].GetSingle(), Values[y].GetSingle());
    }

    public string GetValueString(string valueName) 
    {
        return Values[valueName].GetString();
    }
}

[JsonSerializable(typeof(OgmoEntity))]
internal partial class Loader_OgmoEntity : JsonSerializerContext {}
