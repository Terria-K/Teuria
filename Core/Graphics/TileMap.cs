using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Teuria.Level;

namespace Teuria;

public class TileMap : Entity
{
    public Point LevelSize { get; private set; }
    public Dictionary<string, Layer> layer = new Dictionary<string, Layer>();
    private Dictionary<LayerType, List<string>> recognizeable = new Dictionary<LayerType, List<string>>();
    private OgmoLayer[] layers;
    private PhysicsCanvas physicsHandler;
    
    public enum LayerType { Tiles, Entities, Decal, Grid }

    public TileMap(OgmoLevel level, Dictionary<string, Tileset> tilesets) 
    {
        Active = false;
        LevelSize = level.LevelSize;
        layers = level.LevelData.Layers;
        foreach (var layer in level.LevelData.Layers) 
        {
            LayerType layerType = LayerType.Tiles;
            if (layer.Entities != null) 
            {
                Console.WriteLine(layer.Entities);
                layerType = LayerType.Entities;   
            }
            else if (layer.Data != null) 
            {
                Console.WriteLine(layer.Name);
                layerType = LayerType.Tiles;
            }
            else if (layer.Grid2D != null) 
            {
                Console.WriteLine(layer.Name);
                layerType = LayerType.Grid;
            }
            var tileset = layer.Tileset != null ? tilesets[layer.Tileset] : null;
            var newLayer = new Layer(layer, tileset, layerType);
            this.layer[layer.Name] = newLayer;
        }
    }

    public void RecognizeLayer(string layer, LayerType layerType) 
    {
        if (recognizeable.ContainsKey(layerType)) 
        {
            recognizeable[layerType].Add(layer);
            return;
        }
        recognizeable.Add(layerType, new List<string>() { layer });
    }

    public void AddPhysicsHandler(PhysicsCanvas physicsCanvas) 
    {
        physicsHandler = physicsCanvas;
    }

    public void Begin(Action<OgmoEntity> spawnEntities = null) 
    {
        Active = true;
        foreach (var entityLayer in recognizeable) 
        {
            if (entityLayer.Key == LayerType.Entities) 
            {
                foreach (var str in entityLayer.Value) 
                {
                    foreach (var entity in layer[str].entities)
                        spawnEntities?.Invoke(entity);
                }
            }
        }
    }

    public void InitGrid(Action<Layer> gridLayer) 
    { 
        if (!Active) 
        {
            Console.WriteLine("The TileMap is inactive, unable to initialize grids");
            return;
        }
        foreach (var grid in recognizeable) 
        {
            if (grid.Key == LayerType.Grid) 
            {
                foreach (var str in grid.Value) 
                {
                    var gl = layer[str];
                    gridLayer?.Invoke(gl);
                }
            }
        }

    }

    private void DrawMap(SpriteBatch spriteBatch) 
    {
        foreach (var layer in this.layer) 
        {
            layer.Value.Draw(spriteBatch, physicsHandler);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        DrawMap(spriteBatch);
        base.Draw(spriteBatch);
    }

    public class Layer 
    {
        public string LayerName;
        public Tileset Tileset;
        public int[,] data;
        public string[,] gridData;
        public string[] singleGridData;
        public OgmoEntity[] entities;
        public Point LevelSize;
        // TODO Reference a LayerType
        public LayerType LayerType;

        public Layer(OgmoLayer layer, Tileset tileset, LayerType layerType) 
        {
            LayerType = layerType;
            LevelSize = new Point(layer.GridCellsX, layer.GridCellsY);
            Tileset = tileset;
            LayerName = layer.Name;
            switch (LayerType) 
            {
                case LayerType.Tiles:
#if SYSTEMTEXTJSON
                    data = layer.Data.To2D();
#else
                    data = layer.Data; 
#endif
                    break;
                case LayerType.Entities:
                    entities = layer.Entities;
                    break;
                case LayerType.Decal:
                    break;
                case LayerType.Grid:
                    if (layer.Grid2D is null) 
                    {
                        singleGridData = layer.Grid;
                        return;
                    }
#if SYSTEMTEXTJSON
                    gridData = layer.Grid2D.To2D();
#else
                    gridData = layer.Grid2D;
#endif
                    break;
            }
        }

        public void DrawGrid(SpriteBatch spriteBatch) 
        {
            if (data == null) return;
            for (int y = 0; y < LevelSize.X; y++) 
            {
                for (int x = 0; x < LevelSize.Y; x++) 
                {
                    var tile = gridData[x, y];
                    if (tile == "0")
                        continue;
                    
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, PhysicsCanvas physicsCanvas) 
        {
            if (data == null) return;
            for (int y = 0; y < LevelSize.X; y++) 
            {
                for (int x = 0; x < LevelSize.Y; x++)
                {
                    var gid = data[x, y];
                    if (gid < 0)
                    {
                        continue;
                    }
                    var texture = Tileset.TilesetAtlas[gid];
                    texture.DrawTexture(spriteBatch, new Vector2(y * Tileset.Height, x * Tileset.Width));
                }
            }
        }
    }
}