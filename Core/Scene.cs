using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Scene 
{
    internal Queue<Node> QueueToFree = new Queue<Node>();
    private List<Node> nodeList = new List<Node>();
    private Entities entityList;
    private List<CanvasLayer> layers = new List<CanvasLayer>();
    private Layers layerList;
    protected ContentManager Content;
    protected Camera Camera;
    protected SpriteBatch SpriteBatch;
    public event Action OnPause;
    public SceneCanvas MainCanvas;
    private bool paused;
    public bool Paused 
    {
        get => paused;
        set
        {
            paused = value;
            if (paused)
                OnPause?.Invoke();
        }
    }

    public Entities Entities 
    {
        get => entityList;
    }

    public Scene(ContentManager content, Camera camera) 
    {
        Content = content;
        Camera = camera;
        entityList = new Entities(this);
        layerList = new Layers(this);
    }

    public Scene(ContentManager content)
    {
        Content = content;
        entityList = new Entities(this);
        layerList = new Layers(this);
    }

    public bool HasCanvas(CanvasLayer canvas) 
    {
        return layers.Contains(canvas);
    }

    internal void Activate(SpriteBatch spriteBatch) 
    {
        this.SpriteBatch = spriteBatch;
    }

    internal void AddToQueue(Node entity) 
    {
        QueueToFree.Enqueue(entity);
    }

    public void Add(Entity entity, PauseMode pauseMode = PauseMode.Inherit) 
    {
        entity.PauseMode = pauseMode;
        entityList.Add(entity);
        // nodeList.Add(entity);
        // entity.EnterScene(this, Content);
    }

    public void Add(CanvasLayer layer) 
    {
        layer.Obtain(this);
        layerList.Add(layer);
        // layer.Obtain(SpriteBatch, this);
        // layers.Add(layer);
    }

    public void Remove(CanvasLayer layer) 
    {
        layerList.Remove(layer);
        layers.Remove(layer);
    }

    public void Remove(Entity entity) 
    {
        // entity.Active = false;
        // entity.ExitScene();
        entityList.Remove(entity);
        // nodeList.Remove(entity);
    }

    public void RemoveAllEntities() 
    {
        entityList.Clear();
        // nodeList.Clear();
    }

    public void RemoveAllEntitiesByFilter(Func<Entity, bool> predicate) 
    {
        foreach (var entity in entityList) 
        {
            if (predicate(entity)) 
            {
                entityList.Remove(entity);
            }
        }
    }

    public virtual void Initialize() {}
    public virtual void Hierarchy(GraphicsDevice device) 
    {
        // foreach(var entity in nodeList) 
        // {
        //     entity.Ready();
        // }
    }
    public virtual void ProcessLoop() 
    {
        // if (QueueToFree.Count > 0)
        //     QueueToFree.Dequeue().Free();
        layerList.UpdateLists();
        if (Paused) 
        {
            ProcessEntityInPauseMode();
            return;
        }
        entityList.UpdateSystem();
        entityList.Update();

        // foreach(var entity in nodeList) 
        // {
        //     if (!entity.Active) continue;
        //     entity.Update();
        // }
    }

    private void ProcessEntityInPauseMode() 
    {
        // foreach(var entity in nodeList) 
        // {
        //     if (!entity.Active) continue;
        //     if (entity.PauseMode == PauseMode.Single)
        //         entity.Update();
        // }
    }

    public virtual void BeforeRender() 
    {
        layerList.PreDraw();
    }

    public virtual void Render() 
    {
        // entityList.Draw(SpriteBatch);
        // foreach(var entity in nodeList) 
        // {
        //     if (!entity.Active) continue;
        //     entity.Draw(SpriteBatch);
        // }
        layerList.Draw();
    }

    public virtual void Exit() 
    {
        layerList.Unload();
    }

    internal ContentManager GetContent() 
    {
        return Content;
    }
}