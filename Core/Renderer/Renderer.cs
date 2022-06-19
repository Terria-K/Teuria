using Microsoft.Xna.Framework.Graphics;

namespace Teuria;

public abstract class Renderer 
{
    protected SpriteBatch SpriteBatch;
    // protected GraphicsDevice GraphicsDevice;

    public abstract void Draw();

    internal void Obtain(SpriteBatch spriteBatch) 
    {
        SpriteBatch = spriteBatch;
    }
}