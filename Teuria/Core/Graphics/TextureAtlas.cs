using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class TextureAtlas 
{
    // private readonly SpriteTexture[,] tiles;
    private readonly Array2D<SpriteTexture> tiles;
    public SpriteTexture Texture { get; private set; }
    public int TileWidth { get; private set; }
    public int TileHeight { get; private set; }

    public SpriteTexture this[int x, int y] => tiles[x, y];
    public SpriteTexture this[int gid] => tiles[gid % tiles.Rows, gid / tiles.Rows];


    public TextureAtlas(SpriteTexture texture, int tileWidth, int tileHeight) 
    {
        Texture = texture;
        TileWidth = tileWidth;
        TileHeight = tileHeight;

        tiles = new Array2D<SpriteTexture>(Texture.Width / tileWidth, Texture.Height / tileHeight);
        for (int y = 0; y < Texture.Height / tileHeight; y++) 
        {
            for (int x = 0; x < Texture.Width / tileWidth; x++) 
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

    public SpriteTexture GetTexture(int x, int y) 
    {
        return tiles[x, y];
    }

    public SpriteTexture? GetTexture(int gid) 
    {
        if (gid >= 0) 
        {
            return tiles[gid % tiles.Rows, gid / tiles.Rows];
        }
        return null;
    }
}