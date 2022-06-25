using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class QuadTree<T> where T : IPhysicsEntity
{
    private const int MaxObjects = 10;
    private const int MaxLevels = 7;

    private int level;
    private List<T> entities = new List<T>();
    private AABB bounds;
    private QuadTree<T>[] nodes = new QuadTree<T>[4];

    public bool HasNodes => nodes[0] != null;

    public QuadTree(int level, AABB bounds) 
    {
        this.level = level;
        this.bounds = bounds;
    }

    private void Split() 
    {
        float subWidth = bounds.Width / 2;
        float subHeight = bounds.Height / 2;
        float x = bounds.X;
        float y = bounds.Y;

        nodes[(int)Quadrant.BottomLeft] = new QuadTree<T>(level + 1, new AABB(x + subWidth, y, subWidth, subHeight));
        nodes[(int)Quadrant.BottomRight] = new QuadTree<T>(level + 1, new AABB(x, y, subWidth, subHeight));
        nodes[(int)Quadrant.TopLeft] = new QuadTree<T>(level + 1, new AABB(x, y + subHeight, subWidth, subHeight));
        nodes[(int)Quadrant.TopRight] = new QuadTree<T>(level + 1, new AABB(x + subWidth, y + subHeight, subWidth, subHeight));
    }

    private Quadrant GetQuadrant(AABB rect) 
    {
        var quad = Quadrant.None;
        float verticalMidpoint = bounds.X + (bounds.Width / 2);
        float horizontalMidpoint = bounds.Y + (bounds.Height / 2);

        bool topQuadrant = 
            rect.Y < horizontalMidpoint && 
            rect.Y + rect.Height < horizontalMidpoint;
        bool bottomQuadrant = rect.Y > horizontalMidpoint;

        if (rect.X < verticalMidpoint && bounds.X + rect.Width < verticalMidpoint) 
        {
            if (topQuadrant) 
            {
                quad = Quadrant.BottomRight;
            }
            else if (bottomQuadrant) 
            {
                quad = Quadrant.TopLeft;
            }
        }
        else if (rect.X > verticalMidpoint) 
        {
            if (topQuadrant) 
            {
                quad = Quadrant.BottomLeft;
            }
            else if (bottomQuadrant) 
            {
                quad = Quadrant.TopRight;
            }
        }
        return quad;
    }

    public void Insert(IEnumerable<T> colliders) 
    {
        foreach (var collider in colliders) 
        {
            Insert(collider);
        }
    }

    public void Insert(T physicsEntity)
    {
        if (HasNodes)
        {
            var quadrant = GetQuadrant(physicsEntity.Collider.BoundingArea);
            if (quadrant != Quadrant.None)
            {
                nodes[(int)quadrant].Insert(physicsEntity);
                return;
            }
        }

        entities.Add(physicsEntity);

        if (entities.Count <= MaxObjects || level >= MaxLevels) { return; }
        if (!HasNodes)
        {
            Split();
        }

        for (int i = 0; i < entities.Count; i++) 
        {
            var col = entities[i];
            var boundingArea = col.Collider.BoundingArea;
            var quadrant = GetQuadrant(boundingArea);

            if (quadrant != Quadrant.None) 
            {
                nodes[(int)quadrant].Insert(col);
                entities.Remove(col);
            }
        }
    }

    public IEnumerable<T> Retrieve(T entity) 
    {
        return Retrieve(entity.Collider.BoundingArea);
    }

    public IEnumerable<T> Retrieve(AABB boundingBox) 
    {
        var col = new List<T>(entities);
        if (HasNodes) 
        {
            var quadrant = GetQuadrant(boundingBox);
            if (quadrant != Quadrant.None) 
            {
                col.AddRange(nodes[(int)quadrant].Retrieve(boundingBox));
            }
        }
        return col;
    }

    public void Clear() 
    {
        entities.Clear();

        if (!HasNodes) return;
        foreach (var node in nodes) 
        {
            node.Clear();
        }
    }

    public void ShowBoundaries(SpriteBatch spriteBatch, Texture2D texture2D, Color color) 
    {
        foreach (var node in nodes) 
        {
            if (node == null) return;
            node.ShowBoundaries(spriteBatch, texture2D, color);
            spriteBatch.Draw
                (texture2D, new Rectangle((int)node.bounds.Left - 1, (int)node.bounds.Top - 1, (int)node.bounds.Width + 2, 1), color);
            spriteBatch.Draw
                (texture2D, new Rectangle((int)node.bounds.Left - 1, (int)node.bounds.Top - 1, 1, (int)node.bounds.Height + 2), color);
        }
    }
}