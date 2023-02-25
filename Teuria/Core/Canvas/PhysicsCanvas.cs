using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class PhysicsCanvas : CanvasLayer
{
    private readonly HashSet<PhysicsComponent> physicsComponents = new HashSet<PhysicsComponent>();
    private bool isClearing;

    public PhysicsCanvas() 
    {
    }

    public void Add(ICollidableEntity entity) 
    {
        entity.Collider.IsInTheWorld = true;
        Add(entity.PhysicsComponent);
    }

    public void Add(PhysicsComponent component) 
    {
        physicsComponents.Add(component);
    }

    public void UpdatePhysics() 
    {
        if (isClearing) { return; }
        foreach (var physicsComponent in physicsComponents) 
        {
            if (physicsComponent.Entity is null) 
            {
                physicsComponents.Remove(physicsComponent);
                continue;
            }
            physicsComponent.Detect(physicsComponents);
        }
    }

    public void ClearAll() 
    {
        isClearing = true;
        physicsComponents.Clear();
        isClearing = false;
    }

    public void ClearAllExceptWithTags(int tags) 
    {
        isClearing = true;
        foreach (var comp in physicsComponents) 
        {
            if (comp.Entity == null) 
            {
                physicsComponents.Remove(comp);
                continue;
            }

            if ((comp.Entity.Tags & tags) != 0) 
                continue;
            physicsComponents.Remove(comp);
        }
        isClearing = false;
    }

    public void Remove(ICollidableEntity entity) 
    {
        physicsComponents.Remove(entity.PhysicsComponent);
    }

    public override void Draw(Scene scene, SpriteBatch spriteBatch)
    {
#if DEBUG
#endif
    }

    public override void Unload()
    {
        base.Unload();
    }
}
