using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Teuria.Level;

namespace Teuria;


public class TileMap : Entity
{
    public Point LevelSize { get; private set; }
    public Dictionary<string, Layer> Layers = new Dictionary<string, Layer>();
    private Dictionary<LayerType, List<string>> recognizeable = new Dictionary<LayerType, List<string>>();
    
    public enum LayerType { Tiles, Entities, Decal, Grid }
    private RenderTarget2D renderTiles;
    public bool Dirty => dirty;
    private bool dirty = true;

    public TileMap(
        OgmoLevel level, 
        int width,
        int height,
        Dictionary<string, Tileset> tilesets
    ) 
    {
        renderTiles = new RenderTarget2D(GameApp.Instance.GraphicsDevice, width, height);
        Depth = 3;
        Active = false;
        LevelSize = level.LevelSize;
        for (int i = level.LevelData.Layers.Length - 1; i >= 0; i--) 
        {
            var layer = level.LevelData.Layers[i];
            LayerType layerType = LayerType.Tiles;
            if (layer.Entities != null) 
            {
                layerType = LayerType.Entities;   
                var newLayer = new EntityLayer(layer, layerType);
                this.Layers[layer.Name] = newLayer;
            }
            else if (layer.Data != null) 
            {
                layerType = LayerType.Tiles;
                var tileset = tilesets[layer.Name];
                var newLayer = new TileLayer(layer, tileset, layerType);
                this.Layers[layer.Name] = newLayer;
            }
            else if (layer.Grid2D != null) 
            {
                layerType = LayerType.Grid;
                var tileset = tilesets[layer.Name];
                var newLayer = new GridLayer(layer, tileset, layerType);
                this.Layers[layer.Name] = newLayer;
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

    public void Begin(Action<OgmoEntity>? spawnEntities = null) 
    {
        Active = true;
        foreach (var layer in Layers.Values) 
        {
            if (layer is EntityLayer entityLayer) 
            {
                foreach (var entity in entityLayer.Entities) 
                    spawnEntities?.Invoke(entity); 
            }
        }
    }

    public void InitGrid(Action<Layer> gridLayer) 
    { 
        // if (!Active) 
        // {
        //     Console.WriteLine("The TileMap is inactive, unable to initialize grids");
        //     return;
        // }
        foreach (var grid in recognizeable) 
        {
            if (grid.Key == LayerType.Grid) 
            {
                foreach (var str in grid.Value) 
                {
                    var gl = Layers[str];
                    gridLayer?.Invoke(gl);
                }
            }
        }

    }

    public void BeforeRender(SpriteBatch spriteBatch) 
    {
        if (!dirty)
            return;
        GameApp.Instance.GraphicsDevice.SetRenderTarget(renderTiles);
        GameApp.Instance.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin();
        foreach (var layer in this.Layers) 
        {
            switch (layer.Value.LayerType) 
            {
                case LayerType.Tiles:
                    layer.Value.Draw(spriteBatch);
                    break;
                case LayerType.Grid:
                    layer.Value.Draw(spriteBatch);
                    break;
                    // layer.Value.DrawGrid(spriteBatch);
                    // break;
            }
        }
        spriteBatch.End();
        dirty = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(renderTiles, Vector2.Zero, Color.White);
        // DrawMap(spriteBatch);
        base.Draw(spriteBatch);
    }

    public override void ExitScene(Scene scene)
    {
        renderTiles.Dispose();
        base.ExitScene(scene);
    }

    public abstract class Layer 
    {
        public string LayerName;
        public Point LevelSize;
        public LayerType LayerType;

        public Layer(OgmoLayer layer, LayerType layerType) 
        {
            LayerName = layer.Name;
            LevelSize = new Point(layer.GridCellsX, layer.GridCellsY);
            LayerType = layerType;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }

    public class EntityLayer : Layer
    {
        public OgmoEntity[] Entities;

        public EntityLayer(OgmoLayer layer, LayerType layerType) : base(layer, layerType) 
        {
            Entities = layer.Entities;
        }

        public override void Draw(SpriteBatch spriteBatch) {}
    }

    public class GridLayer : Layer 
    {
        public int[,] GridData;
        public string[,] StringData;
        public Tileset Tileset;
        private Array2D<SpriteTexture> textureGridData;

        public GridLayer(OgmoLayer layer, Tileset tileset, LayerType layerType) : base(layer, layerType) 
        {
            textureGridData = new Array2D<SpriteTexture>(LevelSize.X, LevelSize.Y);
            Tileset = tileset;
            StringData = layer.Grid2D;
            var picker = new Picker<SpriteTexture>();
            GridData = ApplyAutotile(StringData, picker);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (textureGridData == null) return; 
            for (int w = 0; w < LevelSize.X; w++) 
            {
                for (int x = 0; x < LevelSize.Y; x++) 
                {
                    var gid = textureGridData[w, x];
                    if (gid == null) continue;
                    gid.DrawTexture(
                        spriteBatch, 
                        new Vector2(w * 8, x * 8));
                }
            }
        }


        private static bool Check(int x, int y, string[,] grids) 
        {
            if (!(x < grids.GetLength(0) && y < grids.GetLength(1) && x >= 0 && y >= 0)) 
            {
                return true;
            }
            var gr = grids[x, y];
            if (gr == "0")
                return false;
            return true;
        }

        public int[,] ApplyAutotile(string[,] grids, Picker<SpriteTexture> picker) 
        {
#region Constants
            const int NorthWest = 1 << 0;
            const int North = 1 << 1;
            const int NorthEast = 1 << 2;
            const int West = 1 << 3;
            const int East = 1 << 4;
            const int SouthWest = 1 << 5;
            const int South = 1 << 6;
            const int SouthEast = 1 << 7;
#endregion
            var newGrid = new int[LevelSize.Y, LevelSize.X];

            MathUtils.StartRandScope(28);

            for (int x = 0; x < LevelSize.X; x++)
                for (int y = 0; y < LevelSize.Y; y++) 
                {
                    var check = (int x, int y) => Check(x, y, grids);
                    if (grids[y, x] == "0") 
                    {
                        newGrid[y, x] = -1;
                        continue;
                    }
                    var mask = 0;

                    if (check(y, x + 1)) mask += East;
                    if (check(y, x - 1)) mask += West;
                    if (check(y + 1, x)) mask += South;
                    if (check(y - 1, x)) mask += North;

                    if ((mask & (South | West)) == (South | West) && check(y + 1, x - 1))
                        mask += SouthWest;

                    if ((mask & (South | East)) == (South | East) && check(y + 1, x + 1))
                        mask += SouthEast;

                    if ((mask & (North | West)) == (North | West) && check(y - 1, x - 1))
                        mask += NorthWest;

                    if ((mask & (North | East)) == (North | East) && check(y - 1, x + 1))
                        mask += NorthEast;

                    var masker = Tileset.GetTerrainRules(grids[y, x]);
                    var rule = masker[(byte)mask];

                    picker.AddOption(rule.Textures, 1f);
                    textureGridData[x, y] = picker.Pick()!;

                    picker.Clear();
                }
            MathUtils.EndRandScope();
            return newGrid;
        }
    }

    public class TileLayer : Layer
    {
        public Tileset Tileset;
        public int[,] data;

        public TileLayer(OgmoLayer layer, Tileset tileset, LayerType layerType) : base(layer, layerType)
        {
            LayerType = layerType;
            LevelSize = new Point(layer.GridCellsX, layer.GridCellsY);
            Tileset = tileset;
            LayerName = layer.Name;
            data = layer.Data; 
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            if (data == null) return;
            for (int y = 0; y < LevelSize.X; y++) 
            {
                for (int x = 0; x < LevelSize.Y; x++)
                {
                    var gid = data[x, y];
                    if (gid < 0)
                        continue;
                   
                    var texture = Tileset.TilesetAtlas[gid];
                    texture.DrawTexture(
                        spriteBatch, 
                        new Vector2(y * Tileset.Height, x * Tileset.Width),
                        Color.White, 0f, Vector2.One, SpriteEffects.None, 0f);
                }
            }
        }
    }
}