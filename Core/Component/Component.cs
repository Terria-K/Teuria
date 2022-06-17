using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Component : IComponent
{
    protected Entity Entity;
    public bool Active { get; set; }

    public void Added(Entity entity) 
    {
        Entity = entity;
        Active = true;
    }
    public virtual void Update() {}
    public virtual void Draw(SpriteBatch spriteBatch) {}
    public virtual void Removed() {}
}