using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Scene 
{
    private List<Entity> entityList = new List<Entity>();
    protected ContentManager Content;
    protected Camera Camera;
    public event Action OnPause;
    public SceneRenderer SceneRenderer;
    private bool paused;
    public bool Paused 
    {
        get => paused;
        set
        {
            paused = value;
            OnPause?.Invoke();
        }
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

    public void Add(Entity entity, PauseMode pauseMode = PauseMode.Inherit) 
    {
        entity.PauseMode = pauseMode;
        entityList.Add(entity);
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
            entity.EnterScene(this);
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

    public virtual void Draw(SpriteBatch spriteBatch) 
    {
        foreach(var entity in entityList) 
        {
            if (!entity.Active) continue;
            entity.Draw(spriteBatch);
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