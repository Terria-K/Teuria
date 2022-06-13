using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Component 
{
    protected Entity Entity;
    public bool Active { get; set; } = true;

    public void Added(Entity entity) 
    {
        Entity = entity;
    }
    public virtual void Update() {}
    public virtual void Draw(SpriteBatch spriteBatch) {}
}