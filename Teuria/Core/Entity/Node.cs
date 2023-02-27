using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Node 
{
    public string Name;
    private Dictionary<string, Node> childs = new Dictionary<string, Node>();
    public bool Active { get; set; } = true;
    public PauseMode PauseMode = PauseMode.Inherit;
    public int NodeID { get; internal set; }
    public Scene? Scene;

    public Node(string name = "Node") 
    {
        this.Name = name;
        NodeID = GameApp.InternalID;
        GameApp.InternalID++;
    }

    public void AddChild(string name, Node node) 
    {
        if (!string.IsNullOrEmpty(this.Name)) 
        {
            name = $"{this.Name}/{name}";
        }
        childs.Add(name, node);
        node.Name = name;
    }

    public bool TryAddChild(string name, Node node) 
    {
        if (!string.IsNullOrEmpty(this.Name)) 
        {
            name = $"{this.Name}/{name}";
        }
        if (childs.TryAdd(name, node)) 
        {
            node.Name = name;
            return true;
        }
        return false;
    }

    public void RemoveChild(string nodePath) 
    {
        var node = childs[nodePath];
        childs.Remove(nodePath);
    }

    public T? GetNode<T>(string nodePath) where T : Node
    {
        return childs[nodePath] as T;
    }

    public T? TryGetNode<T>(string nodePath) where T : Node 
    {
        if (childs.TryGetValue(nodePath, out Node? value)) 
        {
            return (T)value;
        }
        return null;
    }

    // public void Free() 
    // {
    //     Scene.Remove(this);
    //     foreach (var child in childs) 
    //     {
    //         Scene.Remove(child.Value);
    //     }
    // }

    public virtual void EnterScene(Scene scene, ContentManager content) 
    {
        this.Scene = scene;
    }
    public virtual void ExitScene(Scene scene) 
    {
        foreach (var child in childs) 
        {
            RemoveChild(child.Key);            
        }
    }
    public virtual void Ready() {}
    public virtual void Update() {}
    public virtual void Draw(SpriteBatch spriteBatch) {}
    // TODO Handle it with InputEvent
    public virtual void Input() {}

    public override string ToString()
    {
        return $"[{GetType().Name} {NodeID}]";
    }
}