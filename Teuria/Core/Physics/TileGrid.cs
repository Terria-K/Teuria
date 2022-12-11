using Microsoft.Xna.Framework;

namespace Teuria;

public sealed class TileGrid : Shape
{
    public Array2D<bool> CollisionGrid;
    public float CellWidth { get; private set; }
    public float CellHeight { get; private set; }
    public override AABB BoundingArea { get; }
    public int CellX => CollisionGrid.Columns;
    public int CellY => CollisionGrid.Rows;

    public TileGrid(int cellsX, int cellsY, float cellWidth, float cellHeight) 
    {
        BoundingArea = new AABB(0f, 0f, cellWidth, cellHeight);
        CollisionGrid = new Array2D<bool>(cellsX, cellsY);

        CellWidth = cellWidth;
        CellHeight = cellHeight;
    }

    public TileGrid(float cellWidth, float cellHeight, string[,] characters) 
    {
        var columns = characters.GetLength(0);
        var rows = characters.GetLength(1);
        BoundingArea = new AABB(0f, 0f, rows * 8.0f, columns * 8.0f);
        CollisionGrid = new Array2D<bool>(rows, columns);
        CellWidth = cellWidth;
        CellHeight = cellHeight;
        for (int y = 0; y < columns; y++) 
            for (int x = 0; x < rows; x++) 
            {
                if (characters[y, x] == "0") 
                {
                    CollisionGrid[y, x] = false;
                    continue;
                } 
                CollisionGrid[y, x] = true;
            }
    }

    public override float Width => CellWidth * CellX;
    public override float Height => CellHeight * CellY;

    private bool IntersectBound(AABB bound, Vector2 offset) 
    {
        return Right > bound.Left &&
        Left < bound.Right &&
        Bottom > bound.Top &&
        Top < bound.Bottom;
    }

    public override bool Collide(float x, float y, float width, float height, Vector2 offset = default)
    {
        var aabb = new AABB(x, y, width, height);
        return Collide(aabb, offset);
    }

    public override bool Collide(RectangleShape other, Vector2 offset = default)
    {
        var aabb = new AABB(
            other.GlobalX + offset.X, 
            other.GlobalY + offset.Y,
            other.Width,
            other.Height
        );
        return Collide(aabb, offset);
    }

    public override bool Collide(Rectangle rect, Vector2 offset = default)
    {
        if (!rect.Intersects(BoundingArea)) { return false; }
        return false;
    }

    public override bool Collide(AABB aabb, Vector2 offset = default)
    {
        if (!BoundingArea.Contains(aabb)) { return false; }

        var x = (int)((aabb.Left + offset.X - Left) / CellWidth);
        var y = (int)((aabb.Top + offset.Y - Top) / CellHeight);

        var width = (int)((aabb.Right - Left - 1.0f) / CellWidth) - x + 1;
        var height = (int)((aabb.Bottom - Top - 1.0f) / CellHeight) - y + 1;

        if (x < 0) 
        {
            width += x;
            x = 0;
        }
        if (y < 0) 
        {
            height += y;
            y = 0;
        }
        if (x + width > CellY)  { width = CellY - x; }
        if (y + height > CellX)  { height = CellX - y; }

        for (int xa = 0; xa < width; xa++) 
            for (int ya = 0; ya < height; ya++) 
            {
                if (CollisionGrid[y + ya, x + xa]) { return true; }
            }

        return false;
    }

    public override bool Collide(Point value)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(Vector2 value)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(TileGrid grid, Vector2 offset)
    {
        return false;
    }
}
