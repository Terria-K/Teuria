using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public abstract class CanvasLayer 
{
    protected SpriteBatch SpriteBatch;
    protected Scene Scene;

    public abstract void Draw();
    public virtual void Unload() {}

    internal void Obtain(SpriteBatch spriteBatch, Scene scene) 
    {
        SpriteBatch = spriteBatch;
        Scene = scene;
        Ready();
    }

    public virtual void Ready() 
    {

    }
}