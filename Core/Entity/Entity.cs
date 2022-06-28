using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Entity 
{
    private List<IComponent> components = new List<IComponent>();
    private Scene scene;
    public bool Active { get; set; } = true;
    public Vector2 Position;
    public Color Modulate = Color.White;
    public int ZIndex;
    public PauseMode PauseMode = PauseMode.Inherit;

    public virtual void EnterScene(Scene scene, ContentManager content) 
    {
        this.scene = scene;
    }
    public virtual void ExitScene() 
    {
        for (int i = 0; i < components.Count; i++) 
        {
            components[i].Removed();
        }
    }
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

    public void RemoveComponent(Component comp) 
    {
        components.Remove(comp);
        comp.Removed();
    }

    public void Free() 
    {
        scene.Remove(this);
    }

    public void QueueFree() 
    {
        scene.AddToQueue(this);
    }
}