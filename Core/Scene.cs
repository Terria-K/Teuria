using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Scene 
{
    private List<Entity> entityList = new List<Entity>();
    private List<CanvasLayer> layers = new List<CanvasLayer>();
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

    public List<Entity> Entities 
    {
        get => entityList;
    }

    public Scene(ContentManager content, Camera camera) 
    {
        Content = content;
        Camera = camera;
    }

    public Scene(ContentManager content)
    {
        Content = content;
    }

    internal void Activate(SpriteBatch spriteBatch) 
    {
        this.SpriteBatch = spriteBatch;
    }

    public void Add(Entity entity, PauseMode pauseMode = PauseMode.Inherit) 
    {
        entity.PauseMode = pauseMode;
        entityList.Add(entity);
    }

    public void Add(CanvasLayer layer) 
    {
        layer.Obtain(SpriteBatch);
        layers.Add(layer);
    }

    public void Remove(CanvasLayer layer) 
    {
        layers.Remove(layer);
    }

    public void Remove(Entity entity) 
    {
        entity.Active = false;
        entity.ExitScene();
        entityList.Remove(entity);
    }

    public virtual void Initialize() {}
    public virtual void Ready(GraphicsDevice device) 
    {
        foreach(var entity in entityList) 
        {
            entity.EnterScene(this, Content);
            entity.Ready();
        }
    }
    public virtual void Update() 
    {
        if (Paused) 
        {
            ProcessEntityInPauseMode();
            return;
        }
        foreach(var entity in entityList) 
        {
            if (!entity.Active) continue;
            entity.Update();
        }
    }

    private void ProcessEntityInPauseMode() 
    {
        foreach(var entity in entityList) 
        {
            if (!entity.Active) continue;
            if (entity.PauseMode == PauseMode.Single)
                entity.Update();
        }
    }

    public virtual void Draw() 
    {
        foreach(var entity in entityList) 
        {
            if (!entity.Active) continue;
            entity.Draw(SpriteBatch);
        }

        foreach(var layer in layers) 
        {
            layer.Draw();
        }
    }

    public virtual void Exit() 
    {
        foreach(var entity in entityList) 
        {
            entity.ExitScene();
        }
    }
}