using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public sealed class TileGrid : Shape
{
    public Array2D<bool> CollisionGrid;
    public int CellWidth { get; private set; }
    public int CellHeight { get; private set; }
    public AABB BoundingArea;
    public int CellX => CollisionGrid.Columns;
    public int CellY => CollisionGrid.Rows;

    public TileGrid(int cellsX, int cellsY, int cellWidth, int cellHeight) 
    {
        BoundingArea = new AABB(0, 0, cellWidth, cellHeight);
        CollisionGrid = new Array2D<bool>(cellsX, cellsY);

        CellWidth = cellWidth;
        CellHeight = cellHeight;
    }

    public TileGrid(int cellWidth, int cellHeight, string[,] characters) 
    {
        var columns = characters.GetLength(0);
        var rows = characters.GetLength(1);
        BoundingArea = new AABB(0, 0, rows * 8, columns * 8);
        CollisionGrid = new Array2D<bool>(rows, columns);
        CellWidth = cellWidth;
        CellHeight = cellHeight;
        for (int y = 0; y < columns; y++) 
            for (int x = 0; x < rows; x++) 
            {
                if (characters[y, x] == "0") 
                {
                    CollisionGrid[x, y] = false;
                    continue;
                } 
                CollisionGrid[x, y] = true;
            }
    }

    public TileGrid(int cellWidth, int cellHeight, Array2D<bool> grid) 
    {
        BoundingArea = new AABB(0, 0, grid.Rows * 8, grid.Columns * 8);
        CollisionGrid = grid;
        CellWidth = cellWidth;
        CellHeight = cellHeight;
    }

    public TileGrid(int cellWidth, int cellHeight, int columns, int rows, string[] characters) 
    {
        var characters2D = StackArray2D<string>.FromArray(columns, rows, characters);
        BoundingArea = new AABB(0, 0, rows * 8, columns * 8);
        CollisionGrid = new Array2D<bool>(rows, columns);
        CellWidth = cellWidth;
        CellHeight = cellHeight;
        for (int y = 0; y < columns; y++) 
            for (int x = 0; x < rows; x++) 
            {
                if (characters2D[y, x] == "0") 
                {
                    CollisionGrid[x, y] = false;
                    continue;
                } 
                CollisionGrid[x, y] = true;
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
                if (CollisionGrid[x + xa, y + ya]) { return true; }
            }

        return false;
    }

    public override bool Collide(Point value)
    {
        throw new System.NotImplementedException();
    }

    public override bool Collide(Vector2 value)
    {
        if (value.X >= GlobalLeft && value.Y >= GlobalTop && value.X < GlobalRight && value.Y < GlobalBottom) 
        {
            var indexX = (int)((value.X - GlobalLeft) / CellWidth);
            var indexY = (int)((value.Y - GlobalTop) / CellHeight);
            return CollisionGrid[indexX, indexY];
        }
        return false;
    }

    public override bool Collide(TileGrid grid, Vector2 offset)
    {
        return false;
    }

    public override bool Collide(CircleShape other, Vector2 offset = default)
    {
        return false;
    }

    public override void DebugDraw(SpriteBatch spriteBatch)
    {
    }

    public override bool Collide(Colliders other, Vector2 offset = default)
    {
        return other.Collide(this);
    }

    public override Shape Clone()
    {
        return new TileGrid(CellWidth, CellHeight, CollisionGrid.Clone());
    }
}
