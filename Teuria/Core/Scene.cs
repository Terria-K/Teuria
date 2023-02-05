using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;


public class Scene 
{
    private Entities entityList;
    private Canvases canvasList;
    protected ContentManager Content;
    public Camera Camera;
    public SpriteBatch SpriteBatch;
    public float TimeActive;
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

    public Entities Entities => entityList;
    public Action OnPause;
    public Action<Entity> OnEntityCreated;
    public Action<Entity> OnEntityDeleted;

    public Scene(ContentManager content, Camera camera) 
    {
        Content = content;
        Camera = camera;
        entityList = new Entities(this);
        canvasList = new Canvases(this);
    }

    public Scene(ContentManager content)
    {
        Content = content;
        entityList = new Entities(this);
        canvasList = new Canvases(this);
    }

    public bool HasCanvas(CanvasLayer canvas) 
    {
        return canvasList.CanvasList.Contains(canvas);
    }

    internal void Activate(SpriteBatch spriteBatch) 
    {
        SpriteBatch = spriteBatch;
    }

    public void Add(Entity entity, PauseMode pauseMode = PauseMode.Inherit) 
    {
        entity.PauseMode = pauseMode;
        Add(entity);
    }

    public void Add(Entity entity) 
    {
        entityList.Add(entity);
    }

    public void Add<T>(List<T> entities) 
    where T : Entity
    {
        foreach (var entity in entities) 
        {
            entityList.Add(entity);
        }
    }

    public void Add(CanvasLayer layer) 
    {
        layer.Obtain(this);
        canvasList.Add(layer);
    }

    public void Remove(CanvasLayer layer) 
    {
        canvasList.Remove(layer);
    }

    public void Remove(Entity entity) 
    {
        entityList.Remove(entity);
    }

    public void RemoveAllEntities() 
    {
        entityList.Clear();
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
    public void RemoveAllEntitiesExcludeByTags(int tags) 
    {
        foreach (var entity in entityList) 
        {
            if ((entity.Tags & tags) == 0) 
            {
                entityList.Remove(entity);
            }
        }
    }

    public virtual void Initialize() {}
    public virtual void Hierarchy(GraphicsDevice device) {}
    public virtual void Process() 
    {
        if (!Paused)
            TimeActive += Time.Delta;
        // if (QueueToFree.Count > 0)
        //     QueueToFree.Dequeue().Free();
        canvasList.UpdateLists();
        entityList.UpdateSystem();
        entityList.Update();

        // foreach(var entity in nodeList) 
        // {
        //     if (!entity.Active) continue;
        //     entity.Update();
        // }
    }

    public virtual void BeforeRender() 
    {
        canvasList.PreDraw();
    }

    public virtual void Render() 
    {
        // entityList.Draw(SpriteBatch);
        // foreach(var entity in nodeList) 
        // {
        //     if (!entity.Active) continue;
        //     entity.Draw(SpriteBatch);
        // }
        canvasList.Draw();
    }

    public virtual void AfterRender() 
    {
        canvasList.PostDraw();
    }

    public virtual void Exit() 
    {
        canvasList.Unload();
    }

    internal ContentManager GetContent() 
    {
        return Content;
    }

    public void SortEntities() 
    {
        entityList.SortEntities();
    }

    public void ChangeScene(Scene scene) => GameApp.Instance.Scene = scene;

    public T CreateEntity<T>() 
    where T : Entity, new()
    {
        var entity = new T();
        Add(entity);
        return entity;
    }

    public List<T> GetEntities<T>() 
    where T : Entity
    {
        var list = Enumerable.Empty<T>().ToList();
        foreach (var entity in entityList) 
        {
            if (entity is T ent) 
                list.Add(ent);
        }
        return list;
    }

    public List<Entity> GetEntitiesByTag(int tags) 
    {
        var list = Enumerable.Empty<Entity>().ToList();
        foreach (var entity in entityList) 
        {
            if ((entity.Tags & tags) != 0) 
            {
                list.Add(entity);
            }
        }
        return list;
    }
}
