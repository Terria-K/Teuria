using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Entity 
{
    private List<Component> components = new List<Component>();
    public bool Active { get; set; } = true;
    public Vector2 Position;
    public virtual Rectangle Rectangle { get; set; }
    public Color Modulate = Color.White;

    public Rectangle SpriteFixedRectangle(Sprite sprite) 
    {
        return new Rectangle((int)Position.X, (int)Position.Y, sprite.texture.Width, sprite.texture.Height); 
    }

    public virtual void EnterScene() {}
    public virtual void Ready() {}
    public virtual void Update() 
    {
        for (int i = 0; i < components.Count; i++) 
        {
            if (!components[i].Active) return;
            components[i].Update();
        }
    }
    public virtual void Draw(SpriteBatch spriteBatch) 
    {
        for (int i = 0; i < components.Count; i++) 
        {
            if (!components[i].Active) return;
            components[i].Draw(spriteBatch);
        }
    }

    public void AddComponent(Component comp) 
    {
        components.Add(comp);
        comp.Added(this);
    }
}