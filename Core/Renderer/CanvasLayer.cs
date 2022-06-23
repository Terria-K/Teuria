using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public abstract class CanvasLayer 
{
    protected SpriteBatch SpriteBatch;
    

    public abstract void Draw();

    internal void Obtain(SpriteBatch spriteBatch) 
    {
        SpriteBatch = spriteBatch;
    }
}