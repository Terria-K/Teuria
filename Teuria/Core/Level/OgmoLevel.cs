using System.IO;
using Microsoft.Xna.Framework;
using LightJson;
using LightJson.Serialization;
using System.Collections.Generic;

namespace Teuria.Level;

public class OgmoLevel 
{
    public OgmoLevelData LevelData { get; private set; }
    public Point LevelSize { get; private set; }
    public Point TileSize { get; private set; }
    public Point LevelPixelSize { get; private set; }

    private OgmoLevel(string path) 
    {
        using var fs = TitleContainer.OpenStream(path);
        var result = JsonConvert.DeserializeFromStream<OgmoLevelData>(fs);

        LevelData = result;

        if (result.Layers != null) 
        {
            var firstLayer = result.Layers[0];
            LevelSize = new Point(firstLayer.GridCellsX, firstLayer.GridCellsY);
            TileSize = new Point(firstLayer.GridCellWidth, firstLayer.GridCellHeight);
            LevelPixelSize = LevelSize * TileSize;
            return;
        }
        SkyLog.Log("There is no layer in this ogmo project, please add one.", SkyLog.LogLevel.Error);
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
            return new OgmoLevel(levelPath);
#if DEBUG
        } catch (IOException) 
        {
            return null!;
        }
#endif

    }
}

#nullable disable

[JsonSerializable]
public partial class OgmoLevelData 
{
    [JName("width")]
    public int Width { get; set; }
    [JName("height")]
    public int Height { get; set; }
    [JName("offsetX")]
    public int OffsetX { get; set; }
    [JName("offsetY")]
    public int OffsetY { get; set; }
    [JName("layers")]
    [JArray(SupportedTypes.Other)]
    public OgmoLayer[] Layers { get; set; }
    [JName("values")]
    public JsonValue Values { get; set; }


    public int GetValueInt(string valueName) 
    {
        return Values[valueName].AsInteger;
    }

    public bool GetValueBoolean(string valueName) 
    {
        return Values[valueName].AsBoolean;
    }

    public float GetValueFloat(string valueName) 
    {
        return Values[valueName].AsNumberReal;
    }

    public Vector2 GetValueVector2(string x, string y) 
    {
        return new Vector2(Values[x].AsNumberReal, Values[y].AsNumberReal);
    }

    public string GetValueString(string valueName) 
    {
        return Values[valueName].AsString;
    }

}

[JsonSerializable]
public partial class OgmoLayer 
{
    [JName("name")]
    public string Name { get; set; }
    [JName("offsetX")]
    public int OffsetX { get; set; }
    [JName("offsetY")]
    public int OffsetY { get; set; }
    [JName("gridCellWidth")]
    public int GridCellWidth { get; set; }
    [JName("gridCellHeight")]
    public int GridCellHeight { get; set; }
    [JName("gridCellsX")]
    public int GridCellsX { get; set; }
    [JName("gridCellsY")]
    public int GridCellsY { get; set; }
    [JName("tileset")]
    public string Tileset { get; set; }

    [JName("data2D")]
    [JArray(SupportedTypes.Int2D)]
    public int[,] Data { get; set; }

    [JName("grid2D")]
    [JArray(SupportedTypes.String2D)]
    public string[,] Grid2D { get; set; }

    [JName("grid")]
    [JArray(SupportedTypes.String)]
    public string[] Grid { get; set; }

    [JName("entities")]
    [JArray(SupportedTypes.Other)]
    public OgmoEntity[] Entities { get; set; }
}

[JsonSerializable]
public partial class OgmoNode 
{
    [JName("x")]
    public float X { get; set; }
    [JName("y")]
    public float Y { get; set; }

    public Vector2 ToVector2() 
    {
        return new Vector2(X, Y);
    }
}

[JsonSerializable]
public partial class OgmoEntity 
{
    [JName("name")]
    public string Name { get; set; }
    [JName("id")]
    public int ID { get; set; }
    [JName("x")]
    public int X { get; set; }
    [JName("y")]
    public int Y { get; set; }
    [JName("originX")]
    public int OriginX { get; set; }
    [JName("originY")]
    public int OriginY { get; set; }
    [JName("width")]
    public int Width { get; set; }
    [JName("height")]
    public int Height { get; set; }
    [JName("flippedX")]
    public bool FlippedX { get; set; }
    [JName("flippedY")]
    public bool FlippedY { get; set; }
    [JName("nodes")]
    [JArray(SupportedTypes.Other)]
    public OgmoNode[] Nodes { get; set; }
    [JName("values")]
    public JsonValue Values { get; set; }

    [JIgnore]
    public Vector2 Position => new Vector2(X, Y);
    [JIgnore]
    public Rectangle Size => new Rectangle(OriginX, OriginY, Width, Height);


    public int GetValueInt(string valueName) 
    {
        return Values[valueName].AsInteger;
    }

    public bool GetValueBoolean(string valueName) 
    {
        return Values[valueName].AsBoolean;
    }

    public float GetValueFloat(string valueName) 
    {
        return Values[valueName].AsNumberReal;
    }

    public Vector2 GetValueVector2(string x, string y) 
    {
        return new Vector2(Values[x].AsNumberReal, Values[y].AsNumberReal);
    }

    public string GetValueString(string valueName) 
    {
        return Values[valueName].AsString;
    }

}
