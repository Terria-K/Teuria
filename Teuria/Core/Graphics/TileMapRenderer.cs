using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Teuria.TileMap;

namespace Teuria;

//TODO Work on this
public class TileMapRenderer : IDisposable
{
    public LayerRender[] layerRTs;
    public TileMap tileMap;
    private int width;
    private int height;
    private bool dirty;

    internal TileMapRenderer(TileMap tileMap, int count) 
    {
        this.tileMap = tileMap;
        layerRTs = new LayerRender[count];
    }

    public static TileMapRenderer Create(TileMap tileMap, int width, int height) 
    {

        var count = tileMap.Layers.Count;
        var tileMapRenderer = new TileMapRenderer(tileMap, count) {
            width = width,
            height = height
        };
        for (int i = 0; i < count; i++)
        {
        }
        return tileMapRenderer;
    }

    public void BeginDraw(SpriteBatch batch) 
    {
        if (!dirty)
            return;
        
        foreach (var layerRT in layerRTs) 
        {
            layerRT.Start(batch);
        }
    }

    public void Draw(SpriteBatch batch) 
    {
        foreach (var layerRT in layerRTs) 
        {
            batch.Draw(layerRT.Texture, Vector2.Zero, Color.White);
        }
    }

    public void EndDraw(SpriteBatch batch) {}

    public void Dispose()
    {
        foreach (var layerRT in layerRTs) 
        {
            layerRT.Dispose();
        }
    }

    public class LayerRender : IDisposable
    {
        private RenderTarget2D layerRT;
        private bool dirty;
        private Layer layer;

        public RenderTarget2D Texture => layerRT;

        public LayerRender(Layer layer, RenderTarget2D rt) 
        {
            this.layer = layer;
            rt = layerRT;
        }

        public void Start(SpriteBatch spriteBatch) 
        {
            if (!dirty)
                return;
            GameApp.Instance.GraphicsDevice.SetRenderTarget(layerRT);
            GameApp.Instance.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            switch (layer.LayerType) 
            {
                case LayerType.Tiles:
                    layer.Draw(spriteBatch);
                    break;
                case LayerType.Grid:
                    layer.DrawTextures(spriteBatch);
                    break;
                    // layer.Value.DrawGrid(spriteBatch);
                    // break;
            }
            spriteBatch.End();
        }


        public void Dispose()
        {
            layerRT.Dispose();
        }
    }
}