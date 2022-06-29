using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Component : IComponent
{
    public Entity Entity;
    public bool Active { get; set; }

    public virtual void Added(Entity entity) 
    {
        Entity = entity;
        Active = true;
    }
    public virtual void Update() {}
    public virtual void Draw(SpriteBatch spriteBatch) {}
    public virtual void Removed() 
    {
        Entity = null;
    }

    public void Add(Component component) 
    {
        Entity.AddComponent(component);
    }

    public void Remove(Component component) 
    {
        Entity.RemoveComponent(component);
    }
}