using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Teuria.Level;

namespace Teuria;


public class TileMap : Entity
{
    public Point LevelSize { get; private set; }
    public Dictionary<string, Layer> Layers = new();
    
    public enum LayerType { Tiles, Entities, Decal, Grid }
    private readonly RenderTarget2D renderTiles;
    public bool Dirty 
    {
        get => dirty;
        set => dirty = value;
    }
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
        Active = true;
        LevelSize = level.LevelSize;
        if (level.LevelData.Layers is null)
            return;
        for (int i = level.LevelData.Layers.Length - 1; i >= 0; i--) 
        {
            var layer = level.LevelData.Layers[i];
            var layerName = layer.Name ?? "Layer " + i;
            LayerType layerType;
            if (layer.Entities != null) 
            {
                layerType = LayerType.Entities;
                var newLayer = new EntityLayer(layer, layerType);
                Layers[layerName] = newLayer;
            }
            else if (layer.Data != null) 
            {
                layerType = LayerType.Tiles;
                var tileset = tilesets[layerName];
                var newLayer = new TileLayer(layer, tileset, layerType);
                Layers[layerName] = newLayer;
            }
            else if (layer.Grid2D != null || layer.Grid != null) 
            {
                layerType = LayerType.Grid;
                var tileset = tilesets[layerName];
                var newLayer = new GridLayer(layer, tileset, layerType);
                Layers[layerName] = newLayer;
            }
        }
    }

    public void Begin(Action<OgmoEntity>? spawnEntities = null) 
    {
        foreach (var layer in Layers.Values) 
        {
            if (layer is EntityLayer entityLayer && entityLayer.Entities != null) 
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
            layer.Value.Draw(spriteBatch);
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

    public void SetLayerActive(string layer, bool active)
    {
        var targetLayer = Layers[layer];
        if (targetLayer.Active == active)
            return;
        
        targetLayer.Active = active;
        dirty = true;
    }
    public Layer GetLayer(string layer) 
    {
        return Layers[layer];
    }

    public void SetTile(string layer, Point pixel, string id) 
    {
        var targetLayer = Layers[layer];
        if (!targetLayer.Active)
            return;
        targetLayer.SetTile(pixel, id);
        dirty = true;
    }

    // TODO Decals
    public abstract class Layer 
    {
        public bool Active = true;
        public string LayerName;
        public Point LevelSize;
        public LayerType LayerType;

        public Layer(OgmoLayer layer, LayerType layerType) 
        {
            LayerName = layer.Name ?? "No layer named";
            LevelSize = new Point(layer.GridCellsX, layer.GridCellsY);
            LayerType = layerType;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
        public virtual void SetTile(Point pixelPoint, string id) {}
    }

    public class EntityLayer : Layer
    {
        public OgmoEntity[]? Entities;

        public EntityLayer(OgmoLayer layer, LayerType layerType) : base(layer, layerType) 
        {
            Entities = layer.Entities;
        }

        public override void Draw(SpriteBatch spriteBatch) {}
    }

    public class GridLayer : Layer 
    {
        public Array2D<string> Data;
        public readonly Tileset Tileset;
        public readonly int Rows;
        public readonly int Columns;
        private readonly Array2D<SpriteTexture?> textureGridData;
        private readonly Picker<SpriteTexture> picker;

        public GridLayer(OgmoLayer layer, Tileset tileset, LayerType layerType) : base(layer, layerType) 
        {
            textureGridData = new Array2D<SpriteTexture?>(LevelSize.X, LevelSize.Y);
            Tileset = tileset;
            Rows = LevelSize.X;
            Columns = LevelSize.Y;
            picker = new Picker<SpriteTexture>();
            if (layer.Grid2D != null) 
            {
                Data = Array2D<string>.FromArray2D(Columns, Rows, layer.Grid2D);
                // StringData2D = layer.Grid2D ?? new string[0,0];
                ApplyAutotile(Data, picker);
                return;
            }
            Data = Array2D<string>.FromArray(Columns, Rows, layer.Grid ?? Array.Empty<string>());
            // StringData = layer.Grid ?? Array.Empty<string>();
            ApplyAutotile(Data, picker);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;
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

        public void ApplyAutotile(Array2D<string> grids, Picker<SpriteTexture> picker) 
        {
            MathUtils.StartRandScope(28);
            for (int y = 0; y < grids.Columns; y++) 
                for (int x = 0; x < grids.Rows; x++) 
                {
                    if (grids[x, y] == "0") 
                        continue;

                    var mask = ArrayUtils.GetNeighbourMask(x, y, grids, t => t != "0", true);

                    var terrainRule = Tileset.GetTerrainRules(grids[x, y]);
                    var rule = terrainRule[(byte)mask];

                    picker.AddOption(rule.Textures, 1f);
                    textureGridData[y, x] = picker.ForcePick();

                    picker.Clear();
                }
            MathUtils.EndRandScope();
        }

        public override void SetTile(Point pixelPoint, string id) 
        {
            MathUtils.StartRandScope(28);
            var (y, x) = pixelPoint;
            Data[x, y] = id;

            // Center
            UpdateAutotile(x, y);

            // North and South
            UpdateAutotile(x, y + 1);
            UpdateAutotile(x, y - 1);

            // West and East
            UpdateAutotile(x - 1, y);
            UpdateAutotile(x + 1, y);

            // NorthWest and NorthEast
            UpdateAutotile(x - 1, y - 1);
            UpdateAutotile(x + 1, y - 1);

            // SouthWest and SouthEast
            UpdateAutotile(x - 1, y + 1);
            UpdateAutotile(x + 1, y + 1);

            MathUtils.EndRandScope();
        }

        public void UpdateAutotile(int x, int y) 
        {
            if (x < 0 || x >= Data.Rows || y < 0 || y >= Data.Columns)
                return;    
            var id = Data[x, y];
            if (id == "0") 
            {
                textureGridData[y, x] = null;
                return;
            }

            var mask = ArrayUtils.GetNeighbourMask(x, y, Data, t => t != "0", true);
            var terrainRule = Tileset.GetTerrainRules(id);
            var rule = terrainRule[(byte)mask];
            picker.AddOption(rule.Textures, 1f);
            textureGridData[y, x] = picker.ForcePick();

            picker.Clear();
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
            LayerName = layer.Name ?? "NoNamed Layer";
            data = layer.Data ?? new int[0, 0]; 
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            if (!Active)
                return;
            for (int y = 0; y < LevelSize.X; y++) 
            {
                for (int x = 0; x < LevelSize.Y; x++)
                {
                    var gid = data[x, y];
                    if (gid < 0)
                        continue;
                   
                    var texture = Tileset.Sheet[gid];
                    texture.DrawTexture(
                        spriteBatch, 
                        new Vector2(y * Tileset.Height, x * Tileset.Width),
                        Color.White, 0f, Vector2.One, SpriteEffects.None, 0f);
                }
            }
        }
    }
}