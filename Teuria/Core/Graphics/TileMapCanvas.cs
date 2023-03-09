using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Teuria.TileMap;

namespace Teuria;

//TODO Work on this
public class TileMapCanvas : CanvasLayer, IDisposable
{
    public LayerRender[] renderers;
    public TileMap tileMap;
    private int width;
    private int height;
    private bool dirty;

    public TileMapCanvas(TileMap tileMap, int width, int height) 
    {
        this.tileMap = tileMap;
        var count = tileMap.Layers.Count;
        var i = 0;
        renderers = new LayerRender[count];
        this.width = width;
        this.height = height;
        foreach (var layer in tileMap.Layers.Values) 
        {
            var rt = new RenderTarget2D(GameApp.Instance.GraphicsDevice, width, height);
            renderers[i] = new LayerRender(layer, rt);
            i++;
        }
        dirty = true;
    }

    public override void PreDraw(Scene scene, SpriteBatch batch) 
    {
        if (!dirty)
            return;
        
        foreach (var layerRT in renderers) 
        {
            layerRT.Start(batch);
        }
    }

    public override void Draw(Scene scene, SpriteBatch batch)
    {
        foreach (var layerRenderer in renderers) 
        {
            batch.Draw(layerRenderer.Texture, Vector2.Zero, Color.White);
        }
    }


    public void Dispose()
    {
        foreach (var layerRT in renderers) 
        {
            layerRT.Dispose();
        }
    }

    public class LayerRender : IDisposable
    {
        private RenderTarget2D layerRT;
        private Layer layer;

        public RenderTarget2D Texture => layerRT;

        public LayerRender(Layer layer, RenderTarget2D rt) 
        {
            this.layer = layer;
            layerRT = rt;
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
    }
}