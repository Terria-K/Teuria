using System.IO;
using Microsoft.Xna.Framework;
using Name = 
#if SYSTEMTEXTJSON
System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json;
#else
Newtonsoft.Json.JsonPropertyAttribute;
using Newtonsoft.Json;
#endif

namespace Teuria.Level;

public class OgmoLevel 
{
    public OgmoLevelData LevelData { get; set; }
    public Point LevelSize { get; private set; }
    public Point TileSize { get; private set; }
    public Point LevelPixelSize { get; private set; }

    public OgmoLevel(FileStream fs) 
    {
#if SYSTEMTEXTJSON
        var result = JsonSerializer.Deserialize<OgmoLevelData>(fs);
#else
        using var sr = new StreamReader(fs);
        using var jst = new JsonTextReader(sr);
        var serializer = new JsonSerializer();
        var result = serializer.Deserialize<OgmoLevelData>(jst);
#endif
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
        using var fs = new FileStream(levelPath, FileMode.Open, FileAccess.Read);
        return new OgmoLevel(fs);
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
    [Name("data2D")]
#if !SYSTEMTEXTJSON
    public int[,] Data { get; set; }
#else
    public int[][] Data { get; set; }
#endif
    [Name("entities")]
    public OgmoEntity[] Entities { get; set; }
}

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
}