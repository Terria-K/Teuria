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
                var newLayer = new Layer(layer, null, layerType);
                this.layer[layer.Name] = newLayer;
            }
            else if (layer.Data != null) 
            {
                Console.WriteLine(layer.Name);
                layerType = LayerType.Tiles;
                var tileset = layer.Tileset != null ? tilesets[layer.Tileset] : null;
                var newLayer = new Layer(layer, tileset, layerType);
                this.layer[layer.Name] = newLayer;
            }
            else if (layer.Grid2D != null) 
            {
                Console.WriteLine(layer.Name);
                layerType = LayerType.Grid;
                var tileset = tilesets["Ruins"];
                var newLayer = new Layer(layer, tileset, layerType);
                this.layer[layer.Name] = newLayer;
            }

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
            if (entityLayer.Key != LayerType.Entities)
            {
                continue;
            }
            foreach (var str in entityLayer.Value)
            {
                foreach (var entity in layer[str].entities)
                    spawnEntities?.Invoke(entity);
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
            switch (layer.Value.LayerType) 
            {
                case LayerType.Tiles:
                case LayerType.Grid:
                    layer.Value.Draw(spriteBatch);
                    break;
                    // layer.Value.DrawGrid(spriteBatch);
                    // break;
            }
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
        public LayerType LayerType;

        //TODO Optimize this further
        public Layer(OgmoLayer layer, Tileset tileset, LayerType layerType) 
        {
            var picker = new Picker<Vector2>();
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
                    data = ApplyAutotile(gridData.ToIntArray(), picker, Tileset.InitializeTerrain());
                    break;
            }
        }

        private bool Check(int x, int y, int[,] grids) 
        {
            if (!(x < grids.GetLength(0) && y < grids.GetLength(1) && x >= 0 && y >= 0)) 
            {
                return false;
            }
            var gr = grids[x, y];
            if (gr == 0)
                return false;
            return true;
        }

        private const int NorthWest = 1 << 0;
        private const int North = 1 << 1;
        private const int NorthEast = 1 << 2;
        private const int West = 1 << 3;
        private const int East = 1 << 4;
        private const int SouthWest = 1 << 5;
        private const int South = 1 << 6;
        private const int SouthEast = 1 << 7;

        public int[,] ApplyAutotile(int[,] grids, Picker<Vector2> picker, Dictionary<byte, List<Vector2>> masks) 
        {
            var newGrid = new int[LevelSize.Y, LevelSize.X];
            for (int x = 0; x < LevelSize.X; x++) 
            {
                for (int y = 0; y < LevelSize.Y; y++) 
                {
                    var check = (int x, int y) => Check(x, y, grids);
                    
                    if (grids[y, x] == 0) 
                    {
                        newGrid[y, x] = -1;
                        continue;
                    }
                    var mask = 0;

                    if (check(y, x + 1))
                        mask += East;

                    if (check(y, x - 1))
                        mask += West;

                    if (check(y + 1, x))
                        mask += South;

                    if (check(y - 1, x))
                        mask += North;

                    if ((mask & (64 | 8)) == (64 | 8) && check(y + 1, x - 1))
                        mask += SouthWest;

                    if ((mask & (64 | 16)) == (64 | 16) && check(y + 1, x + 1))
                        mask += SouthEast;

                    if ((mask & (2 | 8)) == (2 | 8) && check(y - 1, x - 1))
                        mask += NorthWest;

                    if ((mask & (2 | 16)) == (2 | 16) && check(y - 1, x + 1))
                        mask += NorthEast;

                    List<Vector2> finalMask = masks[(byte)mask]; 

                    picker.AddOption(finalMask, 1f);
                    var picked = picker.Pick();
                    var idx = Tileset.TilesetAtlas.GetIndex((int)picked.X, (int)picked.Y);

                    newGrid[y, x] = idx; 
                    picker.Clear();
                }
            }
            return newGrid;
        }

        public void DrawGrid(SpriteBatch spriteBatch) 
        {
            if (gridData == null) return;
            for (int y = 0; y < LevelSize.X; y++) 
            {
                for (int x = 0; x < LevelSize.Y; x++) 
                {
                    var tile = gridData[x, y];
                    if (tile == "0")
                        continue;
                    var texture = Tileset.TilesetAtlas[0];
                    texture.DrawTexture(spriteBatch, new Vector2(y * Tileset.Height, x * Tileset.Width));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch) 
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