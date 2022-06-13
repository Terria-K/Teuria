using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public class Scene 
{
    private List<Entity> entityList = new List<Entity>();
    protected ContentManager Content;
    protected Camera Camera;

    public Scene(ContentManager content, Camera camera) 
    {
        Content = content;
        Camera = camera;
    }

    public Scene(ContentManager content)
    {
        Content = content;
    }

    public void Add(Entity entity) 
    {
        entityList.Add(entity);
    }

    public virtual void Initialize() {}
    public virtual void Ready(GraphicsDevice device) 
    {
        foreach(var entity in entityList) 
        {
            entity.Ready();
        }
    }
    public virtual void Update() 
    {
        foreach(var entity in entityList) 
        {
            if (!entity.Active) continue;
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

    public virtual void Exit() {}
}