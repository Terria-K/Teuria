using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Teuria.TileMap;

namespace Teuria;

//TODO Work on this
public class TileMapCanvas : CanvasLayer, IDisposable
{
    private TileMap tileMap;
    private Dictionary<string, int> lookup;
    private WeakList<LayerRender> renderers;
    private bool dirty;

    public TileMapCanvas(TileMap tileMap) 
    {
        lookup = new Dictionary<string, int>();
        this.tileMap = tileMap;
        var count = tileMap.Layers.Count;
        renderers = new WeakList<LayerRender>(count);
        var i = 0;
        foreach (var layer in tileMap.Layers.Values) 
        {
            renderers.Add(new LayerRender(layer));
            lookup.Add(layer.LayerName, i);
            i++;
        }
        dirty = true;
    }

    public override void PreDraw(Scene scene, SpriteBatch batch) 
    {
        if (!dirty)
            return;
        
        for (int i = 0; i < renderers.Count; i++) 
        {
            var layerRT = renderers[i];
            if (!layerRT.Active)
                return;
            layerRT.Start(batch);
        }
    }

    public override void Draw(Scene scene, SpriteBatch batch)
    {
        for (int i = 0; i < renderers.Count; i++) 
        {
            var layerRT = renderers[i];
            if (!layerRT.Active)
                return;
            batch.Draw(layerRT.Texture, Vector2.Zero, Color.White);
        }
    }


    public void Dispose()
    {
        for (int i = 0; i < renderers.Count; i++) 
        {
            var layerRT = renderers[i];
            layerRT.Dispose();
        }
    }

    public LayerRender GetLayer(string name) 
    {
        var idx = lookup[name];
        return renderers[idx];
    }

    public Texture2D GetLayerTexture(string name) 
    {
        var layer = GetLayer(name);
        return layer.Texture;
    }

    public void SetActive(string name, bool active) 
    {
        var layer = GetLayer(name);
        layer.SetActive(this, active);
    }

    public void Remove(string name) 
    {
        var layer = GetLayer(name);
        renderers.Remove(layer);
    }

    public class LayerRender : IDisposable
    {
        private RenderTarget2D layerRT;
        private Layer layer;

        public RenderTarget2D Texture => layerRT;
        public bool Active => active;
        private bool active;

        public LayerRender(Layer layer) 
        {
            layerRT = new RenderTarget2D(GameApp.Instance.GraphicsDevice, layer.LevelSize.X, layer.LevelSize.Y);
            this.layer = layer;
        }

        public void Start(SpriteBatch spriteBatch) 
        {
            GameApp.Instance.GraphicsDevice.SetRenderTarget(layerRT);
            GameApp.Instance.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            layer.Draw(spriteBatch);
            spriteBatch.End();
        }

        public void Dispose()
        {
            layerRT.Dispose();
        }

        public void SetActive(TileMapCanvas ctx, bool active) 
        {
            ctx.dirty = true;
            this.active = active;
        }
    }
}