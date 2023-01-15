using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public float ZIndex;
    public int Depth;
    public int Tags;
    public bool Visible = true;

    public override void EnterScene(Scene scene, ContentManager content) 
    {
        foreach (var comp in components) 
            comp.EntityEntered(scene);
        
        base.EnterScene(scene, content);
    }
    public override void ExitScene() 
    {
        foreach (var comp in components) 
        {
            comp.Removed();
            comp.EntityExited(Scene);
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
        if (!Visible) return;
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

    public void AddComponent<T>(T[] comps) 
    where T : Component
    {
        foreach (var comp in comps) 
        {
            components.Add(comp);
            comp.Added(this);
        }
    }


    public T GetComponent<T>() where T : Component
    {
        Span<Component> comps = CollectionsMarshal.AsSpan(components);
        foreach (var comp in comps) 
        {
            if (comp is T) 
            {
                return comp as T;
            }
        }
    
        return default;
    }

    public void RemoveComponent(Component comp) 
    {
        comp?.Removed();
        components.Remove(comp);
    }

    public IEnumerator<Component> GetEnumerator() => components.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}