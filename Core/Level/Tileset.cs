using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Tileset 
{
    private SpriteTexture[,] tiles;
    public SpriteTexture Texture { get; private set; }
    public int TileWidth { get; private set; }
    public int TileHeight { get; private set; }

    public SpriteTexture this[int x, int y] => tiles[x, y];
    public SpriteTexture this[int gid] => gid >= 0 ? tiles[gid % tiles.GetLength(0), gid / tiles.GetLength(0)] : null;

    public Tileset(SpriteTexture texture, int tileWidth, int tileHeight) 
    {
        Texture = texture;
        TileWidth = tileWidth;
        TileHeight = tileHeight;

        tiles = new SpriteTexture[Texture.Width / tileWidth, Texture.Height / tileHeight];
        for (int x = 0; x < Texture.Width / tileWidth; x++) 
        {
            for (int y = 0; y < Texture.Height / tileHeight; y++) 
            {
                tiles[x, y] = new SpriteTexture(Texture, new Rectangle(
                    x * tileWidth,
                    y * tileHeight,
                    tileWidth,
                    tileHeight
                ));
            }
        }
    }
}