using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Teuria;

public abstract class PhysicsComponent : Component
{
    protected HashSet<PhysicsComponent> Collided = new HashSet<PhysicsComponent>();
    public bool Collideable;
    private readonly Shape collider;
    public Shape Collider => collider;
    public AABB BoundingArea => collider.BoundingArea;    


    public ICollidableEntity PhysicsEntity;

    public PhysicsComponent(Shape collider, bool collidable = true) 
    {
        Collideable = collidable;
        this.collider = collider;
    }

    public override void Added(Entity entity) 
    {
        base.Added(entity);
        if (entity is ICollidableEntity physicsEntity) 
        {
            PhysicsEntity = physicsEntity;
        }
        collider.Entity = entity;
    }

    public override void Removed() 
    {
        if (collider != null)
            collider.Entity = null;
        base.Removed();
    }

    public void Detect(HashSet<PhysicsComponent> components) 
    {
        Collided = components;
    }
    
    public bool Check(ICollidableEntity entity, Vector2 offset) 
    {
        foreach (var wall in Collided) 
        {
            if (!wall.Collideable) { continue; }
            if (entity.Collider.Equals(wall.Collider)) { continue; }
            if (entity.Collider.Collide(wall.Collider, offset)) 
            {
                return true;
            }
        }
        return false;
    }

    public bool Check(ICollidableEntity entity, ICollidableEntity entity2, Vector2 offset) 
    {
        if (entity.Collider.Equals(entity2)) { return false; }
        if (entity.Collider.Collide(entity2.Collider, offset)) 
        {
            return true;
        }
        return false;
    }

    public bool Check<T>(ICollidableEntity entity, Vector2 offset) 
    {
        foreach (var wall in Collided) 
        {
            if (!wall.Collideable) { continue; }
            if (entity.Collider.Equals(wall.Collider)) { continue; }
            if (entity.Collider.Collide(wall.Collider, offset)) 
            {
                if (wall.Entity is T) 
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool Check(ICollidableEntity entity, int tags, Vector2 offset) 
    {
        foreach (var wall in Collided) 
        {
            if (!wall.Collideable) { continue; }
            if (entity.Collider == wall.collider) { continue; }
            if (entity.Collider.Collide(wall.collider, offset)) 
            {
                if ((wall.collider.Tags & tags) != 0) { return true; }
            }
        }
        return false;
    }

    public bool Check(ICollidableEntity entity, string groupName, Vector2 offset) 
    {
        foreach (var wall in Collided) 
        {
            if (!wall.Collideable) { continue; }
            if (entity.Collider == wall.Collider) { continue; }
            if (entity.Collider.Collide(wall.Collider, offset)) 
            {
                if (wall.Collider.GroupName == groupName) { return true; }
            }
        }
        return false;
    }

    public bool Check<T>(ICollidableEntity entity, string groupName, Vector2 offset, out T ent) 
    where T : ICollidableEntity
    {
        foreach (var wall in Collided) 
        {
            if (!wall.Collideable) { continue; }
            if (entity.Collider.Equals(wall.Collider)) { continue; }
            if (entity.Collider.Collide(wall.Collider, offset)) 
            {
                if (wall.Collider.GroupName == groupName) 
                {
                    ent = (T)wall.PhysicsEntity;
                    return true;
                }
                continue;
            }
        }
        ent = default;
        return false;
    }

    public bool Check<T>(ICollidableEntity entity, int tags, Vector2 offset, out T ent) 
    where T : ICollidableEntity
    {
        foreach (var wall in Collided) 
        {
            if (!wall.Collideable) { continue; }
            if (entity.Collider.Equals(wall.Collider)) { continue; }
            if (entity.Collider.Collide(wall.Collider, offset)) 
            {
                if ((wall.Collider.Tags & tags) !=  0)
                {
                    ent = (T)wall.PhysicsEntity;
                    return true;
                }
                continue;
            }
        }
        ent = default;
        return false;
    }

    public bool Check<T1>(ICollidableEntity entity, Vector2 offset, out T1 ent) 
    where T1 : ICollidableEntity
    {
        foreach (var wall in Collided) 
        {
            if (!wall.Collideable) { continue; }
            if (entity.Collider.Equals(wall.Collider)) { continue; }
            if (entity.Collider.Collide(wall.Collider, offset)) 
            {
                if (wall.Entity is T1) 
                {
                    ent = (T1)wall.PhysicsEntity;
                    return true;
                }
                continue;
            }
        }
        ent = default;
        return false;
    }


}