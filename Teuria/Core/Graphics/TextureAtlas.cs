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
    // public SpriteTexture this[int gid] => gid >= 0 ? tiles[gid % tiles.GetLength(0), gid / tiles.GetLength(0)] : null;
    public SpriteTexture? this[int gid] => gid >= 0 ? tiles[gid % tiles.Rows, gid / tiles.Rows] : null;


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

    public int GetIndex(int column, int row) 
    {
        var width = Texture.Width / this.TileWidth;
        var height = Texture.Height / this.TileHeight;

        if (column < width && row < height ) 
        {
            var gid = row * width + column;
            return gid; 
        }
        return 0;
    }
}