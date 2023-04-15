using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Teuria.Level;

namespace Teuria;


public class TileMap : Entity
{
    public Point LevelSize { get; private set; }
    public IReadOnlyList<Layer> Layers => layers;
    private List<ITextureMap> drawableLayer = new();
    private List<Layer> layers = new();
    private Dictionary<string, int> layerIdx = new();
    
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
            layerIdx.Add(layerName, layers.Count);
            LayerType layerType;
            if (layer.Entities != null) 
            {
                layerType = LayerType.Entities;
                var newLayer = new EntityLayer(layer, layerType);
                layers.Add(newLayer);
            }
            else if (layer.Data != null) 
            {
                layerType = LayerType.Tiles;
                var tileset = tilesets[layerName];
                var newLayer = new TileLayer(layer, tileset, layerType);
                layers.Add(newLayer);
                drawableLayer.Add(newLayer);
            }
            else if (layer.Grid2D != null || layer.Grid != null) 
            {
                layerType = LayerType.Grid;
                var tileset = tilesets[layerName];
                var newLayer = new GridLayer(layer, tileset, layerType);
                layers.Add(newLayer);
                drawableLayer.Add(newLayer);
            }
        }
    }

    public void Begin(Action<OgmoEntity>? spawnEntities = null) 
    {
        foreach (var layer in Layers) 
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
        foreach (var layer in Layers) 
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
        foreach (var layer in drawableLayer) 
        {
            if (!layer.Dirty)
                continue;
            GameApp.Instance.GraphicsDevice.SetRenderTarget(layer.Texture);
            GameApp.Instance.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            layer.Draw(spriteBatch);
            spriteBatch.End();
        }
        GameApp.Instance.GraphicsDevice.SetRenderTarget(renderTiles);
        GameApp.Instance.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        foreach (var layer in drawableLayer) 
        {
            spriteBatch.Draw(layer.Texture, Vector2.Zero, Color.White);
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
        var idx = layerIdx[layer];
        var targetLayer = layers[idx];
        if (targetLayer.Active == active)
            return;
        
        targetLayer.Active = active;
        dirty = true;
    }
    public Layer GetLayer(string layer) 
    {
        var idx = layerIdx[layer];
        var targetLayer = layers[idx];
        return targetLayer;
    }

    public void SetTile(string layer, Point pixel, string id) 
    {
        var idx = layerIdx[layer];
        var targetLayer = layers[idx];
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
            LayerName = layer.Name;
            LevelSize = new Point(layer.GridCellsX, layer.GridCellsY);
            LayerType = layerType;
        }

        public virtual void SetTile(Point pixelPoint, string id) {}
    }

    public class EntityLayer : Layer
    {
        public OgmoEntity[]? Entities;

        public EntityLayer(OgmoLayer layer, LayerType layerType) : base(layer, layerType) 
        {
            Entities = layer.Entities;
        }

    }

    public class GridLayer : Layer, ITextureMap
    {
        public Array2D<string> Data;
        public readonly Tileset Tileset;
        public readonly int Rows;
        public readonly int Columns;
        private readonly Array2D<SpriteTexture?> textureGridData;
        private readonly Picker<SpriteTexture> picker;
        private readonly RenderTarget2D texture; 
        private bool dirty = true;


        public RenderTarget2D Texture => texture;
        public bool Dirty => dirty;

        public GridLayer(OgmoLayer layer, Tileset tileset, LayerType layerType) : base(layer, layerType) 
        {
            texture = new RenderTarget2D(
                GameApp.Instance.GraphicsDevice, 
                layer.GridCellsX * layer.GridCellWidth, 
                layer.GridCellsY * layer.GridCellHeight
            );
            textureGridData = new Array2D<SpriteTexture?>(LevelSize.X, LevelSize.Y);
            Tileset = tileset;
            Rows = LevelSize.X;
            Columns = LevelSize.Y;
            picker = new Picker<SpriteTexture>();
            if (layer.Grid2D != null) 
            {
                Data = Array2D<string>.FromArray2D(Columns, Rows, layer.Grid2D);
                ApplyAutotile(Data, picker);
                return;
            }
            if (layer.Grid != null) 
            {
                Data = Array2D<string>.FromArray(Columns, Rows, layer.Grid);
                ApplyAutotile(Data, picker);
                return;
            }
            Data = Array2D<string>.Empty;
        }

        public void Draw(SpriteBatch spriteBatch)
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
            var (y, x) = pixelPoint;
            if (Data[x, y] == id) 
                return;

            dirty = true;

            MathUtils.StartRandScope(28);
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

    public class TileLayer : Layer, ITextureMap
    {
        public readonly Tileset Tileset;
        public readonly int[,] data;
        private readonly RenderTarget2D texture;
        private bool dirty = true;

        public TileLayer(OgmoLayer layer, Tileset tileset, LayerType layerType) : base(layer, layerType)
        {
            texture = new RenderTarget2D(
                GameApp.Instance.GraphicsDevice, 
                layer.GridCellsX * layer.GridCellWidth, 
                layer.GridCellsY * layer.GridCellHeight
            );
            LayerType = layerType;
            LevelSize = new Point(layer.GridCellsX, layer.GridCellsY);
            Tileset = tileset;
            LayerName = layer.Name;
            data = layer.Data ?? new int[0, 0]; 
        }

        public RenderTarget2D Texture => texture;

        public bool Dirty => dirty;

        public void Draw(SpriteBatch spriteBatch) 
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

public interface ITextureMap
{
    bool Dirty { get; }
    RenderTarget2D Texture { get; }
    void Draw(SpriteBatch spriteBatch);
}