using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class QuadTree<T> where T : PhysicsComponent 
{
    private const int MaxObjects = 10;
    private const int MaxLevels = 1;

    private int level;
    private List<T> physicsComponents = new List<T>();
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
        var verticalMidpoint = bounds.X + (bounds.Width * 0.5f);
        var horizontalMidpoint = bounds.Y + (bounds.Height * 0.5f);

        var bottomQuadrant = rect.Y < verticalMidpoint && rect.Y + rect.Height < verticalMidpoint;
        var topQuadrant = rect.Y > verticalMidpoint;

        if (rect.X < horizontalMidpoint && rect.X + rect.Width < horizontalMidpoint) 
        {
            if (bottomQuadrant) 
                quad = Quadrant.BottomLeft;
            else if (topQuadrant)
                quad = Quadrant.TopLeft;
        }
        else if (rect.X > horizontalMidpoint) 
        {
            if (bottomQuadrant)
                quad = Quadrant.BottomRight;
            else if (topQuadrant)
                quad = Quadrant.TopRight;
        }
        return quad;
    }

    public void Insert(HashSet<T> colliders) 
    {
        foreach (var collider in colliders) 
        {
            if (collider.Entity == null) 
            {
                colliders.Remove(collider);
                continue;
            }
            Insert(collider);
        }
    }

    public void Insert(IEnumerable<T> colliders) 
    {
        foreach (var collider in colliders) 
        {
            Insert(collider);
        }
    }

    public void Insert(T physicsComponent)
    {
        if (HasNodes)
        {
            var bArea = physicsComponent.BoundingArea;
            var quadrant = GetQuadrant(bArea);
            if (quadrant != Quadrant.None)
            {
                nodes[(int)quadrant].Insert(physicsComponent);
                return;
            }
        }

        physicsComponents.Add(physicsComponent);

        if (physicsComponents.Count <= MaxObjects || level >= MaxLevels) { return; }
        if (!HasNodes)
        {
            Split();
        }

        for (int i = 0; i < physicsComponents.Count; i++) 
        {
            var col = physicsComponents[i];
            var boundingArea = col.BoundingArea;
            var quadrant = GetQuadrant(boundingArea);

            if (quadrant != Quadrant.None) 
            {
                nodes[(int)quadrant].Insert(col);
                physicsComponents.Remove(col);
            }
        }
    }

    public IEnumerable<T> Retrieve(T physicsComponent) 
    {
        return Retrieve(physicsComponent.BoundingArea);
    }

    public IEnumerable<T> Retrieve(AABB boundingBox) 
    {
        var col = new List<T>(physicsComponents);
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
        physicsComponents.Clear();

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
                (texture2D, new Rectangle((int)node.bounds.Left, (int)node.bounds.Top, 1, (int)node.bounds.Height), color);
            spriteBatch.Draw
                (texture2D, new Rectangle((int)node.bounds.Left, (int)node.bounds.Top, (int)node.bounds.Width, 1), color);
        }
    }
}