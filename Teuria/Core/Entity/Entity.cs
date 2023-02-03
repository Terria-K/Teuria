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
    private List<Component> componentList = new List<Component>();
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
        foreach (var comp in componentList) 
            comp.EntityEntered(scene);
        
        base.EnterScene(scene, content);
    }
    public override void ExitScene() 
    {
        foreach (var comp in componentList) 
        {
            comp.Removed();
            comp.EntityExited(Scene);
        }
        base.ExitScene();
    }
    public override void Ready() {}
    public override void Update() 
    {
        for (int i = 0; i < componentList.Count; i++) 
        {
            if (!componentList[i].Active) continue;
            componentList[i].Update();
        }
    }
    public override void Draw(SpriteBatch spriteBatch) 
    {
        if (!Visible) return;
        for (int i = 0; i < componentList.Count; i++) 
        {
            if (!componentList[i].Active) continue;
            componentList[i].Draw(spriteBatch);
        }
    }

    public void AddComponent(Component comp) 
    {
        componentList.Add(comp);
        comp.Added(this);
    }

    public void AddComponent<T>(T[] comps) 
    where T : Component
    {
        foreach (var comp in comps) 
        {
            AddComponent(comp);
        }
    }


    public T GetComponent<T>() where T : Component
    {
        Span<Component> comps = CollectionsMarshal.AsSpan(componentList);
        foreach (var comp in comps) 
        {
            if (comp is T c) 
            {
                return c;
            }
        }
    
        return default;
    }

    public void RemoveComponent(Component comp) 
    {
        comp?.Removed();
        componentList.Remove(comp);
    }

    public IEnumerator<Component> GetEnumerator() => componentList.GetEnumerator();
    

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}