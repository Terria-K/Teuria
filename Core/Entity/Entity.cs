using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Entity : Node, IEnumerable<Component>
{
    private List<Component> components = new List<Component>();
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;
    public Color Modulate = Color.White;
    public int ZIndex;

    public override void EnterScene(Scene scene, ContentManager content) 
    {
        base.EnterScene(scene, content);
    }
    public override void ExitScene() 
    {
        for (int i = 0; i < components.Count; i++) 
        {
            components[i].Removed();
        }
        base.ExitScene();
    }
    public override void Ready() {}
    public override void Update() 
    {
        for (int i = 0; i < components.Count; i++) 
        {
            if (!components[i].Active) continue;
            components[i].Update();
        }
    }
    public override void Draw(SpriteBatch spriteBatch) 
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

    public IEnumerator<Component> GetEnumerator() => components.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}