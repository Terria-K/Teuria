using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Entity : IEnumerable<Component>
{
    private List<Component> components = new List<Component>();
    private Scene scene;
    public bool Active { get; set; } = true;
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;
    public Color Modulate = Color.White;
    public PauseMode PauseMode = PauseMode.Inherit;
    public int ZIndex;

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
            if (!components[i].Active) continue;
            components[i].Update();
        }
    }
    public virtual void Draw(SpriteBatch spriteBatch) 
    {
        for (int i = 0; i < components.Count; i++) 
        {
            if (!components[i].Active) continue;
            components[i].Draw(spriteBatch);
        }
    }

    public void AddComponent(Component comp) 
    {
        components.Add(comp);
        comp.Added(this);
    }

    public T GetComponent<T>() where T : Component
    {
        foreach (var component in this) 
        {
            if (component is T) 
            {
                return (T)component;
            }
        }
        return null;
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

    public IEnumerator<Component> GetEnumerator() => components.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}