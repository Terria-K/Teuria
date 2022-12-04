using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Node 
{
    protected internal Node parent;
    private string name;
    private Dictionary<string, Node> childs = new Dictionary<string, Node>();
    public bool Active { get; set; } = true;
    public PauseMode PauseMode = PauseMode.Inherit;
    public int NodeID { get; internal set; }
    public Scene Scene;

    public Node(string name = "Node") 
    {
        this.name = name;
        NodeID = TeuriaEngine.InternalID;
        TeuriaEngine.InternalID++;
    }

    public void AddChild(string name, Node node) 
    {
        if (!string.IsNullOrEmpty(this.name)) 
        {
            name = $"{this.name}/{name}";
        }
        childs.Add(name, node);
        node.name = name;
        node.parent = this;
    }

    public void RemoveChild(string nodePath) 
    {
        var node = childs[nodePath];
        childs.Remove(nodePath);
    }

    public T GetNode<T>(string nodePath) where T : Node
    {
        return childs[nodePath] as T;
    }

    // public void Free() 
    // {
    //     Scene.Remove(this);
    //     foreach (var child in childs) 
    //     {
    //         Scene.Remove(child.Value);
    //     }
    // }

    public void QueueFree() 
    {
        Scene.AddToQueue(this);
    }

    public virtual void EnterScene(Scene scene, ContentManager content) 
    {
        this.Scene = scene;
    }
    public virtual void ExitScene() 
    {
        foreach (var child in childs) 
        {
            RemoveChild(child.Key);            
        }
    }
    public virtual void Ready() {}
    public virtual void Update() 
    {
        
    }
    public virtual void Draw(SpriteBatch spriteBatch) 
    {
       
    }

    public override string ToString()
    {
        return $"[{GetType().Name} {NodeID}]";
    }
}