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
        Active = false;
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
        Active = true;
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
        // public readonly int[,] GridData;
        public readonly string[,]? StringData2D;
        public readonly string[]? StringData;
        public readonly Tileset Tileset;
        public readonly int Rows;
        public readonly int Columns;
        private readonly Array2D<SpriteTexture> textureGridData;

        public GridLayer(OgmoLayer layer, Tileset tileset, LayerType layerType) : base(layer, layerType) 
        {
            textureGridData = new Array2D<SpriteTexture>(LevelSize.X, LevelSize.Y);
            Tileset = tileset;
            Rows = LevelSize.X;
            Columns = LevelSize.Y;
            var picker = new Picker<SpriteTexture>();
            if (layer.Grid2D != null) 
            {
                StringData2D = layer.Grid2D ?? new string[0,0];
                ApplyAutotile(StringData2D, picker);
                return;
            }
            StringData = layer.Grid ?? Array.Empty<string>();
            ApplyAutotile(Array2D<string>.FromArray(Columns, Rows, StringData), picker);
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
            MathUtils.StartRandScope(28);

            for (int y = 0; y < LevelSize.Y; y++) 
                for (int x = 0; x < LevelSize.X; x++)
                {
                    if (grids[y, x] == "0") 
                        continue;
                    
                    var mask = ArrayUtils.GetNeighbourMask(y, x, grids, t => t != "0", true);

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