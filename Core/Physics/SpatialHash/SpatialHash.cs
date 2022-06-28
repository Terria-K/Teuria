using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Teuria;

[Obsolete("Everything in the SpatialHash is unfinished")]
public class SpatialHash 
{
    public readonly List<IHashable>[,] Grid;

    private List<IHashable> queryBucket;
    private int queryId;

    public int Width;
    public int Height;

    public readonly int CellSize;

    public SpatialHash(int width, int height, int cellSize) 
    {
        Grid = new List<IHashable>[width, height];
        queryBucket = new List<IHashable>();

        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++) 
            {
                Grid[x, y] = new List<IHashable>();
            }
        }

        Width = width;
        Height = height;

        CellSize = cellSize;
    }

    public CellIndex GetCellIndex(Vector2 position) 
    {
        var inverseCellSize = 1f / CellSize;

        int x = (int)Math.Floor(position.X * inverseCellSize);
        int y = (int)Math.Floor(position.Y * inverseCellSize);

        int clampedX = Math.Min(Width - 1, Math.Max(0, x));
        int clampedY = Math.Min(Height - 1, Math.Max(0, y));

        return new CellIndex(clampedX, clampedY);
    }

    public void Insert(IHashable collider, Vector2 topLeftBounds, Vector2 bottomRightBounds) 
    {
        var startCoords = GetCellIndex(topLeftBounds);
        var endCoords = GetCellIndex(bottomRightBounds);

        collider.RegisteredHashBounds = (startCoords, endCoords);

        for (int x0 = startCoords.X, x1 = endCoords.X; x0 <= x1; x0++) 
        {
            for (int y0 = startCoords.Y, y1 = endCoords.Y; y0 <= y1; y0++) 
            {
                Grid[x0, y0].Add(collider);
            }
        }
    }

    public void Remove(IHashable collider) 
    {
        if (collider.RegisteredHashBounds != null) 
        {
            (CellIndex, CellIndex) colliderHashBounds = ((CellIndex, CellIndex))collider.RegisteredHashBounds;
            var startCoords = colliderHashBounds.Item1;
            var endCoords = colliderHashBounds.Item2;

            collider.RegisteredHashBounds = null;

            for (int x0 = startCoords.X, x1 = endCoords.X; x0 <= x1; x0++) 
            {
                for (int y0 = startCoords.Y, y1 = endCoords.Y; y0 <= y1; y0++) 
                {
                    Grid[x0, y0].Remove(collider);
                }
            }
        }
    }

    public void UpdateCollider(IHashable collider, Vector2 topLeftBounds, Vector2 bottomRightBounds) 
    {
        if (ColliderHasMovedCells()) 
        {
            Remove(collider);
            Insert(collider, topLeftBounds, bottomRightBounds);
        }

        bool ColliderHasMovedCells() 
        {
            var startCoords = GetCellIndex(topLeftBounds);
            var endCoords = GetCellIndex(bottomRightBounds);

            return collider.RegisteredHashBounds != (startCoords, endCoords);
        }
    }

    public IEnumerable<IHashable> FindNearByColliders(IHashable collider, int radius = 0)
    {
        queryBucket.Clear();

        if (collider.RegisteredHashBounds == null)
        {
            return queryBucket;
        }
        (CellIndex, CellIndex) colliderHashBounds = ((CellIndex, CellIndex))collider.RegisteredHashBounds;

        var start = new Point(
            Math.Max(0, colliderHashBounds.Item1.X - radius), 
            Math.Max(0, colliderHashBounds.Item1.Y - radius));

        var end = new Point(
            Math.Min(Width - 1, colliderHashBounds.Item2.X + radius),
            Math.Min(Height - 1, colliderHashBounds.Item2.Y + radius));

        int queryId = this.queryId++;

        for (int x0 = start.X, x1 = end.X; x0 <= x1; x0++)
        {
            for (int y0 = start.Y, y1 = end.Y; y0 <= y1; y0++)
            {
                foreach (IHashable coll in Grid[x0, y0])
                {
                    if (coll.QueryId != queryId && coll.RegisteredHashBounds != collider.RegisteredHashBounds)
                    {
                        coll.QueryId = queryId;
                        queryBucket.Add(coll);
                    }
                }
            }
        }
        return queryBucket;
    }
}
