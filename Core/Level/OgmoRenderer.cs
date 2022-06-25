using System;
using Microsoft.Xna.Framework;

namespace Teuria.Level;

public class OgmoRenderer : CanvasLayer
{
    private OgmoLevel level;
    private Tileset tileset;
    public Action<OgmoEntity> SummoningEntity;
    

    public OgmoRenderer(OgmoLevel level, Tileset tileset) 
    {
        this.level = level;
        this.tileset = tileset;
    }

    public void RenderEntities() 
    {
        foreach (var layer in level.LevelData.Layers) 
        {
            if (layer.Entities != null) 
            {
                SummonEntity(layer.Entities);
            }
        }
    }


    public override void Draw()
    {
        foreach (var layer in level.LevelData.Layers) 
        {
            if (layer.Data != null) 
            {
                DrawMap(layer.Data);
            }
        }
    }
#if !SYSTEMTEXTJSON
    private void DrawMap(int[,] data) 
    {
        for (int y = 0; y < level.LevelSize.X; y++) 
        {
            for (int x = 0; x < level.LevelSize.Y; x++) 
            {
                var gid = data[x, y];
                if (gid >= 0) 
                {    
                    var texture = tileset[gid];
                    texture.DrawTexture(SpriteBatch, FixedPosition(x * tileset.TileWidth, y * tileset.TileHeight));
                }
            }
        }
    }
#else
    private void DrawMap(int[][] data) 
    {
        for (int y = 0; y < level.LevelSize.X; y++) 
        {
            for (int x = 0; x < level.LevelSize.Y; x++) 
            {
                var gid = data[x][y];
                if (gid >= 0) 
                {
                    var texture = tileset[gid];
                    texture.DrawTexture(SpriteBatch, FixedPosition(x * tileset.TileHeight, y * tileset.TileHeight));
                }
            }
        }
    }
#endif

    private void SummonEntity(OgmoEntity[] entities) 
    {
        foreach (var entity in entities) 
        {
            SummoningEntity?.Invoke(entity);
        }
    }
    private static Vector2 FixedPosition(float x, float y) 
    {
        return new Vector2(y, x);
    }
}