using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class QuadTree<T> where T : IPhysicsEntity
{
    private const int MaxObjects = 4;
    private const int MaxLevels = 7;

    private int level;
    private List<T> entities = new List<T>();
    private List<T> returnEntities = new List<T>();
    private AABB bounds;
    private QuadTree<T>[] nodes = new QuadTree<T>[4];
    public Action<T> Modify;

    public List<T> VisibleEntities 
    {
        get => returnEntities;
    }

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
        Quadrant quad = Quadrant.None;
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

    public void Insert(IEnumerable<T> entities) 
    {
        foreach (var entity in entities) 
        {
            Insert(entity);
        }
    }

    public void Insert(T entity) 
    {
        if (nodes[0] != null)
        {
            var quadrant = GetQuadrant(entity.BoundingArea);
            if (quadrant != Quadrant.None)
            {
                nodes[(int)quadrant].Insert(entity);
                return;
            }
        }

        entities.Add(entity);

        if (entities.Count > MaxObjects && level < MaxLevels) 
        {
            if (nodes[0] == null) 
            {
                Split();
            }

            int i = 0;
            while (i < entities.Count) 
            {
                T ent = entities[i];
                var quadrant = GetQuadrant(ent.BoundingArea);
                if (quadrant != Quadrant.None) 
                {
                    nodes[(int)quadrant].Insert(ent);
                    entities.RemoveAt(i);
                }
                else {
                    i++;
                }
            }
        }
    }

    public void Retrieve(List<T> returnedEntities, T entity)
    {
        if (nodes[0] != null)
            NodeRetrieve(returnedEntities, entity);
        returnedEntities.AddRange(entities);
    }

    private void NodeRetrieve(List<T> returnedEntities, T entity)
    {
        var quadrant = GetQuadrant(entity.BoundingArea);
        if (quadrant != Quadrant.None)
        {
            nodes[(int)quadrant].Retrieve(returnedEntities, entity);
            return;
        }
        foreach (var node in nodes)
        {
            node.Retrieve(returnedEntities, entity);
        }
    }

    public void Clear() 
    {
        entities.Clear();

        foreach (var node in nodes) 
        {
            node?.Clear();
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

    public void Insertion(List<T> entities) 
    {
        Clear();
        Insert(entities);

        for (int i1 = 0; i1 < entities.Count; i1++) 
        {
            T entity = entities[i1];
            returnEntities.Clear();
            Retrieve(returnEntities, entity);

            for (int i = 0; i < returnEntities.Count; i++) 
            {
                T returnEntity = returnEntities[i];
                if (entity.BoundingArea.Equals(returnEntity.BoundingArea)) { continue; }
                if (entity.BoundingArea.Contains(returnEntity.BoundingArea)) 
                {
                    Modify?.Invoke(entity);
                }
            }
        }
    }
}