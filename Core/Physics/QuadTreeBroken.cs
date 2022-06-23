using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public sealed class QuadTreeBoken<TNode> where TNode : IPhysicsEntity
{
    private readonly AABB boundary;
    private readonly int capacity;
    private readonly List<TNode> entities = new();
    private readonly QuadTreeBoken<TNode>[] quadTrees = new QuadTreeBoken<TNode>[4];
    private bool divided;

    private const int NW = 0;
    private const int NE = 1;
    private const int SW = 2;
    private const int SE = 3;


    public QuadTreeBoken<TNode> BottomLeft 
    {
        get => quadTrees[(int)Quadrant.BottomLeft];
        set => quadTrees[(int)Quadrant.BottomLeft] = value;
    }
    public QuadTreeBoken<TNode> BottomRight 
    {
        get => quadTrees[(int)Quadrant.BottomRight];
        set => quadTrees[(int)Quadrant.BottomRight] = value;
    }

    public QuadTreeBoken<TNode> TopLeft 
    {
        get => quadTrees[(int)Quadrant.TopLeft];
        set => quadTrees[(int)Quadrant.TopRight] = value;
    }

    public QuadTreeBoken<TNode> TopRight 
    {
        get => quadTrees[(int)Quadrant.TopRight];
        set => quadTrees[(int)Quadrant.TopRight] = value;
    }

    public QuadTreeBoken(AABB boundary, int capacity, int depth = 5)
    {
        this.boundary = boundary;
        this.capacity = capacity;
    }

    public void Split() 
    {
        var subWidth = boundary.Width / 2;
        var subHeight = boundary.Height / 2;
        var x = boundary.X;
        var y = boundary.Y;


        var bottomRight = new AABB(x + subWidth, y, subWidth, subHeight);
        quadTrees[(int)Quadrant.BottomRight] = new QuadTreeBoken<TNode>(bottomRight, capacity);

        var bottomLeft = new AABB(x, y, subWidth, subHeight);
        quadTrees[(int)Quadrant.BottomLeft] = new QuadTreeBoken<TNode>(bottomLeft, capacity);

        var topLeft = new AABB(x, y + subHeight, subWidth, subHeight);
        quadTrees[SE] = new QuadTreeBoken<TNode>(topLeft, capacity);
        
        var topRight = new AABB(x + subWidth, y + subHeight, subWidth, subHeight);
        quadTrees[SW] = new QuadTreeBoken<TNode>(topRight, capacity);
        divided = true;
    }

    public void Reset()
    {
        entities.Clear();

        if (quadTrees[0] == null) 
            return;
        
        foreach (var quadTree in quadTrees)
        {
            quadTree.Reset();
        }
    }

    public void Insert(TNode entity)
    {
        if (divided) 
        {
            var area = entity.BoundingArea;
            var quadrant = GetQuadrant(area);

            if (quadrant != Quadrant.None)
            {
                quadTrees[(int)quadrant].Insert(entity);
                return;
            }
        }
        

        if (entities.Count < capacity)
        {
            entities.Add(entity);
            return;
        }
        if (!divided) 
        {
            Split();
        }
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            var collider = entities[i];
            var colArea = collider.BoundingArea;
            var quad = GetQuadrant(colArea);

            if (quad == Quadrant.None)
            {
                continue;
            }
            quadTrees[(int)quad].Insert(collider);
            entities.Remove(collider);
        }
    }

    public void Insert(IEnumerable<TNode> points) 
    {
        foreach (var point in points) 
        {
            Insert(point);
        }
    }

    public List<TNode> Retrieve(AABB range)
    {
        var col = new List<TNode>(entities);
        var quadrant = GetQuadrant(range);
        if (quadrant == Quadrant.None)
        {
            return col;
        }
        col.AddRange(quadTrees[(int)quadrant].Retrieve(range));
        return col;
    }

    public List<TNode> Retrieve(TNode entity) 
    {
        return Retrieve(entity.BoundingArea);
    }

    private Quadrant GetQuadrant(AABB boundingArea) 
    {
        var x = boundingArea.X;
        var y = boundingArea.Y;
        var width = boundingArea.Width;
        var height = boundingArea.Height;

        var verticalMidpoint = x + height /2;
        var horizontalMidpoint = x + width /2;
        var isBottom = y < verticalMidpoint && y + height < verticalMidpoint;
        var isTop = y > verticalMidpoint;

        if (x < horizontalMidpoint && x + width < horizontalMidpoint) 
        {
            if (isBottom) 
            {
                return Quadrant.BottomLeft;
            }
            if (isTop) 
            {
                return Quadrant.TopLeft;
            }
            return Quadrant.None;
        }
        if (x > horizontalMidpoint) 
        {
            if (isBottom) 
            {
                return Quadrant.BottomRight;
            }
            if (isTop) 
            {
                return Quadrant.TopRight;
            }
        }

        return Quadrant.None;
    }

#if DEBUG
    [Conditional("DEBUG")]
    public void ShowBoundaries(SpriteBatch spriteBatch, Texture2D texture, Color color) 
    {
        spriteBatch.Draw(texture, new Vector2(boundary.X, boundary.Y), boundary, color);
        if (divided) 
        {
            BottomLeft.ShowBoundaries(spriteBatch, texture, Color.Red);
            BottomRight.ShowBoundaries(spriteBatch, texture, Color.Blue);
            TopRight.ShowBoundaries(spriteBatch, texture, Color.Green);
            TopLeft.ShowBoundaries(spriteBatch, texture, Color.Orange);
        }
    }

    [Conditional("DEBUG")]
    public void ShowHitboxes(SpriteBatch spriteBatch, Texture2D texture, Color color) 
    {
        foreach (var point in entities)
        {
            spriteBatch.Draw(texture, new Vector2(point.BoundingArea.X, point.BoundingArea.Y), new Rectangle
                ((int)point.BoundingArea.X, (int)point.BoundingArea.Y, (int)point.BoundingArea.Width, (int)point.BoundingArea.Height), color);
        }


        if (divided) 
        {
            BottomLeft.ShowHitboxes(spriteBatch, texture, Color.Red);
            BottomRight.ShowHitboxes(spriteBatch, texture, Color.Blue);
            TopRight.ShowHitboxes(spriteBatch, texture, Color.Green);
            TopLeft.ShowHitboxes(spriteBatch, texture, Color.Orange);
        }
    }
#endif
}