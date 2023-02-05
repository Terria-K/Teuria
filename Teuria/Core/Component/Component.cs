using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Component
{
    public Scene Scene
    {
        get
        {
            if (this.Entity == null)
                return null;
            
            return this.Entity.Scene;
        }
    }
    public Entity Entity;
    public bool Active { get; set; }

    public virtual void Added(Entity entity) 
    {
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
        Entity.RemoveComponent(this);
    }
}