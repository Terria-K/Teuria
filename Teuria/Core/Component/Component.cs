using System;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Component
{
    internal static ulong id;
    public ulong ID { get; internal set; }
    public Scene? Scene
    {
        get
        {
            if (this.Entity == null)
                throw new EntityDoesNotExistException();
            
            return this.Entity.Scene;
        }
    }
    public Entity? Entity;
    public bool Active { get; set; }

    public Component() {}

    public virtual void Added(Entity entity) 
    {
        ID = id++;
        Entity = entity;
        Active = true;
    }

    public virtual void EntityEntered(Scene scene) 
    {
    }

    public virtual void EntityExited(Scene scene) 
    {
    }
    
    public virtual void Update() {}
    public virtual void Draw(SpriteBatch spriteBatch) {}
    public virtual void Removed() 
    {
        Entity = null;
    }

    public void DetachSelf() 
    {
        Entity?.RemoveComponent(this);
    }

    public override string ToString()
    {
        var typeName = GetType().Name;
        return $"[{typeName} {ID}]";
    }
}

public class EntityDoesNotExistException : Exception 
{
    public EntityDoesNotExistException() {}
    public EntityDoesNotExistException(string? message) : base(message) 
    {

    }
}