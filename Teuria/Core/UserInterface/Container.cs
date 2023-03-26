using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

[Obsolete]
public class Container<T> : Entity 
where T : Entity 
{
    private List<T> items = new List<T>();
    private readonly float offset = 0;
    public new bool Active 
    { 
        get => base.Active; 
        set 
        {
            foreach(var entity in items) 
            {
                entity.Active = value;
            }
            base.Active = value;
        } 
    }

    public Container(Vector2 position, float alignOffset = 0) 
    {
        Position = position;
        this.offset = alignOffset;
    }

    public override void Ready()
    {
        var dynamicOffset = offset;
        foreach (var entity in items) 
        {
            entity.Position = new Vector2(Position.X, Position.Y - dynamicOffset);
            dynamicOffset += offset;
        }
        base.Ready();
    }

    public void AddItem(T item) 
    {
        items.Add(item);
    }

    public void RemoveItem(T item) 
    {
        items.Remove(item);
    }
}