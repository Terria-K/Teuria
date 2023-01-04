using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using LightJson;

namespace Teuria.Level;

public class OgmoLevel 
{
    public OgmoLevelData LevelData { get; private set; }
    public Point LevelSize { get; private set; }
    public Point TileSize { get; private set; }
    public Point LevelPixelSize { get; private set; }

    private OgmoLevel(string path) 
    {
        var result = JsonConvert.DeserializeFromFile<OgmoLevelData>(path);

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
            return new OgmoLevel(levelPath);
#if DEBUG
        } catch (IOException) 
        {
            return null;
        }
#endif

    }
}

public class OgmoLevelData : IJsonDeserializable
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public OgmoLayer[] Layers { get; set; }
    public JsonObject Values { get; set; }


    public void Deserialize(JsonObject obj)
    {
        Width = obj["width"];
        Height = obj["height"];
        OffsetX = obj["offsetX"];
        OffsetY = obj["offsetY"];
        Layers = obj["layers"].ConvertToArray<OgmoLayer>();
        Values = obj["values"].AsJsonObject;
    }

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


public class OgmoLayer : IJsonDeserializable
{
    public string Name { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public int GridCellWidth { get; set; }
    public int GridCellHeight { get; set; }
    public int GridCellsX { get; set; }
    public int GridCellsY { get; set; }
    public string Tileset { get; set; }
    public int[,] Data { get; set; }
    public string[,] Grid2D { get; set; }
    public string[] Grid { get; set; }
    public OgmoEntity[] Entities { get; set; }


    public void Deserialize(JsonObject obj)
    {
        Name = obj["name"];
        OffsetX = obj["offsetX"];
        OffsetY = obj["offsetY"];
        GridCellWidth = obj["gridCellWidth"];
        GridCellHeight = obj["gridCellHeight"];
        GridCellsX = obj["gridCellsX"];
        GridCellsY = obj["gridCellsY"];
        Tileset = obj["tileset"];
        Data = obj["data2D"].ConvertToArrayInt2D();
        Grid2D = obj["grid2D"].ConvertToArrayString2D();
        Grid = obj["grid"].ConvertToArrayString();
        Entities = obj["entities"].ConvertToArray<OgmoEntity>();
    }
}

public class OgmoNode : IJsonDeserializable
{
    public float X { get; set; }
    public float Y { get; set; }

    public void Deserialize(JsonObject obj)
    {
        X = obj["x"];
        Y = obj["y"];
    }

    public Vector2 ToVector2() 
    {
        return new Vector2(X, Y);
    }
}

public class OgmoEntity : IJsonDeserializable
{
    public string Name { get; set; }
    public int ID { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int OriginX { get; set; }
    public int OriginY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool FlippedX { get; set; }
    public bool FlippedY { get; set; }
    public OgmoNode[] Nodes { get; set; }
    public JsonValue Values { get; set; }


    public Vector2 Position => new Vector2(X, Y);
    public Rectangle Size => new Rectangle(OriginX, OriginY, Width, Height);


    public void Deserialize(JsonObject obj)
    {
        Name = obj["name"];
        ID = obj["id"];
        X = obj["x"];
        Y = obj["y"];
        OriginX = obj["originX"];
        OriginY = obj["originY"];
        Width = obj["width"];
        Height = obj["height"];
        FlippedX = obj["flippedX"];
        FlippedY = obj["flippedY"];
        Nodes = obj["nodes"].ConvertToArray<OgmoNode>();
        Values = obj["values"];
    }

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
