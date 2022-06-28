using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Teuria;

[Obsolete("Everything in the World is unfinished")]
public interface IGridObject 
{
    Vector2 Position { get; }
    float Radius { get; }
}

[Obsolete("Everything in the World is unfinished")]
public class GridMap<T> where T : IGridObject
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int CellSize { get; private set; }
    private int columns;
    private int rows;
    private Dictionary<int, List<T>> buckets;

    public GridMap(int width, int height, int cellSize) 
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        columns = width / cellSize;
        rows = height / cellSize;

        buckets = new Dictionary<int, List<T>>();
        Reset();
    }

    public void ClearBuckets() 
    {
        buckets.Clear();
        Reset();
    }

    public void Register(T obj) 
    {
        var cellIds = GetID(obj);
        foreach (var id in cellIds) 
        {
            if (buckets.ContainsKey(id))
            buckets[id].Add(obj);
        }
    }

    public IEnumerable<T> GetNearby(T obj) 
    {
        List<T> objects = new List<T>();
        IEnumerable<int> bucketIds = GetID(obj);
        foreach (var id in bucketIds) 
        {
            if (buckets.ContainsKey(id))
            objects.AddRange(buckets[id]);
        }
        return objects;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Reset() 
    {
        for (int i = 0; i < columns * rows; i++) 
        {
            buckets.Add(i, new List<T>());
        }
    }

    private IEnumerable<int> GetID(T obj) 
    {
        var buckets = new List<int>();

        var minX = obj.Position.X - obj.Radius;
        var minY = obj.Position.Y - obj.Radius;
        var maxX = obj.Position.X + obj.Radius;
        var maxY = obj.Position.Y + obj.Radius;

        float width = Width / CellSize;

        AddBucket(minX, minY, width, buckets);
        AddBucket(maxX, minY, width, buckets);
        AddBucket(maxX, maxY, width, buckets);
        AddBucket(minX, maxY, width, buckets);

        return buckets;
        
    }

    private void AddBucket(float vecX, float vecY, float width, List<int> bucket) 
    {
        int cellPosition = (int)(
            (MathF.Floor(vecX / CellSize)) + 
            (MathF.Floor(vecY / CellSize)) * 
            width);

        if (!bucket.Contains(cellPosition))
            bucket.Add(cellPosition);
    }
}