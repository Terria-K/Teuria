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
            else if (layer.Grid2D != null || layer.Grid != null) 
            {
                layerType = LayerType.Grid;
                var tileset = tilesets[layer.Name];
                var newLayer = new GridLayer(layer, tileset, layerType);
                this.Layers[layer.Name] = newLayer;
            }
        }
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
        foreach (var layer in Layers.Values) 
        {
            if (layer is GridLayer gl) 
            {
                gridLayer?.Invoke(gl);
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
            }
        }
        spriteBatch.End();
        dirty = false;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(renderTiles, Vector2.Zero, Color.White);
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
        // public readonly int[,] GridData;
        public readonly string[,]? StringData2D;
        public readonly string[]? StringData;
        public readonly Tileset Tileset;
        public readonly int Rows;
        public readonly int Columns;
        private readonly Array2D<SpriteTexture> textureGridData;

// TODO 1D Array
        public GridLayer(OgmoLayer layer, Tileset tileset, LayerType layerType) : base(layer, layerType) 
        {
            textureGridData = new Array2D<SpriteTexture>(LevelSize.X, LevelSize.Y);
            Tileset = tileset;
            Rows = LevelSize.X;
            Columns = LevelSize.Y;
            var picker = new Picker<SpriteTexture>();
            if (layer.Grid2D != null) 
            {
                StringData2D = layer.Grid2D;
                ApplyAutotile(StringData2D, picker);
                return;
            }
            StringData = layer.Grid;
            ApplyAutotile(Array2D<string>.FromArray(Columns, Rows, StringData), picker);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < LevelSize.Y; y++) 
            {
                for (int x = 0; x < LevelSize.X; x++) 
                {
                    var gid = textureGridData[x, y];
                    if (gid == null) continue;
                    gid.DrawTexture(spriteBatch, new Vector2(x * 8, y * 8));
                }
            }
        }


        private static bool Check(int x, int y, string[,] grids) 
        {
            if (!(x < grids.GetLength(0) && y < grids.GetLength(1) && x >= 0 && y >= 0)) 
                return true;
            
            var gr = grids[x, y];
            if (gr == "0")
                return false;
            return true;
        }

        private static bool Check(int x, int y, Array2D<string> grids) 
        {
            if (!(x < grids.Rows && y < grids.Columns && x >= 0 && y >= 0)) 
                return true;
            
            var gr = grids[x, y];
            if (gr == "0")
                return false;
            return true;
        }

        public void ApplyAutotile(Array2D<string> grids, Picker<SpriteTexture> picker) 
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
            MathUtils.StartRandScope(28);
            for (int y = 0; y < grids.Columns; y++) 
                for (int x = 0; x < grids.Rows; x++) 
                {
                    var check = (int x, int y) => Check(x, y, grids);
                    if (grids[x, y] == "0") 
                        continue;
                    
                    var mask = 0;

                    if (check(x, y + 1)) mask += East;
                    if (check(x, y - 1)) mask += West;
                    if (check(x + 1, y)) mask += South;
                    if (check(x - 1, y)) mask += North;

                    if ((mask & (South | West)) == (South | West) && check(x + 1, y - 1))
                        mask += SouthWest;

                    if ((mask & (South | East)) == (South | East) && check(x + 1, y + 1))
                        mask += SouthEast;

                    if ((mask & (North | West)) == (North | West) && check(x - 1, y - 1))
                        mask += NorthWest;

                    if ((mask & (North | East)) == (North | East) && check(x - 1, y + 1))
                        mask += NorthEast;

                    var masker = Tileset.GetTerrainRules(grids[x, y]);
                    var rule = masker[(byte)mask];

                    picker.AddOption(rule.Textures, 1f);
                    textureGridData[y, x] = picker.ForcePick();
                    SkyLog.Assert(textureGridData[y, x] != null, "Tile Texture is null!");

                    picker.Clear();
                }
            MathUtils.EndRandScope();
        }

        public void ApplyAutotile(string[,] grids, Picker<SpriteTexture> picker) 
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
            MathUtils.StartRandScope(28);

            for (int y = 0; y < LevelSize.Y; y++) 
                for (int x = 0; x < LevelSize.X; x++)
                {
                    var check = (int x, int y) => Check(x, y, grids);
                    if (grids[y, x] == "0") 
                        continue;
                    
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
                    textureGridData[x, y] = picker.ForcePick();

                    picker.Clear();
                }
            MathUtils.EndRandScope();
        }
    }

    public class TileLayer : Layer
    {
        public readonly Tileset Tileset;
        public readonly int[,] data;

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