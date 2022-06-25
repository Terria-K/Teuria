using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public abstract class CanvasLayer 
{
    protected SpriteBatch SpriteBatch;
    

    public abstract void Draw();
    public virtual void Unload() {}

    internal void Obtain(SpriteBatch spriteBatch) 
    {
        SpriteBatch = spriteBatch;
        Ready();
    }

    public virtual void Ready() 
    {

    }
}